using System;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Consul;
using ConsulMon.Models;

namespace ConsulMon.Actors
{
    public class ConsulServiceChecker : ReceiveActor
    {
        private readonly IActorRef _jobs;

        public ConsulServiceChecker(IActorRef jobs)
        {
            _jobs = jobs;
            ScheduleWakeup();
            Receive<ReceiveTimeout>(_ => GetJob());
            ReceiveAsync<ConsulServiceCheckJob>(CheckConsulServiceAsync);
        }

        private static void ScheduleWakeup()
        {
            Context.SetReceiveTimeout(TimeSpan.FromSeconds(1));
        }

        private async Task CheckConsulServiceAsync(ConsulServiceCheckJob csj)
        {
            using (var cc = new ConsulClient(cfg => { cfg.Address = new Uri("http://" + csj.Host + ":8500"); }))
            {
                var ret = await cc.Health.Service(csj.Service, csj.Tag);
                var statuses = ret.Response?.SelectMany(a => a.Checks.Select(z => z.Status)).ToArray();
                Sender.Tell(new ConsulServiceStatus()
                {
                    Host = csj.Host,
                    Service = csj.Service,
                    Tag = csj.Tag,
                    Statuses = statuses,
                    Details = ret
                });
            }
            ScheduleWakeup();
        }

        private void GetJob()
        {
            _jobs.Tell(new JobRequest());
            ScheduleWakeup();
        }
    }
}