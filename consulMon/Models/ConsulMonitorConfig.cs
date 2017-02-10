using System.Collections.Generic;

namespace ConsulMon.Models
{
    public class ConsulMonitorConfig
    {
        public string Host { get; set; }
        public IList<ConsulMonitorServiceConfig> Services = new List<ConsulMonitorServiceConfig>();
    }
}