using Microsoft.Azure.NotificationHubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MerendaIFCE.WebApp.Services
{
    public class NotificationService
    {
        public NotificationHubClient Hub { get; private set; }

        public NotificationService()
        {
            Hub = NotificationHubClient.CreateClientFromConnectionString(
                @"Endpoint=sb://almocoifce.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=Kp8SOHS12E3eSFQswWE4O8aSkPdlWIMK7VJx43H37Dk=",
                "AlmocoIFCE.NotificationHub");
        }

        public async Task<NotificationOutcome> NotificaStatusRefeicaoAsync(string matricula) =>
            await Hub.SendTemplateNotificationAsync(new Dictionary<string, string> { { "message", "O status da sua refeição foi alterado." } }, $"matricula:{matricula}");

    }
}
