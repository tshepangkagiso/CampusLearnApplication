namespace CampusLearn.UserProfileManagement.API.Database.Module_Data_Seeder;

public class DataSeeder : IDataSeeder
{
    private readonly UserManagementDbContext _context;

    public DataSeeder(UserManagementDbContext context)
    {
        _context = context;
    }

    public async Task SeedModulesAsync()
    {
        // Check if modules already exist to avoid duplicates
        if (await _context.Modules.AnyAsync())
        {
            return; // Database already seeded
        }

        var allModules = new List<Module>();
        allModules.AddRange(bcomModules);
        allModules.AddRange(bitModules);
        allModules.AddRange(dipModules);

        await _context.Modules.AddRangeAsync(allModules);
        await _context.SaveChangesAsync();
    }

    public List<Module> bcomModules = new List<Module>
    {
        // First Academic Year
        new Module { ModuleName = "Academic Writing 181", ModuleCode = "ACW181", ProgramType = Qualification.BCOM, Description = "" },
        new Module { ModuleName = "Computer Architecture 181", ModuleCode = "COA181", ProgramType = Qualification.BCOM, Description = "" },
        new Module { ModuleName = "Database Development 181", ModuleCode = "DBD181", ProgramType = Qualification.BCOM, Description = "" },
        new Module { ModuleName = "Information Systems 181", ModuleCode = "INF181", ProgramType = Qualification.BCOM, Description = "" },
        new Module { ModuleName = "Innovation and Leadership 101", ModuleCode = "INL101", ProgramType = Qualification.BCOM, Description = "" },
        new Module { ModuleName = "Innovation and Leadership 102", ModuleCode = "INL102", ProgramType = Qualification.BCOM, Description = "" },
        new Module { ModuleName = "Linear Programming 181", ModuleCode = "LPR181", ProgramType = Qualification.BCOM, Description = "" },
        new Module { ModuleName = "Mathematics 181", ModuleCode = "MAT181", ProgramType = Qualification.BCOM, Description = "" },
        new Module { ModuleName = "Networking Development 181", ModuleCode = "NWD181", ProgramType = Qualification.BCOM, Description = "" },
        new Module { ModuleName = "Programming 181", ModuleCode = "PRG181", ProgramType = Qualification.BCOM, Description = "" },
        new Module { ModuleName = "Programming 182", ModuleCode = "PRG182", ProgramType = Qualification.BCOM, Description = ""},
        new Module { ModuleName = "Statistics 181", ModuleCode = "STA181", ProgramType = Qualification.BCOM, Description = "" },
        new Module { ModuleName = "Web Programming 181", ModuleCode = "WPR181", ProgramType = Qualification.BCOM, Description = "" },
        new Module { ModuleName = "Business Management 181", ModuleCode = "BUM181", ProgramType = Qualification.BCOM, Description = "" },
        new Module { ModuleName = "Entrepreneurship 181", ModuleCode = "ENT181", ProgramType = Qualification.BCOM, Description = "" },

        // Second Academic Year
        new Module { ModuleName = "Database Development 281", ModuleCode = "DBD281", ProgramType = Qualification.BCOM, Description = ""},
        new Module { ModuleName = "Information Systems 281", ModuleCode = "INF281", ProgramType = Qualification.BCOM, Description = "" },
        new Module { ModuleName = "Innovation and Leadership 201", ModuleCode = "INL201", ProgramType = Qualification.BCOM, Description = "" },
        new Module { ModuleName = "Innovation and Leadership 202", ModuleCode = "INL202", ProgramType = Qualification.BCOM, Description = "" },
        new Module { ModuleName = "Linear Programming 281", ModuleCode = "LPR281", ProgramType = Qualification.BCOM, Description = "" },
        new Module { ModuleName = "Mathematics 281", ModuleCode = "MAT281", ProgramType = Qualification.BCOM, Description = "" },
        new Module { ModuleName = "Programming 281", ModuleCode = "PRG281", ProgramType = Qualification.BCOM, Description = "" },
        new Module { ModuleName = "Programming 282", ModuleCode = "PRG282", ProgramType = Qualification.BCOM, Description = "" },
        new Module { ModuleName = "Project Management 281", ModuleCode = "PMM281", ProgramType = Qualification.BCOM, Description = "" },
        new Module { ModuleName = "Statistics 281", ModuleCode = "STA281", ProgramType = Qualification.BCOM, Description = "" },
        new Module { ModuleName = "Web Programming 281", ModuleCode = "WPR281", ProgramType = Qualification.BCOM, Description = "" },
        new Module { ModuleName = "Software Analysis & Design 281", ModuleCode = "SAD281", ProgramType = Qualification.BCOM, Description = "" },
        new Module { ModuleName = "Data Warehousing 281", ModuleCode = "DWH281", ProgramType = Qualification.BCOM, Description = "" },
        new Module { ModuleName = "Internet Of Things 281", ModuleCode = "IOT281", ProgramType = Qualification.BCOM, Description = "" },
        new Module { ModuleName = "Software Testing 281", ModuleCode = "SWT281", ProgramType = Qualification.BCOM, Description = "" },

        // Third Academic Year
        new Module { ModuleName = "Research Methods 381", ModuleCode = "RSH381", ProgramType = Qualification.BCOM, Description = "" },
        new Module { ModuleName = "Database Development 381", ModuleCode = "DBD381", ProgramType = Qualification.BCOM, Description = "" },
        new Module { ModuleName = "Innovation and Leadership 3Z1", ModuleCode = "INL3Z1", ProgramType = Qualification.BCOM, Description = ""                        },
        new Module { ModuleName = "Linear Programming 381", ModuleCode = "LPR381", ProgramType = Qualification.BCOM, Description = "" },
        new Module { ModuleName = "Machine Learning 381", ModuleCode = "MLC381", ProgramType = Qualification.BCOM, Description = "" },
        new Module { ModuleName = "Project 381", ModuleCode = "PRJ381", ProgramType = Qualification.BCOM, Description = "" },
        new Module { ModuleName = "Project Management 381", ModuleCode = "PMM381", ProgramType = Qualification.BCOM, Description = "" },
        new Module { ModuleName = "Programming 381", ModuleCode = "PRG381", ProgramType = Qualification.BCOM, Description = "" },
        new Module { ModuleName = "Software Engineering 381", ModuleCode = "SEN381", ProgramType = Qualification.BCOM, Description = "" },
        new Module { ModuleName = "Web Programming 381", ModuleCode = "WPR381", ProgramType = Qualification.BCOM, Description = "" },
        new Module { ModuleName = "Data Science 381", ModuleCode = "BIN381", ProgramType = Qualification.BCOM, Description = "" },
        new Module { ModuleName = "Database Administration 381", ModuleCode = "DBA381", ProgramType = Qualification.BCOM, Description = "" },
        new Module { ModuleName = "Statistics 381", ModuleCode = "STA381", ProgramType = Qualification.BCOM, Description = "" },
        new Module { ModuleName = "Innovation Management 381", ModuleCode = "INM381", ProgramType = Qualification.BCOM, Description = "" },
        new Module { ModuleName = "Machine Learning 3B2", ModuleCode = "MLC3B2", ProgramType = Qualification.BCOM, Description = "" },
        new Module { ModuleName = "User Experience Design 381", ModuleCode = "UAX381", ProgramType = Qualification.BCOM, Description = ""}
    };

