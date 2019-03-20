using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MerendaIFCE.WebApp.Models;

namespace MerendaIFCE.WebApp.Pages.Confirmacoes
{
    public class DeleteModel : PageModel
    {
        private readonly MerendaIFCE.WebApp.Models.ApplicationDbContext _context;

        public DeleteModel(MerendaIFCE.WebApp.Models.ApplicationDbContext context)
        {
            _context = context;
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

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Confirmacao = await _context.Confirmacoes.FindAsync(id);

            if (Confirmacao != null)
            {
                _context.Confirmacoes.Remove(Confirmacao);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
