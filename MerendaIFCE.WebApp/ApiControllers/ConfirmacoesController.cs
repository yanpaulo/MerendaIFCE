using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using MerendaIFCE.WebApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MerendaIFCE.WebApp.ApiControllers
{
    [Produces("application/json")]
    [Route("api/Confirmacoes")]
    public class ConfirmacoesController : Controller
    {
        private readonly ApplicationDbContext db;

        public ConfirmacoesController(ApplicationDbContext db)
        {
            this.db = db;
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
        public IActionResult PostConfirmacoes([FromBody] IEnumerable<Confirmacao> confirmacoes)
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