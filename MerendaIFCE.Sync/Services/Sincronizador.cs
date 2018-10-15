using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using MerendaIFCE.Sync.Models;
using Microsoft.AspNetCore.SignalR.Client;

namespace MerendaIFCE.Sync.Services
{
    public class Sincronizador
    {
        public static async Task InicializaAsync()
        {
            var connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:7354/sync")
                .Build();

            connection.On<Inscricao>("InscricaoChanged", AtualizaInscricao);

            connection.Closed += async error =>
            {
                await Task.Delay(1000);
                await connection.StartAsync();
            };

            await connection.StartAsync();

        }

        private static void AtualizaInscricao(Inscricao inscricao)
        {
            using (var db = new LocalDbContext())
            {
                var local = db.Inscricoes.SingleOrDefault(i => i.Id == inscricao.Id);
                if (local != null)
                {
                    db.Inscricoes.Remove(local);
                }

                db.Inscricoes.Add(inscricao);

                db.SaveChanges();
            }
        }
    }
}
