using CampusLearn.Code.Library.PrivateMessageModels;
using CampusLearn.Code.Library.RabbitMQ.TopicRabbitMQ;
using CampusLearn.PrivateMessaging.API.Database;
using Microsoft.EntityFrameworkCore;

namespace CampusLearn.PrivateMessaging.API.Signal_R.ChatRoomService;

public class ChatRoomService(PrivateMessagesDbContext context) : IChatRoomService
{


    public async Task<ChatRooms> GetOrCreateChatRoomAsync(NewTopicMessage message)
    {
        // Check if room already exists
        var existingRoom = await context.ChatRooms
            .FirstOrDefaultAsync(cr => cr.StudentId == message.StudentId
                                    && cr.TutorId == message.TutorId
                                    && cr.QueryId == message.QueryId);

        if (existingRoom != null)
        {
            return existingRoom;
        }

        // Create new room
        var newRoom = new ChatRooms
        {
            StudentId = message.StudentId,
            TutorId = message.TutorId,
            QueryId = message.QueryId,
            Title = message.Title,
            ModuleCode = message.ModuleCode,
            CreatedAt = message.CreatedAt,
            IsActive = true
        };

        context.ChatRooms.Add(newRoom);
        await context.SaveChangesAsync();

        return newRoom;
    }

    public async Task<bool> ChatRoomExistsAsync(int studentId, int tutorId, int queryId)
    {
        return await context.ChatRooms
            .AnyAsync(cr => cr.StudentId == studentId
                         && cr.TutorId == tutorId
                         && cr.QueryId == queryId);
    }

    public async Task<ChatRooms?> GetChatRoomAsync(int roomId)
    {
        return await context.ChatRooms
            .Include(cr => cr.Messages)
            .FirstOrDefaultAsync(cr => cr.RoomId == roomId);
    }

    public async Task AddMessageAsync(int roomId, int senderId, string content)
    {
        var message = new Messages
        {
            RoomId = roomId,
            SenderId = senderId,
            Content = content,
            Timestamp = DateTime.UtcNow
        };

        context.Messages.Add(message);
        await context.SaveChangesAsync();
    }
}
