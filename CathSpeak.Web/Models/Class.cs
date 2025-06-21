using System.Collections.Generic;

namespace CathSpeak.Web.Models
{
    /// <summary>
    /// Represents a STUN/TURN server configuration for WebRTC
    /// </summary>
    public class IceServer
    {
        /// <summary>
        /// URL for the ICE server (STUN/TURN)
        /// </summary>
        public string Urls { get; set; } = string.Empty;
        
        /// <summary>
        /// Username for TURN server authentication (optional)
        /// </summary>
        public string? Username { get; set; }
        
        /// <summary>
        /// Credential for TURN server authentication (optional)
        /// </summary>
        public string? Credential { get; set; }
        
        /// <summary>
        /// The type of credential (optional)
        /// </summary>
        public string? CredentialType { get; set; }
    }

    /// <summary>
    /// Provider for ICE server configurations used in WebRTC connections
    /// </summary>
    public class IceServerProvider
    {
        /// <summary>
        /// List of ICE servers for WebRTC peer connections
        /// </summary>
        public List<IceServer> IceServers { get; set; } = new List<IceServer>();
        
        /// <summary>
        /// Converts the list of IceServer objects to a JSON-serializable format
        /// for use in JavaScript/frontend
        /// </summary>
        public object GetServersForClient()
        {
            return IceServers.Select(server => new
            {
                urls = server.Urls,
                username = server.Username,
                credential = server.Credential,
                credentialType = server.CredentialType ?? "password"
            }).ToArray();
        }
    }
}
