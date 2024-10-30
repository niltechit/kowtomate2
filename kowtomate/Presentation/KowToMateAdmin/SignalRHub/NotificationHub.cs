using CutOutWiz.Core.Utilities;
using CutOutWiz.Services.Models.Security;
using CutOutWiz.Services.DbAccess;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Security.Claims;

namespace KowToMateAdmin.SignalRHub
{
    public class NotificationHub : Hub
    {
        //https://learn.microsoft.com/en-us/aspnet/core/blazor/tutorials/signalr-blazor?view=aspnetcore-6.0&tabs=visual-studio&pivots=server
        public static ConcurrentDictionary<string, string> UserConnections = new ConcurrentDictionary<string, string>();
        //private readonly IWorkContext _workContext;
        private readonly AuthenticationStateProvider _auth;
        
        public NotificationHub(AuthenticationStateProvider auth)
        {
            _auth = auth;
        }

        // ChatHub.cs
        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task JoinIndivisualGroup(string userId)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                UserConnections.AddOrUpdate(userId, Context.ConnectionId, (_, connectionId) => connectionId);
            }

            await base.OnConnectedAsync();
        }

        //public override async Task OnConnectedAsync()
        //{
            //// Get the user ID from the claims (assuming you are using authentication)
            //var authState = await _auth.GetAuthenticationStateAsync();
            //var userClaims = authState.User.Claims;

            //var userObjectId = userClaims.FirstOrDefault(f => f.Type == ClaimTypes.NameIdentifier).Value;


            //if (!string.IsNullOrEmpty(userObjectId))
            //{
            //    UserConnections.AddOrUpdate(userObjectId, Context.ConnectionId, (_, connectionId) => connectionId);
            //}

            //await base.OnConnectedAsync();
        //}

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            // Get the user ID from the claims (assuming you are using authentication)
            var userId = Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (!string.IsNullOrEmpty(userId))
            {
                UserConnections.TryRemove(userId, out _);
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendNotificationToUser(string userId, string message)
        {
            if (UserConnections.TryGetValue(userId, out var connectionId))
            {
                await Clients.Client(connectionId).SendAsync("ReceiveNotification", message);
            }
            //await Clients.User(userId).SendAsync("ReceiveNotification", message);
        }

        public async Task SendMessageToGroup(string groupName, string message)
        {
            // Use the 'groupName' parameter to determine the recipient group
            // ... Your logic here ...

            await Clients.Group(groupName).SendAsync("ReceiveNotification", message);
        }

        // One-to-All Messaging
        public async Task SendMessageToAll(string message)
        {
            await Clients.All.SendAsync("ReceiveNotification", message);
        }
    }
}
