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
    public class DetailsModel : PageModel
    {
        private readonly MerendaIFCE.WebApp.Models.ApplicationDbContext _context;

        public DetailsModel(MerendaIFCE.WebApp.Models.ApplicationDbContext context)
        {
            _context = context;
        }

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
    }
}
