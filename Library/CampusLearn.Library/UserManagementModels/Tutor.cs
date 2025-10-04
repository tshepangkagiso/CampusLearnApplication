namespace CampusLearn.Library.UserManagementModels;

public class Tutor
{
    [Key]
    public int TutorID { get; set; }

    [Required]
    public int UserProfileID { get; set; }

    public bool IsAdmin { get; set; } = false;
    public DateTime? AdminSince { get; set; }

    //Navigation Properties
    public UserProfile UserProfile { get; set; } = null!;
}
