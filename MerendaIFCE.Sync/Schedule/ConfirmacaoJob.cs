using FluentScheduler;
using MerendaIFCE.Sync.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MerendaIFCE.Sync.Services;

namespace MerendaIFCE.Sync.Schedule
{
    public class ConfirmacaoJob : IJob
    {
        public void Execute()
        {
            using (var db = new LocalDbContext())
            {
                var now = App.Current.Now;
                var today = new DateTimeOffset(now.Date);

                var dias = db.InscricaoDias.Include(d => d.Confirmacoes).Where(d => d.Dia == now.DayOfWeek);

                foreach (var dia in dias)
                {
                    var confirmacao = dia.Confirmacoes.FirstOrDefault(d => d.Dia == today);
                    if (confirmacao?.StatusConfirmacao != StatusConfirmacao.Confirmado)
                    {
                        if (confirmacao == null)
                        {
                            confirmacao = new Confirmacao { Dia = today };
                            dia.Confirmacoes.Add(confirmacao);
                        }
                        var cws = new ConfirmacaoWebService();
                        cws.Confirma(confirmacao);
                    }
                }
            }
        }
    }
}
