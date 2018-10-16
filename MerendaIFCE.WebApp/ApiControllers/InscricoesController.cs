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

        // GET: api/Inscricoes
        [HttpGet]
        public IEnumerable<Inscricao> GetInscricoes(DateTimeOffset? alteracao = null)
        {
            return _context.Inscricoes.Include(i => i.Dias)
                .Where(i => alteracao == null || i.UltimaModificacao > alteracao);
        }

        [HttpGet("{id}/confirmacoes")]
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

        // PUT: api/Inscricoes/5
        [HttpPut("{id}")]
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

        private bool InscricaoExists(int id)
        {
            return _context.Inscricoes.Any(e => e.Id == id);
        }
    }
}