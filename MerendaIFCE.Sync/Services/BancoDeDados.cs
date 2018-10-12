using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using MerendaIFCE.Sync.Models;

namespace MerendaIFCE.Sync.Services
{
    public class BancoDeDados
    {
        public static async Task AtualizaAsync()
        {
            using (var db = new LocalDbContext())
            {
                var ws = new SyncWebService();
                var last = db.Inscricoes.OrderByDescending(i => i.UltimaModificacao).FirstOrDefault()?.UltimaModificacao;
                var alteracoes = await ws.GetInscricoesAsync(last);
                foreach (var remoto in alteracoes)
                {
                    var local = db.Inscricoes.SingleOrDefault(i => i.IdRemoto == remoto.IdRemoto);
                    if (local != null)
                    {
                        db.Inscricoes.Remove(local);
                    }

                    db.Inscricoes.Add(remoto);
                }

                db.SaveChanges();
            }
        }
    }
}
