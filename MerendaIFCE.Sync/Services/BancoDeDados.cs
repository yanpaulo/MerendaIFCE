﻿using System;
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
                var local = db.Inscricoes.SingleOrDefault(i => i.Id == remoto.Id);
                if (local != null)
                {
                    db.Inscricoes.Remove(local);
                }

                db.Inscricoes.Add(remoto);
            }
        }

        private static async Task AtualizaConfirmacoes(LocalDbContext db, SyncWebService ws)
        {
            var ultima = db.Confirmacoes.OrderByDescending(i => i.UltimaModificacao).FirstOrDefault()?.UltimaModificacao;

            var alteracoes = await ws.GetConfirmacoesAsync(ultima);
            foreach (var remoto in alteracoes)
            {
                var local = db.Confirmacoes.SingleOrDefault(i => i.IdRemoto == remoto.Id);
                if (local != null)
                {
                    db.Confirmacoes.Remove(local);
                }

                remoto.StatusSincronia = StatusSincronia.Sincronizado;
                db.Confirmacoes.Add(remoto);
            }
        }
    }
}
