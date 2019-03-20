using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using MerendaIFCE.WebApp.Models;
using MerendaIFCE.WebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MerendaIFCE.WebApp.ApiControllers
{
    [Route("api/Confirmacoes")]
    [Produces("application/json")]
    [Authorize(Roles = Constants.SyncRole)]
    public class ConfirmacoesController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly NotificationService notification;

        public ConfirmacoesController(ApplicationDbContext db, NotificationService notification)
        {
            this.db = db;
            this.notification = notification;
        }

        [HttpGet]
        public IEnumerable<Confirmacao> GetConfirmacoes([FromQuery]DateTimeOffset? alteracao = null)
        {
            var semana = DateTimeOffset.Now - TimeSpan.FromDays(7);

            return db.Confirmacoes
                .Where(c => c.UltimaModificacao >= semana && (alteracao == null || c.UltimaModificacao > alteracao))
                .OrderByDescending(c => c.UltimaModificacao);
        }

        [HttpPost]
        public async Task<IActionResult> PostConfirmacoesAsync([FromBody] IEnumerable<Confirmacao> confirmacoes)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var now = DateTimeOffset.Now;
            var novas = confirmacoes.Where(c => c.Id == 0).ToList();
            var antigas = confirmacoes.Except(novas);

            foreach (var item in confirmacoes)
            {
                var inscricao = db.Inscricoes.Single(i => i.Id == item.InscricaoId);
                await notification.NotificaStatusRefeicaoAsync();

                item.UltimaModificacao = now;
            }

            db.Confirmacoes.AddRange(novas);
            foreach (var item in antigas)
            {
                db.Attach(item);
                db.Entry(item).State = EntityState.Modified;
            }

            try
            {
                db.SaveChanges();
            }
            catch (DBConcurrencyException ex)
            {
                return BadRequest(ex);
            }

            return Ok(confirmacoes);
        }
    }
}