    public List<Module> bitModules = new List<Module>
    {
        // First Academic Year - BIT
        new Module { ModuleName = "Academic Writing 171", ModuleCode = "ACW171", ProgramType = Qualification.BIT, Description = "" },
        new Module { ModuleName = "Computer Architecture 171", ModuleCode = "COAT71", ProgramType = Qualification.BIT, Description = "" },
        new Module { ModuleName = "Database Development 171", ModuleCode = "DBD171", ProgramType = Qualification.BIT, Description = "" },
        new Module { ModuleName = "English Communication 171", ModuleCode = "ENC171", ProgramType = Qualification.BIT, Description = "" },
        new Module { ModuleName = "Information Systems 171", ModuleCode = "INF171", ProgramType = Qualification.BIT, Description = "" },
        new Module { ModuleName = "Innovation and Leadership 101", ModuleCode = "INL101", ProgramType = Qualification.BIT, Description = "" },
        new Module { ModuleName = "Innovation and Leadership 102", ModuleCode = "INL102", ProgramType = Qualification.BIT, Description = "" },
        new Module { ModuleName = "Mathematics 171", ModuleCode = "MAT171", ProgramType = Qualification.BIT, Description = "" },
        new Module { ModuleName = "Networking Development 171", ModuleCode = "NWD171", ProgramType = Qualification.BIT, Description = "" },
        new Module { ModuleName = "Programming 171", ModuleCode = "PRG171", ProgramType = Qualification.BIT, Description = "" },
        new Module { ModuleName = "Programming 172", ModuleCode = "PRG172", ProgramType = Qualification.BIT, Description = "" },
        new Module { ModuleName = "Statistics 171", ModuleCode = "STA171", ProgramType = Qualification.BIT, Description = "" },
        new Module { ModuleName = "Web Programming 171", ModuleCode = "WPR171", ProgramType = Qualification.BIT, Description = "" },
        new Module { ModuleName = "Business Management 171", ModuleCode = "BUMT71", ProgramType = Qualification.BIT, Description = "" },
        new Module { ModuleName = "Entrepreneurship 171", ModuleCode = "ENTT71", ProgramType = Qualification.BIT, Description = "" },

        // Second Academic Year - BIT
        new Module { ModuleName = "Cloud-Native Application Architecture 271", ModuleCode = "CNA271", ProgramType = Qualification.BIT, Description = "" },
        new Module { ModuleName = "Database Development 221", ModuleCode = "DBD221", ProgramType = Qualification.BIT, Description = "" },
        new Module { ModuleName = "Enterprise Systems 271", ModuleCode = "ERP271", ProgramType = Qualification.BIT, Description = "" },
        new Module { ModuleName = "Ethics 271", ModuleCode = "ETH271", ProgramType = Qualification.BIT, Description = "" },
        new Module { ModuleName = "Information Systems 271", ModuleCode = "INF271", ProgramType = Qualification.BIT, Description = "" },
        new Module { ModuleName = "Innovation and Leadership 201", ModuleCode = "INL201", ProgramType = Qualification.BIT, Description = "" },
        new Module { ModuleName = "Innovation and Leadership 202", ModuleCode = "INL202", ProgramType = Qualification.BIT, Description = "" },
        new Module { ModuleName = "Linear Programming 171", ModuleCode = "LPR171", ProgramType = Qualification.BIT, Description = "" },
        new Module { ModuleName = "Programming 271", ModuleCode = "PRG271", ProgramType = Qualification.BIT, Description = "" },
        new Module { ModuleName = "Programming 272", ModuleCode = "PRG272", ProgramType = Qualification.BIT, Description = "" },
        new Module { ModuleName = "Project Management 271", ModuleCode = "PMM271", ProgramType = Qualification.BIT, Description = "" },
        new Module { ModuleName = "Statistics 271", ModuleCode = "STA271", ProgramType = Qualification.BIT, Description = "" },
        new Module { ModuleName = "Web Programming 271", ModuleCode = "WPR271", ProgramType = Qualification.BIT, Description = "" },
        new Module { ModuleName = "Internet Of Things 271", ModuleCode = "IOT271", ProgramType = Qualification.BIT, Description = "" },
        new Module { ModuleName = "Software Testing 271", ModuleCode = "SWT271", ProgramType = Qualification.BIT, Description = "" },

        // Third Academic Year - BIT
        new Module { ModuleName = "Business Intelligence 371", ModuleCode = "BIN371", ProgramType = Qualification.BIT, Description = "" },
        new Module { ModuleName = "Cloud-Native Application Programming 371", ModuleCode = "CNA371", ProgramType = Qualification.BIT, Description = "" },
        new Module { ModuleName = "Data Analytics 371", ModuleCode = "DAL371", ProgramType = Qualification.BIT, Description = "" },
        new Module { ModuleName = "Database Development 371", ModuleCode = "DBD371", ProgramType = Qualification.BIT, Description = "" },
        new Module { ModuleName = "Innovation and Leadership 371", ModuleCode = "INL371", ProgramType = Qualification.BIT, Description = "" },
        new Module { ModuleName = "Programming 371", ModuleCode = "PRG371", ProgramType = Qualification.BIT, Description = "" },
        new Module { ModuleName = "Project 371", ModuleCode = "PRJ371", ProgramType = Qualification.BIT, Description = "" },
        new Module { ModuleName = "Project Management 371", ModuleCode = "PMM371", ProgramType = Qualification.BIT, Description = "" },
        new Module { ModuleName = "Software Analysis & Design 371", ModuleCode = "SAD371", ProgramType = Qualification.BIT, Description = "" },
        new Module { ModuleName = "Software Engineering 371", ModuleCode = "SEN371", ProgramType = Qualification.BIT, Description = "" },
        new Module { ModuleName = "Web Programming 371", ModuleCode = "WPR371", ProgramType = Qualification.BIT, Description = "" },
        new Module { ModuleName = "Innovation Management 371", ModuleCode = "INM371", ProgramType = Qualification.BIT, Description = "" },
        new Module { ModuleName = "User Experience Design 371", ModuleCode = "UAX371", ProgramType = Qualification.BIT, Description = "" }
    };

