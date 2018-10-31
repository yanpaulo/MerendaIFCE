using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MerendaIFCE.WebApp.Models
{
    public class RegistroNotificacaoViewModel
    {
        public PlataformaNotificacao Plataforma { get; set; }

        [Required]
        public string Handle { get; set; }
    }

    public enum PlataformaNotificacao
    {
        WNS = 1, GCM, APNS
    }
    
}
