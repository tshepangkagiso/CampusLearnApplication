namespace CampusLearn.Library.TopicModels;

public class QueryTopic
{
    [Key]
    public int QueryTopicID { get; set; }

    [Required]
    [StringLength(200)]
    public string QueryTopicTitle { get; set; } = string.Empty;

    [Required]
    public string QueryTopicDescription { get; set; } = string.Empty;

    [Required]
    public string RelatedModuleCode { get; set; } = string.Empty;

    public DateTime TopicCreationDate { get; set; } = DateTime.UtcNow;
    public DateTime? LastActivity { get; set; }
    public bool IsResolved { get; set; } = false;
    public bool IsUrgent { get; set; } = false;

    // Foreign key (reference to User Management service)
    public int StudentID { get; set; }

    // Navigation property (within this service)
    public ICollection<QueryResponse> Responses { get; set; } = new List<QueryResponse>();
}
