using System;
using System.Threading.Tasks;
using MerendaIFCE.Sync.Configurations;
using MerendaIFCE.Sync.Services;

namespace MerendaIFCE.Sync
{
    public class App
    {
        public static App Current { get; private set; } = new App();
        private App() { }

        public DateTimeOffset Today => new DateTimeOffset(DateTimeOffset.Now.Date);

        public async Task InitAsync()
        {
            MappingConfigurations.Configure();
            await BancoDeDados.AtualizaAsync();
            Sincronizador.Inicializa();
            Agendamento.Inicializa();
        }
        
    }
}
