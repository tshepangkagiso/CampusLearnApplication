namespace CampusLearn.Library;


//Following classes underneath a critical to user profile management: 
public class UserProfile
{
    [Key]
    public int UserProfileID { get; set; } // unique primary key
    [Required]
    public string UserRole { get; set; } = string.Empty; // student or tutor
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    public string Surname { get; set; } = string.Empty;
    [Required]
    public string Email { get; set; } = string.Empty; //campus student email [studentnumber]@student.belgiumcampus.ac.za,unqiue
    public string ProfilePictureUrl {  get; set; } = string.Empty;
    [Required]
    public string QualificationName {  get; set; } = string.Empty; // DIP, BIT, BCOM
    [Required]
    public int StudentNumber { get; set; } //campus given student number
}

public class Login
{
    [Key]
    public int LoginID { get; set; }
    [Required]
    public string Email { get; set; } = string.Empty; //campus student email [studentnumber]@student.belgiumcampus.ac.za,unique
    [Required]
    public string Password { get; set; } = string.Empty; //enforce strictness
}

public class Student
{
    [Key]
    public int StudentID { get; set; } // pk
    [Required]
    public int UserProfileID { get; set; } // fk, where role == student

    //navigition property
    public UserProfile? UserProfile { get; set; }
}

public class Tutor
{
    [Key]
    public int TutorID { get; set; } // pk
    [Required]
    public int UserProfileID { get; set; } // fk, where role == tutor
    public bool IsAdmin { get; set; } = false;

    //navigition property
    public UserProfile? UserProfile { get; set; }
}

public class Module
{
    [Key]
    public int ModuleID { get; set; } //pk
    [Required]
    public string ModuleName { get; set; } = string.Empty; //web programming , programming
    [Required]
    public string ModuleCode { get; set; } = string.Empty; //wpr181 or prg261 unique
    [Required]
    public string ProgramType { get; set; } = string.Empty; // BCOM, BIT OR DIP
}

public class StudentModule //subscribing a student to a module they need help with.
{
    public int StudentID { get; set; } // fk of specific student
    public int ModuleID { get; set; } // fk of specific module

    //navigition properties
    public Student? Student { get; set; }
    public Module? Module { get; set; }
}

public class TutorModule //subscribing a tutor to a module they give help in.
{
    public int TutorID { get; set; } // fk of specific tutor
    public int ModuleID { get; set; } // fk of specific module

    //navigition properties
    public Tutor? Tutor { get; set; }
    public Module? Module { get; set; }
}

public class UserProfileManagementDatabaseContext : DbContext
{
    public DbSet<UserProfile> UserProfiles { get; set; }
    public DbSet<Login> Logins { get; set; }
    public DbSet<Student> Students { get; set; }
    public DbSet<Tutor> Tutors { get; set; }
    public DbSet<Module> Modules { get; set; }
    public DbSet<StudentModule> StudentModules { get; set; }
    public DbSet<TutorModule> TutorModules { get; set; }

    public UserProfileManagementDatabaseContext(DbContextOptions<UserProfileManagementDatabaseContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //UserProfile
        modelBuilder.Entity<UserProfile>()
            .HasIndex(u => u.Email)
            .IsUnique();

        //Login
        modelBuilder.Entity<Login>()
            .HasIndex(l => l.Email)
            .IsUnique();

        //Module
        modelBuilder.Entity<Module>()
            .HasIndex(m => m.ModuleCode)
            .IsUnique();

        // UserProfile -> Student (1:1)
        modelBuilder.Entity<Student>()
            .HasOne(s => s.UserProfile)
            .WithOne() // UserProfile doesn’t need a collection
            .HasForeignKey<Student>(s => s.UserProfileID);

        // UserProfile -> Tutor (1:1)
        modelBuilder.Entity<Tutor>()
            .HasOne(t => t.UserProfile)
            .WithOne()
            .HasForeignKey<Tutor>(t => t.UserProfileID);

        // StudentModules (composite key)
        modelBuilder.Entity<StudentModule>()
            .HasKey(sm => new { sm.StudentID, sm.ModuleID });

        modelBuilder.Entity<StudentModule>()
            .HasOne(sm => sm.Student)
            .WithMany() // or WithMany(s => s.StudentModules) if you add a collection
            .HasForeignKey(sm => sm.StudentID);

        modelBuilder.Entity<StudentModule>()
            .HasOne(sm => sm.Module)
            .WithMany()
            .HasForeignKey(sm => sm.ModuleID);

        // TutorModules (composite key)
        modelBuilder.Entity<TutorModule>()
            .HasKey(tm => new { tm.TutorID, tm.ModuleID });

        modelBuilder.Entity<TutorModule>()
            .HasOne(tm => tm.Tutor)
            .WithMany()
            .HasForeignKey(tm => tm.TutorID);

        modelBuilder.Entity<TutorModule>()
            .HasOne(tm => tm.Module)
            .WithMany()
            .HasForeignKey(tm => tm.ModuleID);

        base.OnModelCreating(modelBuilder);
    }
}


//Following classes underneath a critical to topics:
public class FAQs
{
    [Key]
    public int FAQID { get; set; } // Primary key

    [Required]
    public string FrequentlyAskedQuestion { get; set; } = string.Empty;

    [Required]
    public string Answer { get; set; } = string.Empty;

