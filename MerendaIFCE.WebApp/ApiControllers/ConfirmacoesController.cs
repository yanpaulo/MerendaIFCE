using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using MerendaIFCE.WebApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
            db.Confirmacoes.AttachRange(antigas);

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