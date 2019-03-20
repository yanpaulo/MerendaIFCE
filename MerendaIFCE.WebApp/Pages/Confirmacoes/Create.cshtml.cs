using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MerendaIFCE.WebApp.Models;

namespace MerendaIFCE.WebApp.Pages.Confirmacoes
{
    public class CreateModel : PageModel
    {
        private readonly MerendaIFCE.WebApp.Models.ApplicationDbContext _context;

        public CreateModel(MerendaIFCE.WebApp.Models.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Confirmacao Confirmacao { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Confirmacoes.Add(Confirmacao);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}