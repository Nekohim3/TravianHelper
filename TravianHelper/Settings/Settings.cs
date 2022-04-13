using Microsoft.Practices.Prism.ViewModel;

namespace TravianHelper.Settings
{
    public class Settings : NotificationObject
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
    }
}
