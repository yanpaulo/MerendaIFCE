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
        public const string ConfirmacaoChanged = "ConfirmacaoChanged";

        public override async Task OnConnectedAsync()
        {
            if (Context.User.IsInRole(Constants.SyncRole))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, Constants.SyncRole);
            }
        }
    }
}
