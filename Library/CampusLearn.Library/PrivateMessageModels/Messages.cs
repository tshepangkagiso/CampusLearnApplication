using System.ComponentModel.DataAnnotations.Schema;

namespace CampusLearn.Code.Library.PrivateMessageModels;

public class Messages
{
    [Key]
    public int MessageId { get; set; }

    [ForeignKey("ChatRoom")]
    public int RoomId { get; set; }

    [Required]
    public int SenderId { get; set; }

    [Required]
    public string Content { get; set; } = string.Empty;

    public DateTime Timestamp { get; set; }

    public string MediaContentUrl { get; set; } = string.Empty;

    // Navigation property back to ChatRoom
    public ChatRooms ChatRoom { get; set; } = null!;
}
