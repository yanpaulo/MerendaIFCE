using MerendaIFCE.WebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MerendaIFCE.WebApp.Models
{
    public class LoginResult
    {
        public string Email { get; set; }
        
        public string Token { get; set; }

        public Inscricao Inscricao { get; set; }
    }
}
