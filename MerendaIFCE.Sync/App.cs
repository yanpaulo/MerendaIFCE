using System;
using System.Threading.Tasks;
using MerendaIFCE.Sync.Services;

namespace MerendaIFCE.Sync
{
    public class App
    {
        public static App Current { get; private set; } = new App();
        private App() { }

        public DateTimeOffset Now => DateTimeOffset.Now;

        public async Task InitAsync()
        {
            await BancoDeDados.AtualizaAsync();
            Sincronizador.Inicializa();
            Agendamento.Inicializa();
        }
        
    }
}
