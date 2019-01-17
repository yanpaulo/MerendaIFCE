using System;
using System.Threading.Tasks;

using log4net.Config;

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

            while (true)
            {
                Console.ReadKey();
            }
        }
    }
}
