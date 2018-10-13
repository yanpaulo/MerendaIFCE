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
            Schedule<ConfirmacaoJob>().ToRunNow().AndEvery(5).Minutes();
        }

        public static void Registra()
        {
            JobManager.Initialize(new ConfirmacaoRegistry());
        }
    }
}
