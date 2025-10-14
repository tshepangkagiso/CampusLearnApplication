namespace CampusLearn.Code.Library.PrivateMessageModels;

public class ChatRooms
{
    [Key]
    public int RoomId { get; set; }
    [Required]
    public int StudentId { get; set; }
    [Required]
    public int TutorId { get; set; }
    [Required]
    public int QueryId { get; set; }
    public string Title { get; set; } = string.Empty;
    [Required]
    public string ModuleCode { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation property
    public ICollection<Messages> Messages { get; set; } = new List<Messages>();
}
