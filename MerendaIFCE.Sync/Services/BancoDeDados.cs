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
                await AtualizaInscricoes(db, ws);
                await AtualizaConfirmacoes(db, ws);

                db.SaveChanges();
            }
        }

        private static async Task AtualizaInscricoes(LocalDbContext db, SyncWebService ws)
        {
            var ultima = db.Inscricoes.OrderByDescending(i => i.UltimaModificacao).FirstOrDefault()?.UltimaModificacao;

            var alteracoes = await ws.GetInscricoesAsync(ultima);
            foreach (var remoto in alteracoes)
            {
                if (db.Inscricoes.Include(i => i.Dias).SingleOrDefault(i => i.Id == remoto.Id) is Inscricao local)
                {
                    db.RemoveRange(local.Dias);

                    db.Entry(local).CurrentValues.SetValues(remoto);
                    db.Entry(local).Collection(l => l.Dias).CurrentValue = remoto.Dias;
                }
                else
                {
                    db.Add(remoto);
                }
            }
        }

        private static async Task AtualizaConfirmacoes(LocalDbContext db, SyncWebService ws)
        {
            var ultima = db.Confirmacoes.OrderByDescending(i => i.UltimaModificacao).FirstOrDefault()?.UltimaModificacao;

            var alteracoes = await ws.GetConfirmacoesAsync(ultima);
            foreach (var remoto in alteracoes)
            {
                remoto.StatusSincronia = StatusSincronia.Sincronizado;

                if (db.Confirmacoes.SingleOrDefault(i => i.IdRemoto == remoto.IdRemoto) is Confirmacao local)
                {
                    remoto.Id = local.Id;
                    db.Entry(local).CurrentValues.SetValues(remoto);
                }
                else
                {
                    db.Add(remoto);
                }

            }
        }
    }
}
