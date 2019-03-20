using MerendaIFCE.WebApp.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MerendaIFCE.WebApp.Services
{
    public class SyncHubService
    {
        public const string InscricaoChanged = "InscricaoChanged";
        public const string ConfirmacaoChanged = "ConfirmacaoChanged";

        private readonly IHubContext<SyncHub> _hubContext;

        public SyncHubService(IHubContext<SyncHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task NotificaConfirmacaoAlteradaAsync(Confirmacao confirmacao) =>
            await _hubContext.Clients.Group(Constants.SyncRole).SendAsync(ConfirmacaoChanged, confirmacao);

        public async Task NotificaInscricaoAlteradaAsync(Inscricao inscricao) =>
            await _hubContext.Clients.Group(Constants.SyncRole).SendAsync(InscricaoChanged, inscricao);

    }
}
