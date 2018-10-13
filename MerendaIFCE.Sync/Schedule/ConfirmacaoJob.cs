using FluentScheduler;
using MerendaIFCE.Sync.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MerendaIFCE.Sync.Services;
using System.Threading.Tasks;

namespace MerendaIFCE.Sync.Schedule
{
    public class ConfirmacaoJob : IJob
    {
        public void Execute()
        {
            ExecuteAsync().Wait();
        }

        private async Task ExecuteAsync()
        {
            using (var db = new LocalDbContext())
            {
                var today = App.Current.Today;
                var ws = new SyncWebService();
                var cws = new ConfirmacaoWebService();

                var listaSync = new List<Confirmacao>();
                var dias = db.InscricaoDias.Include(d => d.Confirmacoes).Where(d => d.Dia == today.DayOfWeek);

                foreach (var dia in dias)
                {
                    var confirmacao = dia.Confirmacoes.FirstOrDefault(d => d.Dia == today);
                    if (confirmacao?.StatusConfirmacao != StatusConfirmacao.Confirmado)
                    {
                        if (confirmacao == null)
                        {
                            confirmacao = new Confirmacao { Dia = today };
                            dia.Confirmacoes.Add(confirmacao);
                        }
                        try
                        {
                            cws.Confirma(confirmacao);
                        }
                        catch (ApplicationException ex)
                        {
                            Console.WriteLine(ex.Message);
                            confirmacao.StatusConfirmacao = StatusConfirmacao.Erro;
                        }
                    }

                    if (confirmacao.StatusSincronia != StatusSincronia.Sincronizado)
                    {
                        listaSync.Add(confirmacao);
                    }
                }

                try
                {
                    await ws.PostConfirmacoesAsync(listaSync);
                }
                catch (ApplicationException ex)
                {
                    Console.WriteLine(ex.Message);
                    foreach (var item in listaSync)
                    {
                        item.StatusSincronia = StatusSincronia.Erro;
                    }
                }

                db.SaveChanges();
            }
        }
    }
}
