using MerendaIFCE.UserApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.PushNotifications;
using Xamarin.Forms;

[assembly: Dependency(typeof(MerendaIFCE.UserApp.UWP.Services.PushService))]
namespace MerendaIFCE.UserApp.UWP.Services
{
    public class PushService : IPushService
    {
        public async Task<CanalPush> GetCanal()
        {
            var channel = await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();

            return new CanalPush
            {
                Plataforma = PlataformaPush.WNS,
                Handle = channel.Uri
            };
        }
    }
}
