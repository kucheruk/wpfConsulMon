using Akka.Actor;
using ConsulMon.Models;

namespace ConsulMon.Actors
{
    public class UiBridgeActor : ReceiveActor
    {
        private readonly IActorRef _monitorActor;
        private readonly IConsulMonitorView _view;

        public UiBridgeActor(IConsulMonitorView view, IActorRef monitorActor)
        {
            _view = view;
            _monitorActor = monitorActor;
            _monitorActor.Tell(Self);
            Become(Active);
        }

        private void Active()
        {
            Receive<ConsulServiceStatus>(s => _view.UpdateServiceStatus(s));
        }
    }
}