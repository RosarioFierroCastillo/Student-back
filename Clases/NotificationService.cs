using API_Archivo.Clases;
using Microsoft.AspNetCore.SignalR;

public class NotificationService
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationService(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task EnviarNotificacion(string mensaje)
    {
        await _hubContext.Clients.All.SendAsync("ReceiveNotification", mensaje);
    }
}