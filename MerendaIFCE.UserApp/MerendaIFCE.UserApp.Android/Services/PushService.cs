using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MerendaIFCE.UserApp.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(MerendaIFCE.UserApp.Droid.Services.PushService))]
namespace MerendaIFCE.UserApp.Droid.Services
{
    public class PushService : IPushService
    {
        public const string PnsKey = "ANDROID_PNS_KEY";

        public Task<CanalPush> GetCanal()
        {
            return Task.FromResult(new CanalPush
            {
                Plataforma = PlataformaPush.GCM,
                Handle = App.Current.Properties[PnsKey] as string
            });
        }
    }
}