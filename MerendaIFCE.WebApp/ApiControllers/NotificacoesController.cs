using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MerendaIFCE.WebApp.Models;
using MerendaIFCE.WebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.NotificationHubs;
using Microsoft.EntityFrameworkCore;

namespace MerendaIFCE.WebApp.ApiControllers
{
    [Route("api/Notificacoes")]
    [Produces("application/json")]
    public class NotificacoesController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly NotificationService notification;

        public NotificacoesController(NotificationService notification, ApplicationDbContext db)
        {
            this.db = db;
            this.notification = notification;
        }

        [HttpPost("Inscreve")]
        public async Task<IActionResult> InscreveAsync([FromBody]RegistroNotificacaoViewModel model)
        {
            var user = GetUser();
            var channelRegistrations = await notification.Hub.GetRegistrationsByChannelAsync(model.Handle, 10);
            foreach (var r in channelRegistrations.Skip(1))
            {
                await notification.Hub.DeleteRegistrationAsync(r);
            }

            var registration = channelRegistrations.FirstOrDefault() ?? CriaRegistration(model);

            registration.RegistrationId = registration.RegistrationId ?? await notification.Hub.CreateRegistrationIdAsync();
            registration.Tags = new HashSet<string> { $"matricula:{user.Inscricao.Matricula}" };
            await notification.Hub.CreateOrUpdateRegistrationAsync(registration);

            return Ok();
        }

        [HttpPost("Desinscreve")]
        public async Task<IActionResult> DesinscreveAsync([FromBody]RegistroNotificacaoViewModel model)
        {
            await notification.Hub.DeleteRegistrationsByChannelAsync(model.Handle);

            return Ok();
        }

        private RegistrationDescription CriaRegistration(RegistroNotificacaoViewModel model)
        {
            RegistrationDescription registration;
            switch (model.Plataforma)
            {
                case PlataformaNotificacao.WNS:
                    var template = @"<toast><visual><binding template=""ToastText01""><text id=""1"">$(message)</text></binding></visual></toast>";
                    registration = new WindowsTemplateRegistrationDescription(model.Handle, template);
                    break;
                case PlataformaNotificacao.GCM:
                    template = "{\"data\":{\"message\":\"$(message)\"}}";
                    registration = new FcmTemplateRegistrationDescription(model.Handle, template);
                    break;
                case PlataformaNotificacao.APNS:
                    template = "{\"aps\":{\"alert\":\"$(message)\"}}";
                    registration = new AppleTemplateRegistrationDescription(model.Handle, template);
                    break;
                default:
                    throw new InvalidOperationException();
            }
            return registration;

        }

        private ApplicationUser GetUser()
        {
            var username = User.Identity.Name;
            return db.Users.Include(u => u.Inscricao).Single(u => u.UserName == username);

        }
    }
}