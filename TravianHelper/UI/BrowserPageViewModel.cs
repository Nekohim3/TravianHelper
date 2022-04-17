using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using TravianHelper.SeleniumHost;
using TravianHelper.TravianEntities;

namespace TravianHelper.UI
{
    public class BrowserPageViewModel : NotificationObject
    {
        private Account _account;

        public Account Account
        {
            get => _account;
            set
            {
                _account = value;
                RaisePropertyChanged(() => Account);
            }
        }

        public DelegateCommand SettingsCmd { get; }

        public BrowserPageViewModel(Account acc)
        {
            Account = acc;
            SettingsCmd = new DelegateCommand(OnSettings);
        }

        private void OnSettings()
        {
            var f  = new SettingsView();
            var vm = new SettingsViewModel(Account);
            f.DataContext = vm;
            f.ShowDialog();
        }
    }
}
