namespace CampusLearn.Library.UserManagementModels;

public class Module
{
    [Key]
    public int ModuleID { get; set; }

    [Required]
    [StringLength(100)]
    public string ModuleName { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string ModuleCode { get; set; } = string.Empty;

    [Required]
    public Qualification ProgramType { get; set; }

    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}
