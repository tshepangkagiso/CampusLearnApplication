namespace CampusLearn.Library.TopicModels;

public class QueryResponse
{
    [Key]
    public int QueryResponseID { get; set; }

    [Required]
    public string Comment { get; set; } = string.Empty;

    [Url]
    public string? MediaContentUrl { get; set; }

    public DateTime ResponseCreationDate { get; set; } = DateTime.UtcNow;
    public bool IsSolution { get; set; } = false;
    public int HelpfulVotes { get; set; } = 0;

    // Foreign keys (references to User Management service)
    public int TutorID { get; set; }

    [Required]
    public int QueryTopicID { get; set; }

    // Navigation property (within this service)
    public QueryTopic QueryTopic { get; set; } = null!;
}
