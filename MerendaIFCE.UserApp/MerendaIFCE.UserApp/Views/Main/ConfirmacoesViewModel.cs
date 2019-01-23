using MerendaIFCE.UserApp.Models;
using MerendaIFCE.UserApp.Observable;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using MerendaIFCE.UserApp.Services;
using MerendaIFCE.UserApp.Exceptions;
using Xamarin.Forms;

namespace MerendaIFCE.UserApp.Views.Main
{
    public class ConfirmacoesViewModel : BusyObject
    {
        private bool isRefreshing;
        private Confirmacao hoje;
        private IEnumerable<Confirmacao> _confirmacoes;
        private IList<Confirmacao> confirmacoes;

        public Confirmacao Hoje
        {
            get { return hoje; }
            set { hoje = value; OnPropertyChanged(); }
        }


        public bool IsRefreshing
        {
            get { return isRefreshing; }
            set { isRefreshing = value; OnPropertyChanged(); }
        }


        public Command RefreshCommand => new Command(async () =>
        {
            try
            {
                IsRefreshing = true;
                await LoadAsync();
            }
            finally
            {
                IsRefreshing = false;
            }
        });

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

        public async Task UpdateCancelamento()
        {
            try
            {
                var ws = new WebService();
                Hoje = await ws.PutConfirmacaoAsync(Hoje);
                AppDbContext.Instance.UpdteConfirmacao(Hoje);
            }
            catch (ServerException)
            {
                Hoje.Cancela = !Hoje.Cancela;
                OnPropertyChanged(nameof(Hoje));
            }
        }

        private void LoadLocal()
        {
            _confirmacoes = AppDbContext.Instance.GetConfirmacoes();
            Hoje = _confirmacoes.SingleOrDefault(c => c.Dia.Date == DateTime.Today);
            Confirmacoes = _confirmacoes.Except(new[] { Hoje }).ToList();
        }
    }
}
