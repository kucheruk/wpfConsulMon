namespace ConsulMon.Models
{
    internal class ConsulServiceCheckJob
    {
        public ConsulServiceCheckJob(string host, string service, string tag)
        {
            Host = host;
            Service = service;
            Tag = tag;
        }

        public string Host { get; }
        public string Service { get; }
        public string Tag { get; }
    }
}