using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MerendaIFCE.UserApp.Services
{
    public interface IPushService
    {
        Task<CanalPush> GetCanal();   
    }

    public class CanalPush
    {
        public PlataformaPush Plataforma { get; set; }

        public string Handle { get; set; }
    }

    public enum PlataformaPush
    {
        WNS = 1, GCM, APNS
    }
}
