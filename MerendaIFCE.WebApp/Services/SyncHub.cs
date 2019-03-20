using MerendaIFCE.WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MerendaIFCE.WebApp.Services
{
    [Authorize(AppPolicyProvider.SignalRPolicyName)]
    public class SyncHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            if (Context.User.IsInRole(Constants.SyncRole))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, Constants.SyncRole);
            }
        }
    }
}
