namespace CampusLearn.Notifications.API.Controllers;

[Route("[controller]")]
[ApiController]
public class NotificationsController : ControllerBase
{
    private readonly IMessageStoreForum messageStoreForum;
    private readonly IMessageStoreTopic messageStoreTopic;

    public NotificationsController(IMessageStoreForum messageStoreForum, IMessageStoreTopic messageStoreTopic)
    {
        this.messageStoreForum = messageStoreForum;
        this.messageStoreTopic = messageStoreTopic;
    }

    [HttpGet]
    public IActionResult GetResponse()
    {
        try
        {
            return Ok("Hello, This is the Notifications API.");
        }
        catch (Exception ex)
        {
            return BadRequest($"Error: {ex.Message.ToString()}");
        }
    }

    [HttpGet("topic")]
    public async Task<IActionResult> GetTopicMessages()
    {
        try
        {
            var topicMessages = await this.messageStoreTopic.GetTopicMessagesAsync(); ;
            return Ok(topicMessages);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("forum")]
    public async Task<IActionResult> GetForumMessages()
    {
        try
        {
            var topicMessages = await this.messageStoreForum.GetForumMessagesAsync();
            return Ok(topicMessages);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }
}
