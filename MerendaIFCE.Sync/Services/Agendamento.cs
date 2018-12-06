using MerendaIFCE.Sync.Services.Jobs;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MerendaIFCE.Sync.Services
{
    public class Agendamento
    {
        public async static Task Inicializa()
        {
            var factory = new StdSchedulerFactory();
            var scheduler = await factory.GetScheduler();
            await scheduler.Start();

            var criacaoTrigger = TriggerBuilder.Create()
                .WithDailyTimeIntervalSchedule(builder =>
                    builder
                        .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(7, 0))
                        .OnMondayThroughFriday()
                        .WithIntervalInSeconds(30)
                        .EndingDailyAt(TimeOfDay.HourAndMinuteOfDay(11, 30)))
                .Build();

            var confirmacaoTrigger = TriggerBuilder.Create()
                .WithDailyTimeIntervalSchedule(builder =>
                    builder
                        .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(10, 0))
                        .OnMondayThroughFriday()
                        .WithIntervalInSeconds(30)
                        .EndingDailyAt(TimeOfDay.HourAndMinuteOfDay(10, 30)))
                .Build();

            var tj = new Dictionary<IJobDetail, IReadOnlyCollection<ITrigger>>
            {
                { JobBuilder.Create<CriadorJob>().Build(), new[]{ criacaoTrigger } },
            };

            await scheduler.ScheduleJobs(tj, true);
        }
    }
}
