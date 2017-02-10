using System.Diagnostics;
using System.IO;
using Akka.Actor;
using ConsulMon.Actors;

namespace ConsulMon
{
    public class ConsulMonitorSystem
    {
        private readonly IActorRef _monitorActor;
        private readonly ActorSystem _system;

        public ConsulMonitorSystem()
        {
            _system = ActorSystem.Create(nameof(ConsulMonitorSystem));

            var exePath = Process.GetCurrentProcess().MainModule.FileName;
            var directoryPath = Path.GetDirectoryName(exePath);
            var props = Props.Create(() => new ConsulMonitorSupervisor(directoryPath));
            _monitorActor = _system.ActorOf(props, nameof(ConsulMonitorSupervisor));
        }

        public IActorRef CreateBridge(IConsulMonitorView view)
        {
            var bridgeProps = Props.Create(() => new UiBridgeActor(view, _monitorActor))
                .WithDispatcher("akka.actor.synchronized-dispatcher");
            return _system.ActorOf(bridgeProps, "bridge");
        }
    }
}