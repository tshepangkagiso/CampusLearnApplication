using CampusLearn.Code.Library.PrivateMessageModels;
using Microsoft.EntityFrameworkCore;

namespace CampusLearn.PrivateMessaging.API.Database;

public class PrivateMessagesDbContext : DbContext
{
    public PrivateMessagesDbContext(DbContextOptions<PrivateMessagesDbContext> options) : base(options) { }

    public DbSet<ChatRooms> ChatRooms { get; set; }
    public DbSet<Messages> Messages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure ChatRooms -> Messages relationship
        modelBuilder.Entity<ChatRooms>()
            .HasMany(cr => cr.Messages)
            .WithOne(m => m.ChatRoom)
            .HasForeignKey(m => m.RoomId)
            .OnDelete(DeleteBehavior.Cascade);

        // Prevent duplicate chat rooms for same student-tutor-query
        modelBuilder.Entity<ChatRooms>()
            .HasIndex(cr => new { cr.StudentId, cr.TutorId, cr.QueryId })
            .IsUnique();

        base.OnModelCreating(modelBuilder);
    }
}
/* Use this to run Migrations:

    1) Add-Migration InitialCreate -Context PrivateMessagesDbContext -OutputDir Database\Migrations
    2) Update-Database -Context PrivateMessagesDbContext

    *NB IF THERE IS ALREADY A FOLDER CALLED Migrations inside the Database folder,
    *with .cs files those are the migrations script to turn c# code into sql. So run the Update-Database

    # Generates SQL script from initial migration to latest
    3) Script-Migration -Context PrivateMessagesDbContext -Output "Database\SQL\PrivateMessages.sql"
*/