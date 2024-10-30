using KowToMateAdmin.SignalRHub;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Radzen;
using System.Threading.Tasks;

namespace KowToMateAdmin.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MyNotificationController : ControllerBase
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public MyNotificationController(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        [HttpPost]
        public async Task<IActionResult> SendNotification(int messageType, string userObjectId, string message)
        {
            // Your logic to determine the user ID based on the payload data
            //string userId = "user_id_from_payload";

            // Your logic to get the notification message

            // Call the SendNotificationToUser method in the hub
            if (messageType == 1) //Indivisual
            {
                //Access the UserConnections dictionary using the NotificationHub instance
                //var notificationHub1 = new NotificationHub(); // Create a new instance (or inject if possible)
                if (NotificationHub.UserConnections.TryGetValue(userObjectId, out var connectionId))
                {
                    //await Clients.Client(connectionId).SendAsync("ReceiveNotification", message);
                    await _hubContext.Clients.Client(connectionId).SendAsync("ReceiveNotification", userObjectId, message);
                }
            }
            else if (messageType == 2)
            {
                await _hubContext.Clients.User(userObjectId).SendAsync("SendNotificationToUser", userObjectId, message);
            }

            return Ok();
        }
    }

}
