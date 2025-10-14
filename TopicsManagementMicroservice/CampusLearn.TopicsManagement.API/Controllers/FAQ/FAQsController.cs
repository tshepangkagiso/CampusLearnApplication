using CampusLearn.TopicsManagement.API.Controllers.FAQ.DTOs;

namespace CampusLearn.TopicsManagement.API.Controllers.FAQ;

[Route("[controller]")]
[ApiController]
public class FAQsController(TopicsDbContext context) : ControllerBase
{
    // Create FAQ (Tutors only)
    [HttpPost("tutors/{tutorId}/faqs")]
    public async Task<IActionResult> CreateFAQ([FromRoute] int tutorId, [FromBody] CreateFAQRequest request)
    {
        try
        {
            var faq = new FAQs
            {
                FrequentlyAskedQuestion = request.Question,
                Answer = request.Answer,
                TutorID = tutorId,
                ModuleCode = request.ModuleCode,
                CreatedAt = DateTime.UtcNow,
                IsPublished = request.IsPublished
            };

            await context.FAQs.AddAsync(faq);
            await context.SaveChangesAsync();

            return Ok(new
            {
                FAQ = faq,
                Message = "FAQ created successfully"
            });
        }
        catch (Exception ex)
        {
            Log.Error("Failed to create FAQ", ex);
            return BadRequest("Failed to create FAQ");
        }
    }

    // Get all FAQs (Public - no auth required)
    [HttpGet("faqs")]
    public async Task<IActionResult> GetAllFAQs()
    {
        try
        {
            var faqs = await context.FAQs
                .Where(f => f.IsPublished)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();

            return Ok(new
            {
                TotalFAQs = faqs.Count,
                FAQs = faqs
            });
        }
        catch (Exception ex)
        {
            Log.Error("Failed to get FAQs", ex);
            return BadRequest("Failed to get FAQs");
        }
    }

    // Get FAQ by ID (Public)
    [HttpGet("faqs/{faqId}")]
    public async Task<IActionResult> GetFAQById([FromRoute] int faqId)
    {
        try
        {
            var faq = await context.FAQs
                .FirstOrDefaultAsync(f => f.FAQID == faqId && f.IsPublished);

            if (faq == null)
                return NotFound("FAQ not found");

            // Increment view count
            faq.ViewCount++;
            await context.SaveChangesAsync();

            return Ok(faq);
        }
        catch (Exception ex)
        {
            Log.Error("Failed to get FAQ", ex);
            return BadRequest("Failed to get FAQ");
        }
    }

    // Get FAQs by module code (Public)
    [HttpGet("modules/{moduleCode}/faqs")]
    public async Task<IActionResult> GetFAQsByModule([FromRoute] string moduleCode)
    {
        try
        {
            var faqs = await context.FAQs
                .Where(f => f.ModuleCode == moduleCode && f.IsPublished)
                .OrderByDescending(f => f.ViewCount)
                .ToListAsync();

            return Ok(new
            {
                ModuleCode = moduleCode,
                TotalFAQs = faqs.Count,
                FAQs = faqs
            });
        }
        catch (Exception ex)
        {
            Log.Error("Failed to get module FAQs", ex);
            return BadRequest("Failed to get module FAQs");
        }
    }

    // Get FAQs by tutor ID (Public - see which FAQs a tutor created)
    [HttpGet("tutors/{tutorId}/faqs")]
    public async Task<IActionResult> GetFAQsByTutor([FromRoute] int tutorId)
    {
        try
        {
            var faqs = await context.FAQs
                .Where(f => f.TutorID == tutorId && f.IsPublished)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();

            return Ok(new
            {
                TutorId = tutorId,
                TotalFAQs = faqs.Count,
                FAQs = faqs
            });
        }
        catch (Exception ex)
        {
            Log.Error("Failed to get tutor FAQs", ex);
            return BadRequest("Failed to get tutor FAQs");
        }
    }

    // Update FAQ (Tutors only - can update their own FAQs)
    [HttpPut("faqs/{faqId}/tutors/{tutorId}")]
    public async Task<IActionResult> UpdateFAQ([FromRoute] int faqId,[FromRoute] int tutorId,[FromBody] UpdateFAQRequest request)
    {
        try
        {
            var faq = await context.FAQs
                .FirstOrDefaultAsync(f => f.FAQID == faqId && f.TutorID == tutorId);

            if (faq == null)
                return BadRequest("FAQ not found or not owned by this tutor");

            faq.FrequentlyAskedQuestion = request.Question ?? faq.FrequentlyAskedQuestion;
            faq.Answer = request.Answer ?? faq.Answer;
            faq.ModuleCode = request.ModuleCode ?? faq.ModuleCode;
            faq.IsPublished = request.IsPublished;
            faq.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();

            return Ok(faq);
        }
        catch (Exception ex)
        {
            Log.Error("Failed to update FAQ", ex);
            return BadRequest("Failed to update FAQ");
        }
    }

    // Delete FAQ (Tutors only - can delete their own FAQs)
    [HttpDelete("faqs/{faqId}/tutors/{tutorId}")]
    public async Task<IActionResult> DeleteFAQ([FromRoute] int faqId, [FromRoute] int tutorId)
    {
        try
        {
            var faq = await context.FAQs
                .FirstOrDefaultAsync(f => f.FAQID == faqId && f.TutorID == tutorId);

            if (faq == null)
                return BadRequest("FAQ not found or not owned by this tutor");

            context.FAQs.Remove(faq);
            await context.SaveChangesAsync();

            return Ok(new { Message = "FAQ deleted successfully" });
        }
        catch (Exception ex)
        {
            Log.Error("Failed to delete FAQ", ex);
            return BadRequest("Failed to delete FAQ");
        }
    }

    // Get all FAQs including unpublished (Tutors only - for management)
    [HttpGet("tutors/{tutorId}/faqs/all")]
    public async Task<IActionResult> GetAllTutorFAQs([FromRoute] int tutorId)
    {
        try
        {
            var faqs = await context.FAQs
                .Where(f => f.TutorID == tutorId)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();

            return Ok(new
            {
                TutorId = tutorId,
                TotalFAQs = faqs.Count,
                PublishedFAQs = faqs.Count(f => f.IsPublished),
                DraftFAQs = faqs.Count(f => !f.IsPublished),
                FAQs = faqs
            });
        }
        catch (Exception ex)
        {
            Log.Error("Failed to get tutor all FAQs", ex);
            return BadRequest("Failed to get tutor all FAQs");
        }
    }
}
