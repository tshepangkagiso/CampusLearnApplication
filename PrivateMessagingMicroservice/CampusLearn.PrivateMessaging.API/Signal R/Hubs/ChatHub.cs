using CampusLearn.PrivateMessaging.API.Signal_R.ChatRoomService;
using Microsoft.AspNetCore.SignalR;


namespace CampusLearn.PrivateMessaging.API.Signal_R.Hubs;

public class ChatHub(IChatRoomService chatRoomService) : Hub
{

    public async Task JoinRoom(int roomId)
    {
        // Verify room exists and user has access
        var room = await chatRoomService.GetChatRoomAsync(roomId);
        if (room != null)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"room_{roomId}");
            await Clients.Caller.SendAsync("JoinedRoom", roomId);
        }
    }

    public async Task SendMessage(int roomId, int senderId, string content)
    {
        // Save message to database
        await chatRoomService.AddMessageAsync(roomId, senderId, content);

        // Broadcast to all clients in the room
        await Clients.Group($"room_{roomId}")
            .SendAsync("ReceiveMessage", senderId, content, DateTime.UtcNow);
    }

    public async Task LeaveRoom(int roomId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"room_{roomId}");
    }
}
