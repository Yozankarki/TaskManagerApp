using Microsoft.AspNetCore.SignalR;

namespace TaskManager.Web.Hubs
{
    public class NotificationHub: Hub
    {
        public async Task SendNotification(string userId, string message)
        {
            await Clients.All.SendAsync("ReceiveNotification", message);
        }
    }
}
