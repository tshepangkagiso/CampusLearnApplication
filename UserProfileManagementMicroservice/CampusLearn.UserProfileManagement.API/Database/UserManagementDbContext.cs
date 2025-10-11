namespace CampusLearn.UserProfileManagement.API.Database;

public class UserManagementDbContext : DbContext
{
    public UserManagementDbContext(DbContextOptions<UserManagementDbContext> options) : base(options) { }

    public DbSet<UserProfile> UserProfiles { get; set; }
    public DbSet<Login> Logins { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<Tutor> Tutors { get; set; }
    public DbSet<Module> Modules { get; set; }
    public DbSet<StudentModule> StudentModules { get; set; }
    public DbSet<TutorModule> TutorModules { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // UserProfile configurations
        modelBuilder.Entity<UserProfile>()
            .HasIndex(u => u.Email)
            .IsUnique();
        modelBuilder.Entity<UserProfile>()
            .HasIndex(u => u.StudentNumber)
            .IsUnique();

        // Login configurations
        modelBuilder.Entity<Login>()
            .HasIndex(l => l.Email)
            .IsUnique();
        modelBuilder.Entity<Login>()
            .HasOne(l => l.UserProfile)
            .WithOne()
            .HasForeignKey<Login>(l => l.UserProfileID)
            .OnDelete(DeleteBehavior.Cascade);

        // Student configurations
        modelBuilder.Entity<Student>()
            .HasOne(s => s.UserProfile)
            .WithOne()
            .HasForeignKey<Student>(s => s.UserProfileID)
            .OnDelete(DeleteBehavior.Cascade);

        // Tutor configurations
        modelBuilder.Entity<Tutor>()
            .HasOne(t => t.UserProfile)
            .WithOne()
            .HasForeignKey<Tutor>(t => t.UserProfileID)
            .OnDelete(DeleteBehavior.Cascade);

        // Module configurations
        /*modelBuilder.Entity<Module>()
            .HasIndex(m => m.ModuleCode)
            .IsUnique();*/

        // StudentModule composite key
        modelBuilder.Entity<StudentModule>()
            .HasKey(sm => new { sm.StudentID, sm.ModuleID });
        modelBuilder.Entity<StudentModule>()
            .HasOne(sm => sm.Student)
            .WithMany()
            .HasForeignKey(sm => sm.StudentID)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<StudentModule>()
            .HasOne(sm => sm.Module)
            .WithMany()
            .HasForeignKey(sm => sm.ModuleID)
            .OnDelete(DeleteBehavior.Cascade);

        // TutorModule composite key
        modelBuilder.Entity<TutorModule>()
            .HasKey(tm => new { tm.TutorID, tm.ModuleID });
        modelBuilder.Entity<TutorModule>()
            .HasOne(tm => tm.Tutor)
            .WithMany()
            .HasForeignKey(tm => tm.TutorID)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<TutorModule>()
            .HasOne(tm => tm.Module)
            .WithMany()
            .HasForeignKey(tm => tm.ModuleID)
            .OnDelete(DeleteBehavior.Cascade);

        base.OnModelCreating(modelBuilder);
    }






    /* Use this to run Migrations:
     
        1) Add-Migration InitialCreate -Context UserManagementDbContext -OutputDir Database\Migrations
        2) Update-Database -Context UserManagementDbContext

        *NB IF THERE IS ALREADY A FOLDER CALLED Migrations inside the Database folder,
        *with .cs files those are the migrations script to turn c# code into sql. So run the Update-Database
        

        # Generates SQL script from initial migration to latest
        3) Script-Migration -Context UserManagementDbContext -Output "Database\SQL\UserProfileDB.sql"
     */
}
