using System;
using System.Collections.Generic;
using System.Text;

namespace MerendaIFCE.Sync.Models
{
    public class Login
    {
        public string UserName { get; set; }

        public string Password { get; set; }
    }

    public class Usuario
    {
        public int Id { get; set; }

        public string Login { get; set; }

        public string Token { get; set; }
    }
}
