using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MerendaIFCE.WebApp.Models;
using MerendaIFCE.WebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MerendaIFCE.WebApp.ApiControllers
{
    [Produces("application/json")]
    [Route("api/Notificacoes")]
    [Authorize]
    public class NotificacoesController : Controller
    {
        private NotificationService notification;

        public NotificacoesController(NotificationService notification)
        {
            this.notification = notification;
        }

        [HttpPost("Registra")]
        public async Task<IActionResult> RegistraAsync(RegistroNotificacaoViewModel model)
        {
            throw new NotImplementedException();
        }
    }
}