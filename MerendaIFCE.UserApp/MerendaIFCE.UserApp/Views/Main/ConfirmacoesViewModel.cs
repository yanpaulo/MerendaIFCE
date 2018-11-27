using MerendaIFCE.UserApp.Models;
using MerendaIFCE.UserApp.Observable;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using MerendaIFCE.UserApp.Services;

namespace MerendaIFCE.UserApp.Views.Main
{
    public class ConfirmacoesViewModel : BusyObject
    {
        private ObservableCollection<Confirmacao> confirmacoes;

        public ObservableCollection<Confirmacao> Confirmacoes
        {
            get { return confirmacoes; }
            set { confirmacoes = value; OnPropertyChanged(nameof(Confirmacoes)); }
        }


        public async Task LoadAsync()
        {
            Confirmacoes = new ObservableCollection<Confirmacao>(AppDbContext.Instance.GetConfirmacoes());
            var ws = new WebService();
            var ultima = Confirmacoes.FirstOrDefault()?.UltimaModificacao;

            using (BusyState())
            {
                var novas = await ws.GetConfirmacoesAsync(ultima);
                foreach (var confirmacao in novas)
                {
                    Confirmacoes.Add(confirmacao);
                }
                AppDbContext.Instance.AddConfirmacoes(novas); 
            }

        }
    }
}
