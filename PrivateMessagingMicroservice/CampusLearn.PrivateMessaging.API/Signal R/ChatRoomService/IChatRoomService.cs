using CampusLearn.Code.Library.PrivateMessageModels;
using CampusLearn.Code.Library.RabbitMQ.TopicRabbitMQ;

namespace CampusLearn.PrivateMessaging.API.Signal_R.ChatRoomService;

public interface IChatRoomService
{
    Task<ChatRooms> GetOrCreateChatRoomAsync(NewTopicMessage message);
    Task<bool> ChatRoomExistsAsync(int studentId, int tutorId, int queryId);
    Task<ChatRooms?> GetChatRoomAsync(int roomId);
    Task AddMessageAsync(int roomId, int senderId, string content);
}
