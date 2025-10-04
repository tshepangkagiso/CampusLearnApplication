namespace CampusLearn.Library.UserManagementModels;

public class Login
{
    [Key]
    public int LoginID { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password{ get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastPasswordChange { get; set; }
    public bool IsActive { get; set; } = true;
    public int FailedLoginAttempts { get; set; } = 0;
    public DateTime? LockoutEnd { get; set; }

    [Required]
    public int UserProfileID { get; set; }

    //Navigation Properties
    public UserProfile UserProfile { get; set; } = null!;
}
