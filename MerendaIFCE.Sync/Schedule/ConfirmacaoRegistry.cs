using FluentScheduler;
using System;
using System.Collections.Generic;
using System.Text;

namespace MerendaIFCE.Sync.Schedule
{
    public class ConfirmacaoRegistry : Registry
    {
        private ConfirmacaoRegistry()
        {

        }

        public static void Registra()
        {
            JobManager.Initialize(new ConfirmacaoRegistry());
        }
    }
}
