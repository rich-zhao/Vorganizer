using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoOrganizer.Service
{
    class LogService : INotifyPropertyChanged
    {
        private string _logText = "";
        public string LogText
        {
            get
            {
                return _logText;
            }
            set
            {
                var line = "--" + value + "\n";
                _logText += line;
                OnPropertyChanged("LogText");
            }
        }

        public void Log(string line)
        {
            LogText = line;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string PropertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
        }
    }
}
