using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MerendaIFCE.WebApp.Models.AccountViewModels
{
    public class RegisterUsernameViewModel
    {
        [Required]
        public string Login { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Senha { get; set; }

        [Required]
        [Compare(nameof(Senha))]
        public string ConfirmarSenha { get; set; }
    }
}
