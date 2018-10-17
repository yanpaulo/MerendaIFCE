using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MerendaIFCE.WebApp.Models;
using MerendaIFCE.WebApp.Services;
using Microsoft.AspNetCore.SignalR;

namespace MerendaIFCE.WebApp.ApiControllers
{
    [Produces("application/json")]
    [Route("api/Inscricoes")]
    public class InscricoesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<SyncHub> _hubContext;

        public InscricoesController(ApplicationDbContext context, IHubContext<SyncHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        [HttpPost("{id}/Dias")]
        public async Task<IActionResult> PostInscricaoDia(int id, [FromBody]InscricaoDia dia)
        {
            var user = GetUser();
            if (id != dia.InscricaoId)
            {
                ModelState.AddModelError(nameof(InscricaoDia.InscricaoId), "InscricaoId diferente do especificado na URL");
            }
            if (user.Inscricao.Id != id)
            {
                ModelState.AddModelError(nameof(InscricaoDia.InscricaoId), "Não é possível alterar a inscrição de outro usuário, espertinho.");
            }

            if (ModelState.IsValid)
            {
                _context.InscricaoDias.Add(dia);
                _context.SaveChanges();
                await _hubContext.Clients.All.SendAsync(SyncHub.InscricaoChanged, user.Inscricao);

                return Ok(dia);
            }

            return BadRequest(ModelState);
        }

        [HttpDelete("{idInscricao}/Dias/{idDia}")]
        public async Task<IActionResult> DeleteInscricaoDia(int idInscricao, int idDia)
        {
            var user = GetUser();
            if (user.Inscricao.Id != idInscricao)
            {
                ModelState.AddModelError("", "Não é possível alterar a inscrição de outro usuário, espertinho.");
            }
            if (!user.Inscricao.Dias.Any(d => d.Id == idDia))
            {
                ModelState.AddModelError("", "Dia não encontrado.");
            }
            if (ModelState.IsValid)
            {
                _context.InscricaoDias.Remove(user.Inscricao.Dias.Single(d => d.Id == idDia));
                _context.SaveChanges();
                await _hubContext.Clients.All.SendAsync(SyncHub.InscricaoChanged, user.Inscricao);

                return Ok();
            }

            return BadRequest(ModelState);
        }

        // GET: api/Inscricoes
        [HttpGet]
        public IEnumerable<Inscricao> GetInscricoes(DateTimeOffset? alteracao = null)
        {
            return _context.Inscricoes.Include(i => i.Dias)
                .Where(i => alteracao == null || i.UltimaModificacao > alteracao);
        }

        [HttpGet("{id}/Confirmacoes")]
        public IEnumerable<Confirmacao> GetConfirmacoes(int id, DateTimeOffset? alteracao = null)
        {
            return _context.Confirmacoes
                .Where(c => c.InscricaoId == id && (alteracao == null || c.UltimaModificacao > alteracao));
        }

        // GET: api/Inscricoes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetInscricao([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var inscricao = await _context.Inscricoes.SingleOrDefaultAsync(m => m.Id == id);

            if (inscricao == null)
            {
                return NotFound();
            }

            return Ok(inscricao);
        }

        #region NonAction
        // PUT: api/Inscricoes/5
        [NonAction]
        public async Task<IActionResult> PutInscricao([FromRoute] int id, [FromBody] Inscricao inscricao)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != inscricao.Id)
            {
                return BadRequest();
            }

            var original = _context.Inscricoes.Include(i => i.Dias).SingleOrDefault(i => i.Id == inscricao.Id);
            if (original == null)
            {
                return NotFound();
            }

            original.Matricula = inscricao.Matricula;
            original.UltimaModificacao = DateTimeOffset.Now;

            original.Dias = inscricao.Dias.Select(d => new InscricaoDia
            {
                Dia = d.Dia
            }).ToList();


            await _context.SaveChangesAsync();
            await _hubContext.Clients.All.SendAsync(SyncHub.InscricaoChanged, inscricao);

            return NoContent();
        }

        // POST: api/Inscricoes
        [HttpPost]
        [NonAction]
        public async Task<IActionResult> PostInscricao([FromBody] Inscricao inscricao)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            inscricao.Id = 0;
            inscricao.Dias = inscricao.Dias.Select(d => new InscricaoDia
            {
                Dia = d.Dia
            }).ToList();

            _context.Inscricoes.Add(inscricao);
            await _context.SaveChangesAsync();
            await _hubContext.Clients.All.SendAsync(SyncHub.InscricaoChanged, inscricao);

            return CreatedAtAction("GetInscricao", new { id = inscricao.Id }, inscricao);
        }

        // DELETE: api/Inscricoes/5
        [HttpDelete("{id}")]
        [NonAction]
        public async Task<IActionResult> DeleteInscricao([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var inscricao = await _context.Inscricoes.SingleOrDefaultAsync(m => m.Id == id);
            if (inscricao == null)
            {
                return NotFound();
            }

            _context.Inscricoes.Remove(inscricao);
            await _context.SaveChangesAsync();

            return Ok(inscricao);
        }

        #endregion

        private bool InscricaoExists(int id)
        {
            return _context.Inscricoes.Any(e => e.Id == id);
        }

        private ApplicationUser GetUser()
        {
            var username = User.Identity.Name;
            var user = _context.Users.Include(u => u.Inscricao).Single(u => u.UserName == username);
            return user;
        }
    }
}