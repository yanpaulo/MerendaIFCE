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
        private Confirmacao hoje;
        private IEnumerable<Confirmacao> _confirmacoes;
        private IList<Confirmacao> confirmacoes;
        
        public Confirmacao Hoje
        {
            get { return hoje; }
            set { hoje = value; OnPropertyChanged(); }
        }

        public IList<Confirmacao> Confirmacoes
        {
            get { return confirmacoes; }
            set { confirmacoes = value; OnPropertyChanged(nameof(Confirmacoes)); }
        }

        public async Task LoadAsync()
        {
            LoadLocal();

            using (BusyState())
            {
                var ws = new WebService();
                var ultima = _confirmacoes.FirstOrDefault()?.UltimaModificacao;
                var novas = await ws.GetConfirmacoesAsync(ultima);
                foreach (var confirmacao in novas)
                {
                    if (_confirmacoes.SingleOrDefault(c => c.Id == confirmacao.Id) is Confirmacao local)
                    {
                        AppDbContext.Instance.UpdteConfirmacao(confirmacao);

                    }
                    else
                    {
                        AppDbContext.Instance.AddConfirmacao(confirmacao);
                    }
                }
            }

            LoadLocal();
        }

        private void LoadLocal()
        {
            _confirmacoes = AppDbContext.Instance.GetConfirmacoes();
            Hoje = _confirmacoes.SingleOrDefault(c => c.Dia.Date == DateTime.Today);
            Confirmacoes = _confirmacoes.Except(new[] { Hoje }).ToList();
        }
    }
}