    public List<Module> dipModules = new List<Module>
    {
        // First Year - Core DIP
        new Module { ModuleName = "Business Communication 161", ModuleCode = "BUC161", ProgramType = Qualification.DIP, Description = "" },
        new Module { ModuleName = "Business Management and Entrepreneurship 161", ModuleCode = "BME161", ProgramType = Qualification.DIP, Description = "" },
        new Module { ModuleName = "Computer Architecture 161", ModuleCode = "COA161", ProgramType = Qualification.DIP, Description = "" },
        new Module { ModuleName = "Database Concept 161", ModuleCode = "DBC161", ProgramType = Qualification.DIP, Description = "" },
        new Module { ModuleName = "Database Functionality 161", ModuleCode = "DBF161", ProgramType = Qualification.DIP, Description = "" },
        new Module { ModuleName = "End User Computing 161", ModuleCode = "EUC161", ProgramType = Qualification.DIP, Description = "" },
        new Module { ModuleName = "Innovation and Leadership 161", ModuleCode = "INL161", ProgramType = Qualification.DIP, Description = "" },
        new Module { ModuleName = "Internet of Things 161", ModuleCode = "IOT161", ProgramType = Qualification.DIP, Description = "" },
        new Module { ModuleName = "Mathematics 161", ModuleCode = "MAT161", ProgramType = Qualification.DIP, Description = "" },
        new Module { ModuleName = "Network Development 161", ModuleCode = "NWD161", ProgramType = Qualification.DIP, Description = "" },
        new Module { ModuleName = "Problem Solving 161", ModuleCode = "PRS161", ProgramType = Qualification.DIP, Description = "" },
        new Module { ModuleName = "Programming 161", ModuleCode = "PRG161", ProgramType = Qualification.DIP, Description = "" },
        new Module { ModuleName = "Programming Principles 161", ModuleCode = "PRP161", ProgramType = Qualification.DIP, Description = "" },
        new Module { ModuleName = "Web Programming 161", ModuleCode = "WPR161", ProgramType = Qualification.DIP, Description = "" },
        new Module { ModuleName = "Statistics 161", ModuleCode = "STA161", ProgramType = Qualification.DIP, Description = "" },

        // Second Year - Core DIP
        new Module { ModuleName = "Database Development 261", ModuleCode = "DBD261", ProgramType = Qualification.DIP, Description = "" },
        new Module { ModuleName = "Enterprise Systems 261", ModuleCode = "ERP261", ProgramType = Qualification.DIP, Description = "" },
        new Module { ModuleName = "Innovation and Leadership 261", ModuleCode = "INL261", ProgramType = Qualification.DIP, Description = "" },
        new Module { ModuleName = "IT Law and Ethics 261", ModuleCode = "ILE261", ProgramType = Qualification.DIP, Description = "" },
        new Module { ModuleName = "Project Management 261", ModuleCode = "PMM261", ProgramType = Qualification.DIP, Description = "" },

        // Web Development Specialization
        new Module { ModuleName = "Web Database 361", ModuleCode = "WDB35G1", ProgramType = Qualification.DIP, Description = "" },
        new Module { ModuleName = "Web Frontend Scripting 361", ModuleCode = "WFS361", ProgramType = Qualification.DIP, Description = "" },
        new Module { ModuleName = "Web Servers 361", ModuleCode = "WSE361", ProgramType = Qualification.DIP, Description = "" },

        // Programming Specialization
        new Module { ModuleName = "Programming 361", ModuleCode = "PRG361", ProgramType = Qualification.DIP, Description = "" },
        new Module { ModuleName = "Scripting and Syntax 361", ModuleCode = "SSX361", ProgramType = Qualification.DIP, Description = "" },
        new Module { ModuleName = "Software Analysis and Design 361", ModuleCode = "SWA361-V1", ProgramType = Qualification.DIP, Description = "" },

        // Cloud Specialization
        new Module { ModuleName = "Cloud-Native Architecture 361", ModuleCode = "CNA361", ProgramType = Qualification.DIP, Description = "" },
        new Module { ModuleName = "Database Administration 361", ModuleCode = "DBA361", ProgramType = Qualification.DIP, Description = "" },
        new Module { ModuleName = "Database Cloud 361", ModuleCode = "DBC361", ProgramType = Qualification.DIP, Description = "" },
        new Module { ModuleName = "Database Development 361", ModuleCode = "DBD361", ProgramType = Qualification.DIP, Description = "" },
        new Module { ModuleName = "Database Reporting 361", ModuleCode = "DBR361", ProgramType = Qualification.DIP, Description = "" },
        new Module { ModuleName = "Distributed Database 361", ModuleCode = "DDB361", ProgramType = Qualification.DIP, Description = "" },
        new Module { ModuleName = "Database Administration 361", ModuleCode = "DBA361", ProgramType = Qualification.DIP, Description = "" },
        new Module { ModuleName = "Database Cloud 361", ModuleCode = "DBC361", ProgramType = Qualification.DIP, Description = "" },
        new Module { ModuleName = "Database Development 361", ModuleCode = "DBD361", ProgramType = Qualification.DIP, Description = "" },
        new Module { ModuleName = "Database Reporting 361", ModuleCode = "DBR361", ProgramType = Qualification.DIP, Description = "" },
        new Module { ModuleName = "Distributed Database 361", ModuleCode = "DDB361", ProgramType = Qualification.DIP, Description = "" },

        // Security Specialization
        new Module { ModuleName = "Ethical Hacking 361", ModuleCode = "EHA361", ProgramType = Qualification.DIP, Description = "" },
        new Module { ModuleName = "Penetration Testing 361", ModuleCode = "PET361", ProgramType = Qualification.DIP, Description = "" },

        // Networking Specialization
        new Module { ModuleName = "DevOps 361", ModuleCode = "DOP361", ProgramType = Qualification.DIP, Description = "" },
        new Module { ModuleName = "Mainframe 361", ModuleCode = "MFR361", ProgramType = Qualification.DIP, Description = "" },
        new Module { ModuleName = "Network Development 361", ModuleCode = "NWD361", ProgramType = Qualification.DIP, Description = "" },
        new Module { ModuleName = "Operating Systems 361", ModuleCode = "OPS361", ProgramType = Qualification.DIP, Description = "" }
    };




