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
    public class ConfirmacaoJob
    {
        public void Execute()
        {
            ExecuteAsync().Wait();
        }

        public async Task ExecuteAsync()
        {
            using (var db = new LocalDbContext())
            {
                var today = App.Current.Today;
                var ws = new SyncWebService();
                var cws = new ConfirmacaoWebService();

                var listaSync = new List<Confirmacao>();
                var dias = db.InscricaoDias.Include(d => d.Inscricao).ThenInclude(i => i.Confirmacoes).Where(d => d.Dia == today.DayOfWeek);

                foreach (var dia in dias)
                {
                    var confirmacao = dia.Inscricao.Confirmacoes.FirstOrDefault(d => d.Dia == today);
                    if (confirmacao?.StatusConfirmacao != StatusConfirmacao.Confirmado)
                    {
                        if (confirmacao == null)
                        {
                            confirmacao = new Confirmacao
                            {
                                Dia = today,
                                InscricaoId = dia.InscricaoId
                            };
                        }
                        try
                        {
                            cws.Confirma(confirmacao);
                            confirmacao.StatusConfirmacao = StatusConfirmacao.Confirmado;
                            confirmacao.StatusSincronia = StatusSincronia.Modificado;
                        }
                        catch (ApplicationException ex)
                        {
                            Console.WriteLine(ex.Message);
                            confirmacao.StatusConfirmacao = StatusConfirmacao.Erro;
                        }
                    }

                    if (confirmacao.StatusSincronia != StatusSincronia.Sincronizado)
                    {
                        confirmacao.StatusSincronia = StatusSincronia.Sincronizado;
                        listaSync.Add(confirmacao);
                    }
                }

                try
                {
                    var result = await ws.PostConfirmacoesAsync(listaSync);
                    for (int i = 0; i < result.Count; i++)
                    {
                        listaSync[i].IdRemoto = result[i].Id;
                    }

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
