using System;
using System.IO;
using System.Threading.Tasks;
using log4net;
using MerendaIFCE.Sync.Services;
using Microsoft.Extensions.Configuration;

namespace MerendaIFCE.Sync
{
    public class App
    {
        private readonly ILog log = LogManager.GetLogger(typeof(App));
        private App() { }

        public static App Current { get; private set; } = new App();

        public DateTimeOffset Today => new DateTimeOffset(DateTimeOffset.Now.Date);

        public IConfigurationRoot Settings { get; private set; }

        public async Task InitializaAsync()
        {
            log.Info("Inicializando.");

            try
            {
                Configura();
                Mapeamentos.Configura();
                await BancoDeDados.AtualizaAsync();
                await Sincronizador.InicializaAsync();
                await Agendador.InicializaAsync();
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
                throw;
            }
        }

        public async Task IniciaAsync()
        {
            await Agendador.Instance.IniciaAsync();
        }

        public async Task ParaAsync()
        {
            await Agendador.Instance.ParaAsync();
        }

        private void Configura()
        {
            log.Debug("Lendo configurações.");

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true)
                .AddUserSecrets<App>();

            Settings = builder.Build();
        }
    }
}
