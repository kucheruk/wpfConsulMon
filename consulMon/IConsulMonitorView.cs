using ConsulMon.Models;

namespace ConsulMon
{
    public interface IConsulMonitorView
    {
        void UpdateServiceStatus(ConsulServiceStatus m);
    }
}