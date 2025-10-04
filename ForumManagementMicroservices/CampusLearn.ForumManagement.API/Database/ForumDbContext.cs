namespace CampusLearn.ForumManagement.API.Database;

public class ForumDbContext : DbContext
{
    public ForumDbContext(DbContextOptions<ForumDbContext> options) : base(options) { }

    public DbSet<ForumTopic> ForumTopics { get; set; }
    public DbSet<ForumTopicResponse> ForumTopicResponses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ForumTopic -> ForumTopicResponse relationship
        modelBuilder.Entity<ForumTopic>()
            .HasMany(ft => ft.Responses)
            .WithOne(r => r.ForumTopic)
            .HasForeignKey(r => r.ForumTopicID)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes for performance
        modelBuilder.Entity<ForumTopic>()
            .HasIndex(ft => ft.RelatedModuleCode);
        modelBuilder.Entity<ForumTopic>()
            .HasIndex(ft => ft.IsPinned);
        modelBuilder.Entity<ForumTopic>()
            .HasIndex(ft => ft.LastActivity);
        modelBuilder.Entity<ForumTopicResponse>()
            .HasIndex(r => r.UserProfileID);

        base.OnModelCreating(modelBuilder);
    }

    /* Use this to run Migrations: 
     
        1) Add-Migration InitialCreate -Context ForumDbContext -OutputDir Database\Migrations
        2) Update-Database -Context ForumDbContext

        *NB IF THERE IS ALREADY A FOLDER CALLED Migrations inside the Database folder,
        *with .cs files those are the migrations script to turn c# code into sql. So run the Update-Database
        
        # Generates SQL script from initial migration to latest
        3) Script-Migration -Context ForumDbContext -Output "Database\SQL\ForumDB.sql"
    */
}
