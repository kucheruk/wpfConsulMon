using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using ConsulMon.Properties;

namespace ConsulMon.ViewModels
{
    public class ConsulHostVm : INotifyPropertyChanged
    {
        private string _host;
        private string _title;

        public ConsulHostVm()
        {
            Services = new ObservableCollection<ConsulServiceVm>();
        }

        public string Host
        {
            get { return _host; }
            set
            {
                if (value == _host) return;
                _host = value;
                OnPropertyChanged();
            }
        }

        public string Title
        {
            get { return _title; }
            set
            {
                if (value == _title) return;
                _title = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ConsulServiceVm> Services { get; set; }

        public int Height => 32;

        public event PropertyChangedEventHandler PropertyChanged;

        public ConsulServiceVm GetOrCreateService(string service, string tag)
        {
            var existing = Services.FirstOrDefault(a => a.Name == service && a.Tag == tag);
            if (existing == null)
                Services.Add(existing = new ConsulServiceVm
                {
                    Name = service,
                    Tag = tag
                });
            return existing;
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}