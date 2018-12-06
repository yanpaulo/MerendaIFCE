using MerendaIFCE.Sync.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MerendaIFCE.Sync.Services;
using System.Threading.Tasks;
using MerendaIFCE.Sync.Services.Confirmador;

namespace MerendaIFCE.Sync.Services
{
    public class Tarefas
    {
        public async Task CriaConfirmacoesAsync()
        {
            using (var db = new LocalDbContext())
            {
                var today = App.Current.Today;

                var listaSync = new List<Confirmacao>();
                var dias = db.InscricaoDias.Include(d => d.Inscricao).ThenInclude(i => i.Confirmacoes).Where(d => d.Dia == today.DayOfWeek).ToList();

                foreach (var dia in dias)
                {
                    var confirmacao = dia.Inscricao.Confirmacoes.FirstOrDefault(d => d.Dia == today);

                    if (confirmacao == null)
                    {
                        confirmacao = new Confirmacao
                        {
                            Dia = today,
                            InscricaoId = dia.InscricaoId,
                            Inscricao = dia.Inscricao,
                            StatusConfirmacao = StatusConfirmacao.NaoConfirmado
                        };
                    }

                    if (confirmacao.StatusSincronia != StatusSincronia.Sincronizado)
                    {
                        listaSync.Add(confirmacao);
                    }

                    confirmacao.StatusSincronia = StatusSincronia.Sincronizado;
                }

                await PostConfirmacoesAsync(listaSync);
                db.SaveChanges();
            }
        }

        public async Task ExecutaConfirmacoesAsync()
        {
            using (var db = new LocalDbContext())
            {
                var today = App.Current.Today;
                var ws = new SyncWebService();

                var listaSync = new List<Confirmacao>();
                var confirmacoes = db.Confirmacoes.Where(c => !c.Cancela && c.Dia == today);

                foreach (var confirmacao in confirmacoes)
                {
                    if (confirmacao.StatusConfirmacao != StatusConfirmacao.Confirmado)
                    {
                        try
                        {
                            await ConfirmacaoWebService.Instance.ConfirmaAsync(confirmacao);
                            confirmacao.StatusConfirmacao = StatusConfirmacao.Confirmado;
                        }
                        catch (ServerException ex)
                        {
                            Console.WriteLine(ex.Message);
                            confirmacao.Mensagem = "Erro no servidor de confirmação";
                            confirmacao.StatusConfirmacao = StatusConfirmacao.Erro;
                        }
                        confirmacao.StatusSincronia = StatusSincronia.Modificado;

                    }

                    if (confirmacao.StatusSincronia != StatusSincronia.Sincronizado)
                    {
                        listaSync.Add(confirmacao);
                    }

                    confirmacao.StatusSincronia = StatusSincronia.Sincronizado;
                }

                await PostConfirmacoesAsync(listaSync);

                db.SaveChanges();
            }
        }

        private static async Task PostConfirmacoesAsync(List<Confirmacao> confirmacoes)
        {
            try
            {
                var ws = new SyncWebService();
                var result = await ws.PostConfirmacoesAsync(confirmacoes);
                for (int i = 0; i < result.Count; i++)
                {
                    confirmacoes[i].IdRemoto = result[i].Id;
                }

            }
            catch (ServerException ex)
            {
                Console.WriteLine(ex.Message);
                foreach (var item in confirmacoes)
                {
                    item.StatusSincronia = StatusSincronia.Erro;
                }
            }
        }

    }
}
