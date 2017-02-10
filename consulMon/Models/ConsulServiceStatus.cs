using Consul;

namespace ConsulMon.Models
{
    public class ConsulServiceStatus
    {
        public HealthStatus[] Statuses { get; set; }
        public string Host { get; set; }
        public string Service { get; set; }
        public string Tag { get; set; }
        public QueryResult<ServiceEntry[]> Details { get; set; }
    }
}