    public async Task SeedUsersAsync()
    {
        // Check if users already exist to avoid duplicates
        if (await _context.UserProfiles.AnyAsync())
        {
            return; // Database already seeded
        }

        var users = new List<UserProfile>();
        var logins = new List<Login>();
        var students = new List<Student>();
        var tutors = new List<Tutor>();

        var hashingService = new HashingService();
        var password = "password123";

        // Real human first names arrays
        var dipFirstNames = new[] { "Liam", "Noah", "Oliver", "Elijah", "James", "William", "Benjamin", "Lucas", "Henry", "Alexander" };
        var bitFirstNames = new[] { "Michael", "Daniel", "Matthew", "David", "Joseph", "Andrew", "Christopher", "Joshua", "Samuel", "Ryan" };
        var bcomFirstNames = new[] { "Emma", "Olivia", "Ava", "Isabella", "Sophia", "Charlotte", "Mia", "Amelia", "Harper", "Evelyn" };
        var tutorFirstNames = new[] { "Thomas", "Emily", "Robert", "Sarah", "Kevin", "Jessica", "Brian", "Nicole", "Jason" };
        var adminFirstNames = new[] { "Admin", "System" };

        // Real human surnames arrays
        var studentSurnames = new[] {
        "Smith", "Johnson", "Williams", "Brown", "Jones",
        "Garcia", "Miller", "Davis", "Rodriguez", "Martinez",
        "Hernandez", "Lopez", "Gonzalez", "Wilson", "Anderson",
        "Thomas", "Taylor", "Moore", "Jackson", "Martin",
        "Lee", "Perez", "Thompson", "White", "Harris",
        "Sanchez", "Clark", "Ramirez", "Lewis", "Robinson"
    };

        var tutorSurnames = new[] {
        "Wilson", "Anderson", "Thomas", "Taylor", "Moore",
        "Jackson", "Martin", "Lee", "Perez", "Thompson"
    };

        var adminSurnames = new[] { "Administrator", "Manager" };

        // Seed 30 Students (10 DIP, 10 BIT, 10 BCOM)
        for (int i = 1; i <= 30; i++)
        {
            var qualification = i <= 10 ? Qualification.DIP :
                               i <= 20 ? Qualification.BIT : Qualification.BCOM;

            var studentNumber = 200000 + i;

            // Get appropriate name based on qualification and index
            string firstName;
            if (qualification == Qualification.DIP)
                firstName = dipFirstNames[i - 1];
            else if (qualification == Qualification.BIT)
                firstName = bitFirstNames[i - 11];
            else
                firstName = bcomFirstNames[i - 21];

            var userProfile = new UserProfile
            {
                Name = firstName,
                Surname = studentSurnames[i - 1], // Use real surname
                Email = $"{studentNumber}@student.belgiumcampus.ac.za",
                UserRole = UserRole.Student,
                Qualification = qualification,
                StudentNumber = studentNumber,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };
            users.Add(userProfile);
        }

        // Seed 9 Tutors (3 DIP, 3 BIT, 3 BCOM)
        for (int i = 1; i <= 9; i++)
        {
            var qualification = i <= 3 ? Qualification.DIP :
                               i <= 6 ? Qualification.BIT : Qualification.BCOM;

            var studentNumber = 300000 + i;

            // Get tutor name and surname
            var firstName = tutorFirstNames[i - 1];
            var surname = tutorSurnames[i - 1];

            var userProfile = new UserProfile
            {
                Name = firstName,
                Surname = surname,
                Email = $"{studentNumber}@student.belgiumcampus.ac.za",
                UserRole = UserRole.Tutor,
                Qualification = qualification,
                StudentNumber = studentNumber,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };
            users.Add(userProfile);
        }

        // Seed 2 Admins
        for (int i = 1; i <= 2; i++)
        {
            var studentNumber = 400000 + i;

            var firstName = adminFirstNames[i - 1];
            var surname = adminSurnames[i - 1];

            var userProfile = new UserProfile
            {
                Name = firstName,
                Surname = surname,
                Email = $"{studentNumber}@student.belgiumcampus.ac.za",
                UserRole = UserRole.Admin,
                Qualification = Qualification.BIT,
                StudentNumber = studentNumber,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };
            users.Add(userProfile);
        }

        // Save users first to get their IDs
        await _context.UserProfiles.AddRangeAsync(users);
        await _context.SaveChangesAsync();

        // Create logins and role-specific records
        foreach (var user in users)
        {
            var (hash, salt) = hashingService.HashPassword(password);

            var login = new Login
            {
                Email = user.Email,
                PasswordHash = hash,
                PasswordSalt = salt,
                UserProfileID = user.UserProfileID,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };
            logins.Add(login);

            // Create Student/Tutor records based on role
            if (user.UserRole == UserRole.Student)
            {
                students.Add(new Student { UserProfileID = user.UserProfileID });
            }
            else if (user.UserRole == UserRole.Tutor || user.UserRole == UserRole.Admin)
            {
                tutors.Add(new Tutor
                {
                    UserProfileID = user.UserProfileID,
                    IsAdmin = (user.UserRole == UserRole.Admin)
                });
            }
        }

        await _context.Logins.AddRangeAsync(logins);
        await _context.Students.AddRangeAsync(students);
        await _context.Tutors.AddRangeAsync(tutors);
        await _context.SaveChangesAsync();

        // Auto-create subscriptions after users are created
        await SeedAutomaticSubscriptionsAsync();
    }



