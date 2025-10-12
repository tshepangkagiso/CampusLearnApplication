namespace CampusLearn.TopicsManagement.API.Controllers.QueryTopics.DTOs;

public class TutorInfo
{
    public int TutorID { get; set; }
    public int UserProfileID { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
}