using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using ConsulMon.Models;
using ConsulMon.Properties;

namespace ConsulMon.ViewModels
{
    public class AppViewmodel : IConsulMonitorView, INotifyPropertyChanged
    {
        private int _totalHeight;
        public ObservableCollection<ConsulHostVm> Hosts { get; set; }

        public int TotalHeight
        {
            get { return _totalHeight; }
            set
            {
                if (value == _totalHeight) return;
                _totalHeight = value;
                OnPropertyChanged();
            }
        }

        public AppViewmodel()
        {
            Hosts = new ObservableCollection<ConsulHostVm>();
            App.ConsulMonitorSystem.CreateBridge(this);
            HostHeight = 32;
            ServiceHeight = 32;
        }

        public void UpdateServiceStatus(ConsulServiceStatus m)
        {
            var host = GetExistingOrNew(m.Host);
            var service = host.GetOrCreateService(m.Service, m.Tag);
            service.SetStatus(m);
            CalculateHeight();
        }

        private void CalculateHeight()
        {
            var hh = Hosts.Select(a => a.Height).Sum();
            var sh = Hosts.SelectMany(h => h.Services).Select(a => a.Height).Sum();
            TotalHeight = sh + hh + 10;
        }

        public int ServiceHeight { get; set; }

        public int HostHeight { get; set; }

        private ConsulHostVm GetExistingOrNew(string host)
        {
            var existing = Hosts.FirstOrDefault(a => a.Host == host);
            return existing ?? AddHost(host);
        }

        private ConsulHostVm AddHost(string host)
        {
            var ret = new ConsulHostVm {Host = host, Services = new ObservableCollection<ConsulServiceVm>(), Title = host};
            Hosts.Add(ret);
            return ret;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
