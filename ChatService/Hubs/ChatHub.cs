using ChatService.DataService;
using ChatService.Models;
using Microsoft.AspNet.SignalR.Infrastructure;
using Microsoft.AspNetCore.SignalR;

namespace ChatService.Hubs
{
    public class ChatHub: Hub
    {
        private readonly SharedDB _shared;
        public ChatHub(SharedDB shared) => _shared = shared;
        public async Task JoinChat(UserConnection connection)
        {
            await Clients.All.SendAsync("ReceivedMessage", "admin", $"{connection.Username} has joined");
        }
        public async Task JoinSpecificChatRoom(UserConnection connection)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, connection.ChatRoom);
            _shared.connections[Context.ConnectionId] = connection; 
            await Clients.Group(connection.ChatRoom).SendAsync("JoinSpecificChatRoom", $"{connection.Username}", $"has joined {connection.ChatRoom}");
        }
        public async Task SendMessage(String msg)
        {
            if (_shared.connections.TryGetValue(Context.ConnectionId,out UserConnection connection))
            {
                await Clients.Group(connection.ChatRoom) //Client Proxy
                    .SendAsync("ReceiveSpecificMessage", connection.Username, msg); //Task
            }
        }
    }
}
