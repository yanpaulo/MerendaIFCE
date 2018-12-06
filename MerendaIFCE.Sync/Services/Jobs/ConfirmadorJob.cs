using Quartz;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MerendaIFCE.Sync.Services.Jobs
{
    public class ConfirmadorJob : IJob
    {
        public async Task Execute(IJobExecutionContext context) => 
            await Tarefas.ExecutaConfirmacoesAsync();
    }
}
