using System;
using System.IO;
using System.Threading.Tasks;
using MerendaIFCE.Sync.Services;
using Microsoft.Extensions.Configuration;

namespace MerendaIFCE.Sync
{
    public class App
    {
        public static App Current { get; private set; } = new App();
        private App() { }

        public DateTimeOffset Today => new DateTimeOffset(DateTimeOffset.Now.Date);

        public IConfigurationRoot Settings { get; private set; }

        public async Task InitAsync()
        {
            throw new NotImplementedException();
            Configura();
            Mapeamentos.Configura();
            await BancoDeDados.AtualizaAsync();
            await Sincronizador.InicializaAsync();
            await new TarefaConfirmacao().ExecuteAsync();
        }

        private void Configura()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true)
                .AddUserSecrets<App>();

            Settings = builder.Build();
        }
    }
}
