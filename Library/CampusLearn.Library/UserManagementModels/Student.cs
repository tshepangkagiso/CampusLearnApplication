namespace CampusLearn.Library.UserManagementModels;

public class Student
{
    [Key]
    public int StudentID { get; set; }

    [Required]
    public int UserProfileID { get; set; }

    //Navigation Properties
    public UserProfile UserProfile { get; set; } = null!;
}
