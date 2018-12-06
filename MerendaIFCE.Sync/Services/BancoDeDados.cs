using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using MerendaIFCE.Sync.Models;
using Microsoft.EntityFrameworkCore;
using log4net;

namespace MerendaIFCE.Sync.Services
{
    public class BancoDeDados
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(BancoDeDados));

        public static async Task AtualizaAsync()
        {
            log.Debug("Atualizando banco de dados.");

            try
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
            //Tipos conhecidos de exceção.
            catch (ServerException)
            {
                throw;
            }
            catch (DbUpdateException)
            {
                throw;
            }
        }

        private static async Task AtualizaInscricoes(LocalDbContext db, SyncWebService ws)
        {
            log.Debug("Atualizando inscrições");

            var ultima = db.Inscricoes.OrderByDescending(i => i.UltimaModificacao).FirstOrDefault()?.UltimaModificacao;

            var alteracoes = await ws.GetInscricoesAsync(ultima);
            foreach (var remoto in alteracoes)
            {
                db.UpdateInscricao(remoto);
            }
        }

        private static async Task AtualizaConfirmacoes(LocalDbContext db, SyncWebService ws)
        {
            log.Debug("Atualizando confirmações");
            var ultima = db.Confirmacoes.OrderByDescending(i => i.UltimaModificacao).FirstOrDefault()?.UltimaModificacao;

            var alteracoes = await ws.GetConfirmacoesAsync(ultima);
            foreach (var remoto in alteracoes)
            {
                remoto.StatusSincronia = StatusSincronia.Sincronizado;

                db.UpdateConfirmacao(remoto);

            }
        }
    }
}
