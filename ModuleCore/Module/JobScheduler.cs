using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;

namespace ModuleCore
{
    public static class JobScheduler
    {
        private static ISchedulerFactory scheduleFactory = null;
        private static Dictionary<JobKey, IScheduler> schedulers = null;
        private const string DEFAULT_GROUP_NAME = "FXAPI";
        private const string DEFAULT_NAME = "IMPORT";

        static JobScheduler()
        {
            scheduleFactory = new StdSchedulerFactory();
            schedulers = new Dictionary<JobKey, IScheduler>();
        }

        public static JobKey Start<T>(params string[] cronExpressions) where T : IJob
        {
            try
            {
                IScheduler scheduler = scheduleFactory.GetScheduler();

                string guidSuffix = Guid.NewGuid().ToString();
                JobKey jobKey = new JobKey(DEFAULT_NAME + guidSuffix, DEFAULT_GROUP_NAME + guidSuffix);
                IJobDetail job = JobBuilder.Create<T>()
                       .WithIdentity(jobKey)
                       .Build();

                foreach (var cronExp in cronExpressions)
                {
                    // Trigger the job to run now and wrt cron schedule
                    ITrigger trigger = TriggerBuilder.Create().WithCronSchedule(cronExp).StartNow().Build();
                    scheduler.ScheduleJob(job, trigger);
                }

                scheduler.Start();

                schedulers.Add(jobKey, scheduler);

                return jobKey;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static JobKey StartWithData<T>(List<KeyValuePair<string, object>> contextData, string cronExpression, bool startImmediately) where T : IJob
        {
            try
            {
                IScheduler scheduler = scheduleFactory.GetScheduler();

                string guidSuffix = Guid.NewGuid().ToString();
                JobKey jobKey = new JobKey(DEFAULT_NAME + guidSuffix, DEFAULT_GROUP_NAME + guidSuffix);
                IJobDetail job = JobBuilder.Create<T>()
                       .WithIdentity(jobKey)
                       .Build();

                // Trigger the job to run now and wrt cron schedule
                ITrigger trigger = TriggerBuilder.Create().WithCronSchedule(cronExpression).StartNow().Build();
                scheduler.ScheduleJob(job, trigger);


                foreach (var item in contextData)
                {
                    scheduler.Context.Put(item.Key, item.Value);
                }

                scheduler.Start();

                schedulers.Add(jobKey, scheduler);

                if (startImmediately)
                {
                    scheduler.TriggerJob(jobKey);
                }

                return jobKey;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public static void PauseJob(JobKey jobkey)
        {
            if (schedulers.ContainsKey(jobkey))
            {
                IScheduler scheduler = schedulers[jobkey];
                scheduler.PauseJob(jobkey);
            }
        }

        public static void ResumeJob(JobKey jobkey)
        {
            if (schedulers.ContainsKey(jobkey))
            {
                IScheduler scheduler = schedulers[jobkey];
                scheduler.ResumeJob(jobkey);
            }
        }

        public static bool IsRunning(JobKey jobkey)
        {
            if (jobkey != null && schedulers.ContainsKey(jobkey))
            {
                IScheduler scheduler = schedulers[jobkey];
                return scheduler.IsStarted;
            }

            return false;
        }

        public static void Stop(JobKey jobkey)
        {
            if (schedulers.ContainsKey(jobkey))
            {
                schedulers[jobkey].Shutdown();
                schedulers.Remove(jobkey);
            }
        }

        public static void StopAll()
        {
            foreach (var scheduler in schedulers)
            {
                scheduler.Value.Shutdown();
            }
            schedulers = null;
        }
    }
}
