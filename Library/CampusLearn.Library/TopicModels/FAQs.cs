namespace CampusLearn.Library.TopicModels;

public class FAQs
{
    [Key]
    public int FAQID { get; set; }

    [Required]
    [StringLength(500)]
    public string FrequentlyAskedQuestion { get; set; } = string.Empty;

    [Required]
    public string Answer { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsPublished { get; set; } = true;
    public int ViewCount { get; set; } = 0;

    // Foreign keys (references to User Management service)
    public int TutorID { get; set; }
    public string ModuleCode { get; set; } = string.Empty;
}

