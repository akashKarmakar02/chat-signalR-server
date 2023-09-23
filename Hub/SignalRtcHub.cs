using System.Text.Json;
using Microsoft.AspNetCore.SignalR;

public class SignalRtcHub : Hub
{
    private readonly Dictionary<string, List<string>> _groupChatConnections = new Dictionary<string, List<string>>();

    public async Task NewUser(string userName, string groupName)
    {
        var userInfo = new UserInfo() { userName = userName, connetionId = Context.ConnectionId };
        await Clients.Others.SendAsync("NewUserArrived", JsonSerializer.Serialize(userInfo));


        if (!_groupChatConnections.ContainsKey(groupName))
        {
            _groupChatConnections[groupName] = new List<string>();
        }
        _groupChatConnections[groupName].Add(Context.ConnectionId);

        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
    }

    public async Task HelloUser(string userName, string user)
    {
        var userInfo = new UserInfo() { userName = userName, connetionId = Context.ConnectionId };
        await Clients.Client(user).SendAsync("UserSaidHello", JsonSerializer.Serialize(userInfo));
    }

    public async Task SendMessageToGroup(string userName, string groupName, string message)
    {
        var mes = new Message() { userName = userName, message = message };
        await Clients.Group(groupName).SendAsync("NewMessageArrived", JsonSerializer.Serialize(mes));
    }

    public async Task LeaveGroup(string groupName)
    {
        if (_groupChatConnections.ContainsKey(groupName))
        {
            _groupChatConnections[groupName].Remove(Context.ConnectionId);
            if (_groupChatConnections[groupName].Count == 0)
            {
                _groupChatConnections.Remove(groupName);
            }
        }
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
    }

    public async Task SendSignal(string signal, string user)
    {
        await Clients.Client(user).SendAsync("SendSignal", Context.ConnectionId, signal);
    }
}