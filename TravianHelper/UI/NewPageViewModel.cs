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

        private AccountSettingsViewModel _accountSettingsVM;

        public AccountSettingsViewModel AccountSettingsVM
        {
            get => _accountSettingsVM;
            set
            {
                _accountSettingsVM = value;
                RaisePropertyChanged(() => AccountSettingsVM);
            }
        }

        public NewPageViewModel()
        {
            ProxySettingsVM   = new ProxySettingsViewModel(UpdateAll);
            ServerSettingsVM  = new ServerSettingsViewModel(UpdateAll);
            AccountSettingsVM = new AccountSettingsViewModel(UpdateAll);
        }

        public void UpdateAll()
        {
            ProxySettingsVM.Init();
            ServerSettingsVM.Init();
            AccountSettingsVM.Init();
        }
    }
}
