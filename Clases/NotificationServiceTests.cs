using API_Archivo.Clases;
using Microsoft.AspNetCore.SignalR;
using Moq;
using Org.BouncyCastle.Utilities;
using Xunit;

public class NotificationServiceTests
{
    [Fact]
    public async Task EnviarNotificacion_ShouldSendToAllClients()
    {
        var mockHubContext = new Mock<IHubContext<NotificationHub>>();
        var mockClients = new Mock<IHubClients>();
        var mockClientProxy = new Mock<IClientProxy>();
        mockHubContext.Setup(ctx => ctx.Clients).Returns(mockClients.Object);
        mockClients.Setup(clients => clients.All).Returns(mockClientProxy.Object);

        var service = new NotificationService(mockHubContext.Object);
        string testMessage = "Test Notification";

        await service.EnviarNotificacion(testMessage);

        mockClientProxy.Verify(
            proxy => proxy.SendCoreAsync(
                "ReceiveNotification",
                It.Is<object[]>(o => (string)o[0] == testMessage),
                default
            ),
            Moq.Times.Once
        );
    }
}
