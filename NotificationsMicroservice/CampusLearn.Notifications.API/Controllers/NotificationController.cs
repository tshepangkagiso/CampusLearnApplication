using CampusLearn.Notifications.API.RabbitMQ;
using CampusLearn.Notifications.API.RabbitMQ.Forum_MessageQ;

namespace CampusLearn.Notification.API.Controllers;

[Route("[controller]")]
[ApiController]
public class NotificationController : ControllerBase
{

    //Returns all messages currently in the in-memory queue(safe copy).
    [HttpGet("topic")]
    public IActionResult GetMessagesTopic()
    {
        // Convert queue to list for API response
        var messages = InternalTopicQueue.Messages.ToList();
        return Ok(messages);
    }


    //Pops one message off the queue(FIFO) for processing.
    [HttpGet("next-topic")]
    public IActionResult GetNextMessageTopic()
    {
        if (InternalTopicQueue.Messages.TryDequeue(out var message))
        {
            return Ok(message);
        }
        return NotFound("No messages available");
    }


    //Returns all messages currently in the in-memory queue(safe copy).
    [HttpGet("forum")]
    public IActionResult GetMessagesForum()
    {
        // Convert queue to list for API response
        var messages = InternalForumQueue.Messages.ToList();
        return Ok(messages);
    }


    //Pops one message off the queue(FIFO) for processing.
    [HttpGet("next-forum")]
    public IActionResult GetNextMessageForum()
    {
        if (InternalForumQueue.Messages.TryDequeue(out var message))
        {
            return Ok(message);
        }
        return NotFound("No messages available");
    }
}
