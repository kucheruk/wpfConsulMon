using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Media;
using Consul;
using ConsulMon.Models;
using ConsulMon.Properties;

namespace ConsulMon.ViewModels
{
    public class ConsulServiceVm : INotifyPropertyChanged
    {
        private string _name;
        private Color _background;
        private string _statusText;
        private string _tag;
        private Color _foreground;

        public string Name
        {
            get { return _name; }
            set
            {
                if (value == _name) return;
                _name = value;
                OnPropertyChanged();
            }
        }

        public string Tag
        {
            get { return _tag; }
            set
            {
                if (value == _tag) return;
                _tag = value;
                OnPropertyChanged();
            }
        }

        public Color Background
        {
            get { return _background; }
            set
            {
                if (value.Equals(_background)) return;
                _background = value;
                OnPropertyChanged();
            }
        }

        public string StatusText
        {
            get { return _statusText; }
            set
            {
                if (value == _statusText) return;
                _statusText = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void SetStatus(ConsulServiceStatus st)
        {
            if (st.Statuses.Length > 0 && st.Statuses.All(a => Equals(a, HealthStatus.Passing)))
            {
                Background = Colors.Chartreuse;
                Foreground = Color.FromRgb(33, 33, 33);
            }
            else if (st.Statuses.Length == 0 || st.Statuses.Any(a => Equals(a, HealthStatus.Critical)))
            {
                Background = Colors.Red;
                Foreground = Colors.White;
            }
            else
            {
                Background = Colors.Orange;
                Foreground = Color.FromRgb(33, 33, 33);
            }
            StatusText = FormatStatusText(st);
        }

        public Color Foreground
        {
            get { return _foreground; }
            set
            {
                if (value.Equals(_foreground)) return;
                _foreground = value;
                OnPropertyChanged();
            }
        }

        public int Height => 32;

        private string FormatStatusText(ConsulServiceStatus st)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{st.Statuses.Length} checks");
            sb.AppendLine($"{st.Statuses.Count(a => Equals(a, HealthStatus.Passing))} passing");
            foreach (
                var entry in
                st.Details.Response.SelectMany(a => a.Checks).Where(c => !Equals(c.Status, HealthStatus.Passing)))
                sb.AppendLine($"{entry.CheckID} status = {entry.Status.Status}");
            return sb.ToString();
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}