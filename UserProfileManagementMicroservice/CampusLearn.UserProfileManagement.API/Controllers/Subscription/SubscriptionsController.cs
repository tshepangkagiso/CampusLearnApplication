namespace CampusLearn.UserProfileManagement.API.Controllers.Subscription;

[Route("[controller]")]
[ApiController]
public class SubscriptionsController(UserManagementDbContext context) : ControllerBase
{

    //student subscription to module
    [HttpPost("module/student/subscribe")]
    public async Task<IActionResult> OnStudentSubscription(SubscribeDTO subscribeDTO)
    {
        try
        {
            var student = await context.Students
                .Include(s => s.UserProfile)  
                .FirstOrDefaultAsync(s => s.UserProfileID == subscribeDTO.UserId);

            if (student == null) return BadRequest("Student not found");

            var module = await context.Modules.FirstOrDefaultAsync(m => m.ModuleCode == subscribeDTO.moduleCode);
            if (module == null) return BadRequest("Module not found");

            // Check if already subscribed
            var existingSubscription = await context.StudentModules
                .FirstOrDefaultAsync(sm => sm.StudentID == student.StudentID && sm.ModuleID == module.ModuleID);

            if (existingSubscription != null) return BadRequest("Already subscribed to this module");

            // Ensure module matches student's qualification
            if (module.ProgramType != student.UserProfile.Qualification)  // ← This will work now
                return BadRequest("Module does not match your qualification");

            var studentModule = new StudentModule();
            studentModule.StudentID = student.StudentID;
            studentModule.ModuleID = module.ModuleID;

            await context.StudentModules.AddAsync(studentModule);
            await context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Subscribed successfully",
                Module = module.ModuleName,
                SubscriptionDate = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message, ex);
            return BadRequest();
        }
    }

    //student unsubscribing to module
    [HttpPost("module/student/unsubscribe")]
    public async Task<IActionResult> OnStudentUnSubscribing(SubscribeDTO subscribeDTO)
    {
        try
        {
            var student = await context.Students
                .FirstOrDefaultAsync(s => s.UserProfileID == subscribeDTO.UserId);
            if (student == null) return BadRequest("Student not found");

            var module = await context.Modules
                .FirstOrDefaultAsync(m => m.ModuleCode == subscribeDTO.moduleCode);
            if (module == null) return BadRequest("Module not found");

            // Find the subscription
            var subscription = await context.StudentModules
                .FirstOrDefaultAsync(sm => sm.StudentID == student.StudentID && sm.ModuleID == module.ModuleID);

            if (subscription == null) return BadRequest("Not subscribed to this module");

            // Remove the subscription
            context.StudentModules.Remove(subscription);
            await context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Unsubscribed successfully",
                Module = module.ModuleName
            });
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message, ex);
            return BadRequest();
        }
    }

    //all modules student subscribed to
    [HttpGet("module/students/subscribed/{userId}")]
    public async Task<IActionResult> OnStudentsSubscribed([FromRoute] int userId)
    {
        try
        {
            var student = await context.Students
                .Include(s => s.UserProfile)
                .FirstOrDefaultAsync(s => s.UserProfileID == userId);

            if (student == null) return BadRequest("Student not found");

            // Get all modules the student is subscribed to
            var subscribedModules = await context.StudentModules
                .Where(sm => sm.StudentID == student.StudentID)
                .Include(sm => sm.Module)
                .Select(sm => new
                {
                    ModuleID = sm.Module.ModuleID,
                    ModuleCode = sm.Module.ModuleCode,
                    ModuleName = sm.Module.ModuleName,
                    ProgramType = sm.Module.ProgramType,
                    SubscribedAt = sm.SubscribedAt
                })
                .ToListAsync();

            return Ok(new
            {
                Student = $"{student.UserProfile.Name} {student.UserProfile.Surname}",
                SubscribedModules = subscribedModules,
                TotalSubscriptions = subscribedModules.Count
            });
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message, ex);
            return BadRequest();
        }
    }
    //all modules student is not subscribed to
    [HttpGet("module/students/notsubscribed/{userId}")]
    public async Task<IActionResult> OnStudentsNotSubscribed([FromRoute] int userId)
    {
        try
        {
            var student = await context.Students
                .Include(s => s.UserProfile)
                .FirstOrDefaultAsync(s => s.UserProfileID == userId);

            if (student == null) return BadRequest("Student not found");

            // Get all modules the student IS subscribed to (to exclude them)
            var subscribedModuleIds = await context.StudentModules
                .Where(sm => sm.StudentID == student.StudentID)
                .Select(sm => sm.ModuleID)
                .ToListAsync();

            // Get all modules NOT subscribed to that match student's qualification
            var notSubscribedModules = await context.Modules
                .Where(m => m.ProgramType == student.UserProfile.Qualification &&
                           !subscribedModuleIds.Contains(m.ModuleID))
                .Select(m => new
                {
                    ModuleID = m.ModuleID,
                    ModuleCode = m.ModuleCode,
                    ModuleName = m.ModuleName,
                    ProgramType = m.ProgramType,
                    Description = m.Description
                })
                .ToListAsync();

            return Ok(new
            {
                Student = $"{student.UserProfile.Name} {student.UserProfile.Surname}",
                NotSubscribedModules = notSubscribedModules,
                TotalAvailable = notSubscribedModules.Count
            });
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message, ex);
            return BadRequest();
        }
    }

    // Get all students subscribed to a particular module
    [HttpGet("module/{moduleCode}/students")]
    public async Task<IActionResult> GetStudentsSubscribedToModule([FromRoute] string moduleCode)
    {
        try
        {
            var module = await context.Modules
                .FirstOrDefaultAsync(m => m.ModuleCode == moduleCode);
            if (module == null) return BadRequest("Module not found");

            var subscribedStudents = await context.StudentModules
                .Where(sm => sm.ModuleID == module.ModuleID)
                .Include(sm => sm.Student)
                    .ThenInclude(s => s.UserProfile)
                .Select(sm => new
                {
                    StudentID = sm.Student.StudentID,
                    UserProfileID = sm.Student.UserProfileID,
                    Name = sm.Student.UserProfile.Name,
                    Surname = sm.Student.UserProfile.Surname,
                    Email = sm.Student.UserProfile.Email,
                    StudentNumber = sm.Student.UserProfile.StudentNumber,
                    Qualification = sm.Student.UserProfile.Qualification,
                    SubscribedAt = sm.SubscribedAt
                })
                .ToListAsync();

            return Ok(new
            {
                Module = module.ModuleName,
                ModuleCode = module.ModuleCode,
                SubscribedStudents = subscribedStudents,
                TotalStudents = subscribedStudents.Count
            });
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message, ex);
            return BadRequest();
        }
    }





    //tutor subscription to module
    [HttpPost("module/tutor/subscribe")]
    public async Task<IActionResult> OnTutorsSubscription(SubscribeDTO subscribeDTO)
    {
        try
        {
            var tutor = await context.Tutors
                .Include(t => t.UserProfile)
                .FirstOrDefaultAsync(t => t.UserProfileID == subscribeDTO.UserId);

            if (tutor == null) return BadRequest("Tutor not found");

            var module = await context.Modules
                .FirstOrDefaultAsync(m => m.ModuleCode == subscribeDTO.moduleCode);
            if (module == null) return BadRequest("Module not found");

            // Check if already qualified
            var existingQualification = await context.TutorModules
                .FirstOrDefaultAsync(tm => tm.TutorID == tutor.TutorID && tm.ModuleID == module.ModuleID);

            if (existingQualification != null) return BadRequest("Already qualified for this module");

            // Ensure module matches tutor's qualification
            if (module.ProgramType != tutor.UserProfile.Qualification)
                return BadRequest("Module does not match your qualification");

            var tutorModule = new TutorModule
            {
                TutorID = tutor.TutorID,
                ModuleID = module.ModuleID,
                QualifiedSince = DateTime.UtcNow,
                IsActive = true
            };

            await context.TutorModules.AddAsync(tutorModule);
            await context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Successfully qualified for module",
                Module = module.ModuleName,
                QualifiedSince = tutorModule.QualifiedSince
            });
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message, ex);
            return BadRequest();
        }
    }

    //tutor unsubscribing to module
    [HttpPost("module/tutor/unsubscribe")]
    public async Task<IActionResult> OnTutorUnSubscribing(SubscribeDTO subscribeDTO)
    {
        try
        {
            var tutor = await context.Tutors
                .FirstOrDefaultAsync(t => t.UserProfileID == subscribeDTO.UserId);
            if (tutor == null) return BadRequest("Tutor not found");

            var module = await context.Modules
                .FirstOrDefaultAsync(m => m.ModuleCode == subscribeDTO.moduleCode);
            if (module == null) return BadRequest("Module not found");

            // Find the qualification
            var qualification = await context.TutorModules
                .FirstOrDefaultAsync(tm => tm.TutorID == tutor.TutorID && tm.ModuleID == module.ModuleID);

            if (qualification == null) return BadRequest("Not qualified for this module");

            // Remove the qualification
            context.TutorModules.Remove(qualification);
            await context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Successfully unqualified from module",
                Module = module.ModuleName
            });
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message, ex);
            return BadRequest();
        }
    }

    //all modules tutor subscribed to
    [HttpGet("module/tutors/subscribed/{userId}")]
    public async Task<IActionResult> OnTutorSubscribed([FromRoute] int userId)
    {
        try
        {
            var tutor = await context.Tutors
                .Include(t => t.UserProfile)
                .FirstOrDefaultAsync(t => t.UserProfileID == userId);

            if (tutor == null) return BadRequest("Tutor not found");

            // Get all modules the tutor is qualified for
            var qualifiedModules = await context.TutorModules
                .Where(tm => tm.TutorID == tutor.TutorID)
                .Include(tm => tm.Module)
                .Select(tm => new
                {
                    ModuleID = tm.Module.ModuleID,
                    ModuleCode = tm.Module.ModuleCode,
                    ModuleName = tm.Module.ModuleName,
                    ProgramType = tm.Module.ProgramType,
                    QualifiedSince = tm.QualifiedSince,
                    IsActive = tm.IsActive
                })
                .ToListAsync();

            return Ok(new
            {
                Tutor = $"{tutor.UserProfile.Name} {tutor.UserProfile.Surname}",
                QualifiedModules = qualifiedModules,
                TotalQualifications = qualifiedModules.Count
            });
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message, ex);
            return BadRequest();
        }
    }

    //all modules student is not subscribed to
    [HttpGet("module/tutors/notsubscribed/{userId}")]
    public async Task<IActionResult> OnTutorNotSubscribed([FromRoute] int userId)
    {
        try
        {
            var tutor = await context.Tutors
                .Include(t => t.UserProfile)
                .FirstOrDefaultAsync(t => t.UserProfileID == userId);

            if (tutor == null) return BadRequest("Tutor not found");

            // Get all modules the tutor IS qualified for (to exclude them)
            var qualifiedModuleIds = await context.TutorModules
                .Where(tm => tm.TutorID == tutor.TutorID && tm.IsActive)
                .Select(tm => tm.ModuleID)
                .ToListAsync();

            // Get all modules NOT qualified for that match tutor's qualification
            var notQualifiedModules = await context.Modules
                .Where(m => m.ProgramType == tutor.UserProfile.Qualification &&
                           !qualifiedModuleIds.Contains(m.ModuleID))
                .Select(m => new
                {
                    ModuleID = m.ModuleID,
                    ModuleCode = m.ModuleCode,
                    ModuleName = m.ModuleName,
                    ProgramType = m.ProgramType,
                    Description = m.Description
                })
                .ToListAsync();

            return Ok(new
            {
                Tutor = $"{tutor.UserProfile.Name} {tutor.UserProfile.Surname}",
                NotQualifiedModules = notQualifiedModules,
                TotalAvailable = notQualifiedModules.Count
            });
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message, ex);
            return BadRequest();
        }
    }

    // Get all tutors qualified for a particular module
    [HttpGet("module/{moduleCode}/tutors")]
    public async Task<IActionResult> GetTutorsQualifiedForModule([FromRoute] string moduleCode)
    {
        try
        {
            var module = await context.Modules
                .FirstOrDefaultAsync(m => m.ModuleCode == moduleCode);
            if (module == null) return BadRequest("Module not found");

            var qualifiedTutors = await context.TutorModules
                .Where(tm => tm.ModuleID == module.ModuleID && tm.IsActive)
                .Include(tm => tm.Tutor)
                    .ThenInclude(t => t.UserProfile)
                .Select(tm => new
                {
                    TutorID = tm.Tutor.TutorID,
                    UserProfileID = tm.Tutor.UserProfileID,
                    Name = tm.Tutor.UserProfile.Name,
                    Surname = tm.Tutor.UserProfile.Surname,
                    Email = tm.Tutor.UserProfile.Email,
                    StudentNumber = tm.Tutor.UserProfile.StudentNumber,
                    Qualification = tm.Tutor.UserProfile.Qualification,
                    IsAdmin = tm.Tutor.IsAdmin,
                    QualifiedSince = tm.QualifiedSince
                })
                .ToListAsync();

            return Ok(new
            {
                Module = module.ModuleName,
                ModuleCode = module.ModuleCode,
                QualifiedTutors = qualifiedTutors,
                TotalTutors = qualifiedTutors.Count
            });
        }
        catch (Exception ex)
        {
            Log.Error(ex.Message, ex);
            return BadRequest();
        }
    }

}
