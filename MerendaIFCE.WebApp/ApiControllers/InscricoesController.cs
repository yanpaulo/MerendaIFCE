﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MerendaIFCE.WebApp.Models;
using MerendaIFCE.WebApp.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace MerendaIFCE.WebApp.ApiControllers
{
    [Produces("application/json")]
    [Route("api/Inscricoes")]
    public class InscricoesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<SyncHub> _hubContext;
        private readonly UserManager<ApplicationUser> userManager;

        public InscricoesController(ApplicationDbContext context, IHubContext<SyncHub> hubContext, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _hubContext = hubContext;
            this.userManager = userManager;
        }

        // GET: api/Inscricoes
        [HttpGet]
        [Authorize(Roles = Constants.SyncRole)]
        //[Authorize]
        public IEnumerable<Inscricao> GetInscricoes(DateTimeOffset? alteracao = null)
        {
            return _context.Inscricoes.Include(i => i.Dias)
                .Where(i => alteracao == null || i.UltimaModificacao > alteracao);
        }

        [HttpPost("{id}/Dias")]
        [Authorize(Roles = Constants.UserRole)]
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
                var inscricao = user.Inscricao;
                if (!inscricao.Dias.Any(d => d.Dia == dia.Dia))
                {
                    inscricao.UltimaModificacao = DateTimeOffset.Now;
                    inscricao.UltimaModificacao = DateTimeOffset.Now;

                    _context.InscricaoDias.Add(dia);
                    _context.SaveChanges();


                    await HubSend(SyncHub.InscricaoChanged, user.Inscricao);

                    return Created($"{inscricao.Id}/Dias/{dia.Id}", dia);
                }
                
                return Ok(dia);
            }

            return BadRequest(ModelState);
        }

        [HttpDelete("{idInscricao}/Dias/{idDia}")]
        [Authorize(Roles = Constants.UserRole)]
        public async Task<IActionResult> DeleteInscricaoDia(int idInscricao, int idDia)
        {
            var user = GetUser();
            if (user.Inscricao.Id != idInscricao)
            {
                ModelState.AddModelError("", "Não é possível alterar a inscrição de outro usuário, espertinho.");
            }
            if (ModelState.IsValid)
            {
                var inscricao = user.Inscricao;
                if (inscricao.Dias.Any(d => d.Id == idDia))
                {
                    inscricao.UltimaModificacao = DateTimeOffset.Now;
                    _context.InscricaoDias.Remove(user.Inscricao.Dias.Single(d => d.Id == idDia));
                    _context.SaveChanges();

                    await HubSend(SyncHub.InscricaoChanged, user.Inscricao);
                }

                return Ok();
            }

            return BadRequest(ModelState);
        }


        [HttpGet("Confirmacoes")]
        [Authorize(Roles = Constants.UserRole)]
        public IEnumerable<Confirmacao> GetConfirmacoes([FromQuery]DateTimeOffset? alteracao = null)
        {
            var user = GetUser();
            
            return _context.Confirmacoes
                .OrderByDescending(c => c.UltimaModificacao)
                .Where(c => c.InscricaoId == user.Inscricao.Id && (alteracao == null || c.UltimaModificacao > alteracao))
                .Take(20);
        }

        [HttpPut("Confirmacoes/{id}")]
        [Authorize(Roles = Constants.UserRole)]
        public IActionResult PutConfirmacao(int id, [FromBody]Confirmacao confirmacao)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (id != confirmacao.Id)
            {
                return BadRequest();
            }
            if (confirmacao.InscricaoId != GetUser().Inscricao.Id)
            {
                return BadRequest();
            }
            
            if (_context.Confirmacoes.SingleOrDefault(c => c.Id == confirmacao.Id) is Confirmacao local)
            {
                _context.Entry(local).State = EntityState.Detached;
                _context.Entry(confirmacao).State = EntityState.Modified;
                confirmacao.UltimaModificacao = DateTimeOffset.Now;

                _context.SaveChanges();
                return Ok(confirmacao);
            }
            return BadRequest();
        }

        #region NonAction
        // GET: api/Inscricoes/5
        [HttpGet("{id}")]
        [NonAction]
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
            await HubSend(SyncHub.InscricaoChanged, inscricao);

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
            await HubSend(SyncHub.InscricaoChanged, inscricao);

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

        private async Task HubSend(string method, object arg)
        {
            var users = await userManager.GetUsersInRoleAsync(Constants.SyncRole);
            var clients = _hubContext.Clients.Users(users.Select(u => u.UserName).ToList());
            await clients.SendAsync(method, arg);
        }

        private bool InscricaoExists(int id)
        {
            return _context.Inscricoes.Any(e => e.Id == id);
        }

        private ApplicationUser GetUser()
        {
            var username = User.Identity.Name;
            var user = _context.Users
                .Include(u => u.Inscricao)
                    .ThenInclude(i => i.Dias)
                .Single(u => u.UserName == username);
            return user;
        }
    }
}