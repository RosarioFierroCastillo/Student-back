using Microsoft.AspNetCore.SignalR;
using MySqlX.XDevAPI;

namespace API_Archivo.Clases
{
    public class NotificationHub : Hub
    {
        public async Task SendNotification(string message)
        {
            await Clients.All.SendAsync("ReceiveNotification", message);
        }

        private async Task NotifyClients(string message)
        {
            await Clients.All.SendAsync("ReceiveNotification", message);
        }

        public async Task TriggerNotification(string message)
        {
            await NotifyClients(message);
        }
    }
}
