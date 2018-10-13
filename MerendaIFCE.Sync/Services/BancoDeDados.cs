using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using MerendaIFCE.Sync.Models;
using Microsoft.EntityFrameworkCore;

namespace MerendaIFCE.Sync.Services
{
    public class BancoDeDados
    {
        public static async Task AtualizaAsync()
        {
            using (var db = new LocalDbContext())
            {
                db.Database.Migrate();

                var ws = new SyncWebService();
                var last = db.Inscricoes.OrderByDescending(i => i.UltimaModificacao).FirstOrDefault()?.UltimaModificacao;
                var alteracoes = await ws.GetInscricoesAsync(last);
                foreach (var remoto in alteracoes)
                {
                    var local = db.Inscricoes.Include(l => l.Dias).SingleOrDefault(i => i.Id == remoto.Id);
                    if (local != null)
                    {
                        db.Inscricoes.Remove(local);
                        db.InscricaoDias.RemoveRange(local.Dias);
                    }

                    db.Inscricoes.Add(remoto);
                }

                db.SaveChanges();
            }
        }
    }
}
