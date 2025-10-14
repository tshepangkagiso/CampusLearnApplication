using CampusLearn.PrivateMessaging.API.Database;
using CampusLearn.PrivateMessaging.API.RabbitMQ;
using Microsoft.EntityFrameworkCore;
using System;

namespace CampusLearn.PrivateMessaging.API.Controllers;

[Route("[controller]")]
[ApiController]
public class MessageController(PrivateMessagesDbContext context) : ControllerBase
{

    // Get all chat rooms with their messages
    [HttpGet("chatrooms")]
    public async Task<IActionResult> GetAllChatRooms()
    {
        var chatRooms = await context.ChatRooms
            .Include(cr => cr.Messages)
            .OrderByDescending(cr => cr.CreatedAt)
            .ToListAsync();

        return Ok(chatRooms);
    }

    // Get chat room by RoomId with messages
    [HttpGet("chatrooms/{roomId}")]
    public async Task<IActionResult> GetChatRoomById(int roomId)
    {
        var chatRoom = await context.ChatRooms
            .Include(cr => cr.Messages)
            .FirstOrDefaultAsync(cr => cr.RoomId == roomId);

        if (chatRoom == null)
        {
            return NotFound($"Chat room with ID {roomId} not found");
        }

        return Ok(chatRoom);
    }

    // Get all chat rooms for a specific student
    [HttpGet("chatrooms/student/{studentId}")]
    public async Task<IActionResult> GetChatRoomsByStudentId(int studentId)
    {
        var chatRooms = await context.ChatRooms
            .Include(cr => cr.Messages)
            .Where(cr => cr.StudentId == studentId)
            .OrderByDescending(cr => cr.CreatedAt)
            .ToListAsync();

        return Ok(chatRooms);
    }

    // Get all chat rooms for a specific tutor
    [HttpGet("chatrooms/tutor/{tutorId}")]
    public async Task<IActionResult> GetChatRoomsByTutorId(int tutorId)
    {
        var chatRooms = await context.ChatRooms
            .Include(cr => cr.Messages)
            .Where(cr => cr.TutorId == tutorId)
            .OrderByDescending(cr => cr.CreatedAt)
            .ToListAsync();

        return Ok(chatRooms);
    }

    // Get specific chat room by studentId, tutorId, and queryId
    [HttpGet("chatrooms/student/{studentId}/tutor/{tutorId}/query/{queryId}")]
    public async Task<IActionResult> GetChatRoomByStudentTutorQuery(int studentId, int tutorId, int queryId)
    {
        var chatRoom = await context.ChatRooms
            .Include(cr => cr.Messages)
            .FirstOrDefaultAsync(cr => cr.StudentId == studentId
                                    && cr.TutorId == tutorId
                                    && cr.QueryId == queryId);

        if (chatRoom == null)
        {
            return NotFound($"Chat room not found for student {studentId}, tutor {tutorId}, query {queryId}");
        }

        return Ok(chatRoom);
    }

    // Get messages for a specific chat room
    [HttpGet("chatrooms/{roomId}/messages")]
    public async Task<IActionResult> GetMessagesByChatRoom(int roomId)
    {
        var messages = await context.Messages
            .Where(m => m.RoomId == roomId)
            .OrderBy(m => m.Timestamp)
            .ToListAsync();

        if (!messages.Any())
        {
            return NotFound($"No messages found for chat room {roomId}");
        }

        return Ok(messages);
    }

    // Get active chat rooms only
    [HttpGet("chatrooms/active")]
    public async Task<IActionResult> GetActiveChatRooms()
    {
        var activeChatRooms = await context.ChatRooms
            .Include(cr => cr.Messages)
            .Where(cr => cr.IsActive)
            .OrderByDescending(cr => cr.CreatedAt)
            .ToListAsync();

        return Ok(activeChatRooms);
    }

    // Get chat room count statistics
    [HttpGet("chatrooms/stats")]
    public async Task<IActionResult> GetChatRoomStats()
    {
        var totalRooms = await context.ChatRooms.CountAsync();
        var activeRooms = await context.ChatRooms.CountAsync(cr => cr.IsActive);
        var totalMessages = await context.Messages.CountAsync();

        var stats = new
        {
            TotalChatRooms = totalRooms,
            ActiveChatRooms = activeRooms,
            InactiveChatRooms = totalRooms - activeRooms,
            TotalMessages = totalMessages,
            AverageMessagesPerRoom = totalRooms > 0 ? (double)totalMessages / totalRooms : 0
        };

        return Ok(stats);
    }


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


