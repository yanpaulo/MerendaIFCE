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
        public static void Inicializa()
        {
            var connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:53353/Sync")
                .Build();

            connection.On<Inscricao>("Inscricao_Changed", AtualizaInscricao);

            connection.Closed += async error =>
            {
                await Task.Delay(1000);
                await connection.StartAsync();
            };

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
