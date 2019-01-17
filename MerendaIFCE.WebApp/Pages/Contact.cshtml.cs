using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MerendaIFCE.WebApp.Pages
{
    public class ContactModel : PageModel
    {
        public void OnGet()
        {
            ViewData["Message"] = "Your contact page.";
        }
    }
}