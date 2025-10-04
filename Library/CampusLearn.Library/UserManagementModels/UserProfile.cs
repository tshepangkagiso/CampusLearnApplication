namespace CampusLearn.Library.UserManagementModels;

public enum UserRole { Student, Tutor, Admin }
public enum Qualification { DIP, BIT, BCOM }
public class UserProfile
{
    [Key]
    public int UserProfileID { get; set; }

    [Required]
    public UserRole UserRole { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Surname { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;

    [Url]
    public string ProfilePictureUrl { get; set; } = string.Empty;

    [Required]
    public Qualification Qualification { get; set; }

    [Required]
    [Range(100000, 999999)]
    public int StudentNumber { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLogin { get; set; }
    public bool IsActive { get; set; } = true;
}
