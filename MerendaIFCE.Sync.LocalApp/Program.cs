using log4net.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MerendaIFCE.Sync.LocalApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            BasicConfigurator.Configure();

            var app = App.Current;
            await app.InitializaAsync();
            await app.IniciaAsync();

            await Task.Delay(TimeSpan.FromMinutes(5));
        }
    }
}
