using CampusLearn.PrivateMessaging.API.RabbitMQ;
using System;

namespace CampusLearn.PrivateMessaging.API.Controllers;

[Route("[controller]")]
[ApiController]
public class MessageController : ControllerBase
{

    //Returns all messages currently in the in-memory queue(safe copy).
    [HttpGet]
    public IActionResult GetMessages()
    {
        // Convert queue to list for API response
        var messages = InternalQueue.Messages.ToList();
        return Ok(messages);
    }


    //Pops one message off the queue(FIFO) for processing.
    [HttpGet("next")]
    public IActionResult GetNextMessage()
    {
        if (InternalQueue.Messages.TryDequeue(out var message))
        {
            return Ok(message);
        }
        return NotFound("No messages available");
    }
}


