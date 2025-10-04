namespace CampusLearn.Library.UserManagementModels;

public class TutorModule
{
    public int TutorID { get; set; }
    public int ModuleID { get; set; }
    public DateTime QualifiedSince { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;

    //Navigation Properties
    public Tutor Tutor { get; set; } = null!;
    public Module Module { get; set; } = null!;
}
