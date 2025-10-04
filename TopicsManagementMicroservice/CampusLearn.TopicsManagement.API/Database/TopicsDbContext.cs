namespace CampusLearn.TopicsManagement.API.Database;

public class TopicsDbContext : DbContext
{
    public TopicsDbContext(DbContextOptions<TopicsDbContext> options) : base(options) { }

    public DbSet<FAQs> FAQs { get; set; }
    public DbSet<QueryTopic> QueryTopics { get; set; }
    public DbSet<QueryResponse> QueryResponses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // QueryTopic -> QueryResponse relationship
        modelBuilder.Entity<QueryTopic>()
            .HasMany(qt => qt.Responses)
            .WithOne(qr => qr.QueryTopic)
            .HasForeignKey(qr => qr.QueryTopicID)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes for performance
        modelBuilder.Entity<QueryTopic>()
            .HasIndex(qt => qt.RelatedModuleCode);
        modelBuilder.Entity<QueryTopic>()
            .HasIndex(qt => qt.IsResolved);
        modelBuilder.Entity<FAQs>()
            .HasIndex(f => f.ModuleCode);

        base.OnModelCreating(modelBuilder);
    }

    /* Use this to run Migrations:
     
        1) Add-Migration InitialCreate -Context TopicsDbContext -OutputDir Database\Migrations
        2) Update-Database -Context TopicsDbContext

        *NB IF THERE IS ALREADY A FOLDER CALLED Migrations inside the Database folder,
        *with .cs files those are the migrations script to turn c# code into sql. So run the Update-Database
        
        # Generates SQL script from initial migration to latest
        3) Script-Migration -Context TopicsDbContext -Output "Database\SQL\TopicsDB.sql"
    */
}
