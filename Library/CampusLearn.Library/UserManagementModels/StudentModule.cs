namespace CampusLearn.Library.UserManagementModels;

public class StudentModule
{
    public int StudentID { get; set; }
    public int ModuleID { get; set; }
    public DateTime SubscribedAt { get; set; } = DateTime.UtcNow;

    //Navigation Properties
    public Student Student { get; set; } = null!;
    public Module Module { get; set; } = null!;
}
