using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.ViewModel;

namespace TravianHelper.UI
{
    public class NewPageViewModel : NotificationObject
    {
        private ProxySettingsViewModel _proxySettingsVM;

        public ProxySettingsViewModel ProxySettingsVM
        {
            get => _proxySettingsVM;
            set
            {
                _proxySettingsVM = value;
                RaisePropertyChanged(() => ProxySettingsVM);
            }
        }

        private ServerSettingsViewModel _serverSettingsVM;

        public ServerSettingsViewModel ServerSettingsVM
        {
            get => _serverSettingsVM;
            set
            {
                _serverSettingsVM = value;
                RaisePropertyChanged(() => ServerSettingsVM);
            }
        }

        public NewPageViewModel()
        {
            ProxySettingsVM  = new ProxySettingsViewModel();
            ServerSettingsVM = new ServerSettingsViewModel();
        }
    }
}
