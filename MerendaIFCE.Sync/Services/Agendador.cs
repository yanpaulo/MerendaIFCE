﻿using log4net;
using MerendaIFCE.Sync.Services.Jobs;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MerendaIFCE.Sync.Services
{
    public class Agendador
    {
        private static ILog log = LogManager.GetLogger(typeof(Agendador));
        private IScheduler scheduler;
        private Agendador() { }

        public static Agendador Instance { get; private set; }

        public async static Task InicializaAsync()
        {
            log.Debug($"Inicializando {nameof(Agendador)}");
            if (Instance != null)
            {
                throw new InvalidOperationException($"{nameof(Agendador)}.{nameof(InicializaAsync)} já foi chamado anteriormente.");
            }
            Instance = new Agendador();

            var factory = new StdSchedulerFactory();
            Instance.scheduler = await factory.GetScheduler();

            var criacaoTrigger = TriggerBuilder.Create()
                .WithDailyTimeIntervalSchedule(builder =>
                    builder
                        .OnMondayThroughFriday()
                        .WithIntervalInSeconds(30)
                    )
                .Build();

            var confirmacaoTrigger = TriggerBuilder.Create()
                .WithDailyTimeIntervalSchedule(builder =>
                    builder
                        .OnMondayThroughFriday()
                        .WithIntervalInSeconds(60)
#if HORARIO_CONFIRMACAO
                        .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(10, 0))
                        .EndingDailyAt(TimeOfDay.HourAndMinuteOfDay(10, 30))
#endif
                )
                .Build();

            var tj = new Dictionary<IJobDetail, IReadOnlyCollection<ITrigger>>
            {
                { JobBuilder.Create<CriadorJob>().Build(), new[]{ criacaoTrigger } },
                { JobBuilder.Create<ConfirmadorJob>().Build(), new[]{ confirmacaoTrigger } },
            };

            await Instance.scheduler.ScheduleJobs(tj, true);
        }

        public async Task IniciaAsync()
        {
            log.Debug("Iniciando agendamentos.");
            await scheduler.Start();
        }

        public async Task ParaAsync()
        {
            log.Debug("Parando agendamentos.");

            await scheduler.Shutdown();
        }
    }
}
