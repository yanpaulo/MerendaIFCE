using MerendaIFCE.Sync.Schedule;
using System;
using System.Collections.Generic;
using System.Text;

namespace MerendaIFCE.Sync.Services
{
    public class Agendamento
    {
        public static void Inicializa()
        {
            ConfirmacaoRegistry.Registra();
        }
    }
}
