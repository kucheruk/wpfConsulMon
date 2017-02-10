using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Akka.Actor;
using Akka.Routing;
using ConsulMon.Models;
using Newtonsoft.Json;

namespace ConsulMon.Actors
{
    public class ConsulMonitorSupervisor : ReceiveActor
    {
        private readonly Queue<ConsulServiceCheckJob> _jobs = new Queue<ConsulServiceCheckJob>();
        private readonly string _path;
        private IActorRef _bridge;
        private ICancelable _cancelable;

        public ConsulMonitorSupervisor(string path)
        {
            _path = path;
            var pool = Props.Create(() => new ConsulServiceChecker(Self)).WithRouter(new BroadcastPool(5));
            Context.ActorOf(pool);
            Receive<MsgTick>(_ => GenerateJobs());
            Receive<JobRequest>(rq => SendJob());
            Receive<ConsulServiceStatus>(jr => ReceiveCompleted(jr));
            Receive<IActorRef>(iar => AddSubscriber(iar));
            Schedule();
        }

        private void AddSubscriber(IActorRef iar)
        {
            _bridge = iar;
        }

        private void Schedule()
        {
            _cancelable = Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(TimeSpan.FromSeconds(1),
                TimeSpan.FromSeconds(15), Self, new MsgTick(), Self);
        }

        protected override void PostStop()
        {
            _cancelable?.Cancel();
            base.PostStop();
        }

        private void ReceiveCompleted(ConsulServiceStatus jr)
        {
            _bridge.Forward(jr);
        }

        private void SendJob()
        {
            if (_jobs.Count > 0)
                Sender.Tell(_jobs.Dequeue());
        }

        private void GenerateJobs()
        {
            var cfg = ReadConfig();
            var configs = cfg as ConsulMonitorConfig[] ?? cfg.ToArray();
            var len = configs.SelectMany(a => a.Services).Count();
            foreach (var config in configs)
            foreach (var service in config.Services)
            {
                _jobs.Enqueue(new ConsulServiceCheckJob(config.Host, service.Name, service.Tag));
                if (_jobs.Count > len * 2) break;
            }
        }

        private IEnumerable<ConsulMonitorConfig> ReadConfig()
        {
            return Directory.GetFiles(_path, "*.services.json")
                .Select(DeserializeConfigFile)
                .Where(f => f != null)
                .ToArray();
        }

        private ConsulMonitorConfig DeserializeConfigFile(string fileName)
        {
            try
            {
                var cfgContent = File.ReadAllText(fileName);
                return JsonConvert.DeserializeObject<ConsulMonitorConfig>(cfgContent);
            }
            catch (Exception)
            {
                //TODO: add logging
                return null;
            }
        }
    }
}