    // Navigation Property
    public int TutorID { get; set; } // FK to Tutor
    public Tutor Tutor { get; set; } = null!;
}

public class QueryTopic
{
    [Key]
    public int QueryTopicID { get; set; }

    [Required]
    public string QueryTopicTitle { get; set; } = string.Empty;

    [Required]
    public string QueryTopicDescription { get; set; } = string.Empty;

    [Required]
    public string RelatedModuleCode { get; set; } = string.Empty;

    public DateTime TopicCreationDate { get; set; } = DateTime.UtcNow;

    [Required]
    public int StudentID { get; set; } // student who created it

    // Navigation Property
    public ICollection<QueryResponse> Responses { get; set; } = new List<QueryResponse>();
}

public class QueryResponse
{
    [Key]
    public int QueryResponseID { get; set; }

    [Required]
    public string Comment { get; set; } = string.Empty;

    public string MediaContentUrl { get; set; } = string.Empty;

    public DateTime ResponseCreationDate { get; set; } = DateTime.UtcNow;

    [Required]
    public int TutorID { get; set; } // tutor who responded
    public Tutor Tutor { get; set; } = null!;

    // Foreign key to QueryTopic
    [Required]
    public int QueryTopicID { get; set; }
    public QueryTopic QueryTopic { get; set; } = null!;
}

public class TopicDatabaseContext : DbContext
{
    public TopicDatabaseContext(DbContextOptions<TopicDatabaseContext> options) : base(options) { }
    public DbSet<FAQs> FAQs { get; set; }
    public DbSet<QueryTopic> QueryTopics { get; set; }
    public DbSet<QueryResponse> QueryResponses { get; set; }
    public DbSet<Tutor> Tutors { get; set; } // needed for FKs

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // FAQs -> Tutor (1:1)
        modelBuilder.Entity<FAQs>()
            .HasOne(f => f.Tutor)
            .WithMany() // assuming Tutor doesn't have a collection of FAQs
            .HasForeignKey(f => f.TutorID)
            .OnDelete(DeleteBehavior.Restrict);

        // QueryTopic -> QueryResponse (1:N)
        modelBuilder.Entity<QueryTopic>()
            .HasMany(qt => qt.Responses)
            .WithOne(qr => qr.QueryTopic)
            .HasForeignKey(qr => qr.QueryTopicID)
            .OnDelete(DeleteBehavior.Cascade);

        // QueryResponse -> Tutor (1:1)
        modelBuilder.Entity<QueryResponse>()
            .HasOne(qr => qr.Tutor)
            .WithMany() // assuming Tutor doesn't have a collection of responses
            .HasForeignKey(qr => qr.TutorID)
            .OnDelete(DeleteBehavior.Restrict);

        // Optional: Indexes for performance
        modelBuilder.Entity<QueryTopic>()
            .HasIndex(qt => qt.RelatedModuleCode);

        modelBuilder.Entity<QueryResponse>()
            .HasIndex(qr => qr.TutorID);
    }
}



//Following classes underneath a critical to forum:

public class Admins
{
    [Key]
    public int AdminID { get; set; }

    // Navigation Property
    public ICollection<ForumTopic> ForumTopics { get; set; } = new List<ForumTopic>();
}

public class ForumTopic
{
    [Key]
    public int ForumTopicID { get; set; }

    [Required]
    public string ForumTopicTitle { get; set; } = string.Empty;
    [Required]
    public string ForumTopicDescription { get; set; } = string.Empty;

    [Required]
    public string RelatedModuleCode { get; set; } = string.Empty;

    public int TopicUpVote { get; set; } = 0;

    public DateTime TopicCreationDate { get; set; } = DateTime.UtcNow;

    public int UserProfileID { get; set; } // reference to who made it

    // Navigation Property
    public ICollection<ForumTopicResponses> Responses { get; set; } = new List<ForumTopicResponses>();
}

public class ForumTopicResponses
{
    [Key]
    public int ResponseID { get; set; }

    [Required]
    public string Comment { get; set; } = string.Empty;

    public string MediaContentUrl { get; set; } = string.Empty;

    public int ResponseUpVote { get; set; } = 0;

    public DateTime ResponseCreationDate { get; set; } = DateTime.UtcNow;

    public int UserProfileID { get; set; } // reference to who made it

    // Foreign key to ForumTopic
    public int ForumTopicID { get; set; }
    public ForumTopic ForumTopic { get; set; } = null!;
}

public class ForumDatabaseContext : DbContext
{
    public ForumDatabaseContext(DbContextOptions<ForumDatabaseContext> options) : base(options) { }

    public DbSet<Admins> Admins { get; set; }
    public DbSet<ForumTopic> ForumTopics { get; set; }
    public DbSet<ForumTopicResponses> ForumTopicResponses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ForumTopic -> ForumTopicResponses (1:N)
        modelBuilder.Entity<ForumTopic>()
            .HasMany(ft => ft.Responses)
            .WithOne(r => r.ForumTopic)
            .HasForeignKey(r => r.ForumTopicID)
            .OnDelete(DeleteBehavior.Cascade);

        // Optional: indexes for performance
        modelBuilder.Entity<ForumTopic>()
            .HasIndex(ft => ft.RelatedModuleCode);

        modelBuilder.Entity<ForumTopicResponses>()
            .HasIndex(r => r.UserProfileID);
    }
}