using Microsoft.Practices.Prism.ViewModel;

namespace TravianHelper.Settings
{
    public class ServerConfig : NotificationObject
    {
        private string _server;

        public string Server
        {
            get => _server;
            set
            {
                _server = value;
                RaisePropertyChanged(() => Server);
            }
        }

        private string _domain;

        public string Domain
        {
            get => _domain;
            set
            {
                _domain = value;
                RaisePropertyChanged(() => Domain);
            }
        }

        private string _region;

        public string Region// com, ru
        {
            get => _region;
            set
            {
                _region = value;
                RaisePropertyChanged(() => Region);
            }
        }
    }
}
