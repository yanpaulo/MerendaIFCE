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
    public class IndexModel : PageModel
    {
        private readonly MerendaIFCE.WebApp.Models.ApplicationDbContext _context;

        public IndexModel(MerendaIFCE.WebApp.Models.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Confirmacao> Confirmacao { get;set; }

        public async Task OnGetAsync()
        {
            Confirmacao = await _context.Confirmacoes.ToListAsync();
        }
    }
}
