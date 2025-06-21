using CathSpeak.Web.Services;
using CathSpeak.Web.Hubs;
using CathSpeak.Web.Models;
using CathSpeak.Web.Models; // Added namespace for IceServerProvider  

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.  
builder.Services.AddRazorPages();

// Add HttpClient for API calls  
builder.Services.AddHttpClient("CathSpeakAPI", client =>
{
    client.BaseAddress = new Uri("http://localhost:7001/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Register API Service  
builder.Services.AddScoped<IApiService, ApiService>();

// Add session support  
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
});

// Add authentication  
builder.Services.AddAuthentication("CookieAuth")
   .AddCookie("CookieAuth", options =>
   {
       options.LoginPath = "/Auth/Login";
       options.LogoutPath = "/Auth/Logout";
       options.AccessDeniedPath = "/Auth/AccessDenied";
       options.ExpireTimeSpan = TimeSpan.FromHours(8);
       options.SlidingExpiration = true;
   });

// Add authorization  
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdmin", policy => policy.RequireRole("Admin"));
});

// Add SignalR for real-time features  
builder.Services.AddSignalR();

// Add logging  
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Register IceServerProvider  
builder.Services.AddSingleton<IceServerProvider>(new IceServerProvider
{
    IceServers = new List<IceServer>
   {
       new IceServer { Urls = "stun:stun.l.google.com:19302" },
       new IceServer
       {
           Urls = "turn:your-turn-server.com:3478",
           Username = "username",
           Credential = "credential"
       }
   }
});

var app = builder.Build();

// Configure the HTTP request pipeline.  
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

// Map SignalR hubs  
app.MapHub<ChatHub>("/hubs/chat");
app.MapHub<VideoChatHub>("/hubs/videochat");

// Add default route  
app.MapGet("/", context =>
{
    context.Response.Redirect("/Index");
    return Task.CompletedTask;
});

app.Run();