    private async Task SeedAutomaticSubscriptionsAsync()
    {
        var studentModules = new List<StudentModule>();
        var tutorModules = new List<TutorModule>();

        // Get all students and tutors
        var students = await _context.Students
            .Include(s => s.UserProfile)
            .ToListAsync();

        var tutors = await _context.Tutors
            .Include(t => t.UserProfile)
            .Where(t => !t.IsAdmin) // Exclude admins
            .ToListAsync();

        // Get all modules
        var allModules = await _context.Modules.ToListAsync();

        // Auto-subscribe students to modules matching their qualification
        foreach (var student in students)
        {
            var studentQualificationModules = allModules
                .Where(m => m.ProgramType == student.UserProfile.Qualification)
                .ToList();

            // Subscribe to 3-5 random modules from their qualification
            var random = new Random();
            var modulesToSubscribe = studentQualificationModules
                .OrderBy(x => random.Next())
                .Take(random.Next(3, 6)) // 3 to 5 modules
                .ToList();

            foreach (var module in modulesToSubscribe)
            {
                // Check if not already subscribed
                var existing = await _context.StudentModules
                    .FirstOrDefaultAsync(sm => sm.StudentID == student.StudentID && sm.ModuleID == module.ModuleID);

                if (existing == null)
                {
                    studentModules.Add(new StudentModule
                    {
                        StudentID = student.StudentID,
                        ModuleID = module.ModuleID,
                        SubscribedAt = DateTime.UtcNow
                    });
                }
            }
        }

        // Auto-qualify tutors for modules matching their qualification
        foreach (var tutor in tutors)
        {
            var tutorQualificationModules = allModules
                .Where(m => m.ProgramType == tutor.UserProfile.Qualification)
                .ToList();

            // Qualify for 4-8 random modules from their qualification
            var random = new Random();
            var modulesToQualify = tutorQualificationModules
                .OrderBy(x => random.Next())
                .Take(random.Next(4, 9)) // 4 to 8 modules
                .ToList();

            foreach (var module in modulesToQualify)
            {
                // Check if not already qualified
                var existing = await _context.TutorModules
                    .FirstOrDefaultAsync(tm => tm.TutorID == tutor.TutorID && tm.ModuleID == module.ModuleID);

                if (existing == null)
                {
                    tutorModules.Add(new TutorModule
                    {
                        TutorID = tutor.TutorID,
                        ModuleID = module.ModuleID,
                        QualifiedSince = DateTime.UtcNow,
                        IsActive = true
                    });
                }
            }
        }

        // Save all subscriptions and qualifications
        if (studentModules.Any())
        {
            await _context.StudentModules.AddRangeAsync(studentModules);
        }

        if (tutorModules.Any())
        {
            await _context.TutorModules.AddRangeAsync(tutorModules);
        }

        await _context.SaveChangesAsync();
    }
}
