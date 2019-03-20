using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MerendaIFCE.WebApp.Models;
using MerendaIFCE.WebApp.Services;
using Microsoft.AspNetCore.SignalR;

namespace MerendaIFCE.WebApp.Pages.Confirmacoes
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly NotificationService _notificationService;
        private readonly SyncHubService _hubService;

        public EditModel(ApplicationDbContext context, NotificationService notificationService, SyncHubService hubService)
        {
            _context = context;
            _notificationService = notificationService;
            _hubService = hubService;
        }

        [BindProperty]
        public Confirmacao Confirmacao { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Confirmacao = await _context.Confirmacoes.FirstOrDefaultAsync(m => m.Id == id);

            if (Confirmacao == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Confirmacao.UltimaModificacao = DateTimeOffset.Now;
            _context.Attach(Confirmacao).State = EntityState.Modified;
            _context.Entry(Confirmacao).Property(c => c.Dia).IsModified = false;
            _context.Entry(Confirmacao).Property(c => c.InscricaoId).IsModified = false;

            try
            {
                await _context.SaveChangesAsync();

                Confirmacao = (Confirmacao)_context.Entry(Confirmacao).GetDatabaseValues().ToObject();
                var inscricao = _context.Inscricoes.Find(Confirmacao.InscricaoId);
                await _notificationService.NotificaStatusRefeicaoAsync(inscricao.Matricula);
                await _hubService.NotificaConfirmacaoAlteradaAsync(Confirmacao);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ConfirmacaoExists(Confirmacao.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool ConfirmacaoExists(int id)
        {
            return _context.Confirmacoes.Any(e => e.Id == id);
        }
    }
}
