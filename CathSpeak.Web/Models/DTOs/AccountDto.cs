namespace CathSpeak.Web.Models.DTOs
{
    public class AccountDetailDto
    {
        public int AccountId { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? AvatarImageUrl { get; set; }
        public string? Address { get; set; }
        public string? DateOfBirth { get; set; }
        public string? Level { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime LastEdited { get; set; }
        public int Status { get; set; }
        public int RoleId { get; set; }
        public string? RoleName { get; set; }
    }

    public class ConversationDto
    {
        public int ConversationId { get; set; }
        public AccountDetailDto Friend { get; set; } = new();
        public string? LastMessage { get; set; }
        public DateTime? LastMessageTime { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime LastEdited { get; set; }
    }

    public class MessageDto
    {
        public int MessageId { get; set; }
        public int ConversationId { get; set; }
        public string MessageContent { get; set; } = string.Empty;
        public string MessageType { get; set; } = string.Empty;
        public bool IsRead { get; set; }
        public DateTime CreateDate { get; set; }
        public AccountDetailDto Sender { get; set; } = new();
    }

    public class RoomDto
    {
        public int RoomId { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int CreatorId { get; set; }
        public string? CreatorName { get; set; }
        public DateTime CreateDate { get; set; }
        public int Status { get; set; }
    }

    public class VideoChatSessionDto
    {
        public int SessionId { get; set; }
        public string? SessionToken { get; set; }
        public int SessionType { get; set; }
        public int? CreatorId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int RoomId { get; set; }
        public string? RoomName { get; set; }
        public int Status { get; set; }
        public List<VideoChatParticipantDto> Participants { get; set; } = new();
    }

    public class VideoChatParticipantDto
    {
        public int ParticipantId { get; set; }
        public int AccountId { get; set; }
        public string? Username { get; set; }
        public string? AvatarImageUrl { get; set; }
        public DateTime JoinTime { get; set; }
        public DateTime? LeaveTime { get; set; }
        public int Status { get; set; }
    }
}