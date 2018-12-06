﻿using System;
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
        private static Task<string> GetToken()
        {
            using (var db = new LocalDbContext())
            {
                return Task.FromResult(db.Usuario.SingleOrDefault()?.Token);
            }
        }

        public static async Task InicializaAsync()
        {
            var connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:7354/sync", options =>
                {

                    options.AccessTokenProvider = GetToken;
                })
                .Build();
            
            connection.On<Inscricao>("InscricaoChanged", AtualizaInscricao);
            connection.On<Confirmacao>("ConfirmacaoChanged", AtualizaConfirmacao);

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
                db.UpdateInscricao(inscricao);
                db.SaveChanges();
            }
        }

        private static void AtualizaConfirmacao(Confirmacao confirmacao)
        {
            using (var db = new LocalDbContext())
            {
                confirmacao.IdRemoto = confirmacao.Id;
                db.UpdateConfirmacao(confirmacao);
                db.SaveChanges();
            }
        }
    }
}
