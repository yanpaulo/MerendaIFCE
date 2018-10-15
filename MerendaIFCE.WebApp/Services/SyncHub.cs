using MerendaIFCE.WebApp.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MerendaIFCE.WebApp.Services
{
    public class SyncHub : Hub
    {
        public const string InscricaoChanged = "InscricaoChanged";

        public async Task NotifyInscricaoCHanged(Inscricao inscricao)
        {
            await Clients.All.SendAsync(InscricaoChanged, inscricao);
        }
    }
}
