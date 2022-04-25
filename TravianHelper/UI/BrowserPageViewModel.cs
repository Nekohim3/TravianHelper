using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using TravianHelper.SeleniumHost;
using TravianHelper.TravianEntities;
using TravianHelper.Utils.Commands;

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

        private string _command;

        public string Command
        {
            get => _command;
            set
            {
                _command = value;
                RaisePropertyChanged(() => Command);
            }
        }

        public DelegateCommand SettingsCmd    { get; }
        public DelegateCommand TestCommandCmd { get; }

        public BrowserPageViewModel(Account acc)
        {
            Account        = acc;
            SettingsCmd    = new DelegateCommand(OnSettings);
            TestCommandCmd = new DelegateCommand(OnTestCmd);
        }

        private void OnTestCmd()
        {
            var cmdType = Command.Split(':')[0];
            if (cmdType == "BU")
            {
                var cmd = new BuildingUpgradeCmd(Account);
                if (cmd.Init(Command))
                {
                    var reply = cmd.Exec();
                    MessageBox.Show(reply);
                }
                else
                {
                    MessageBox.Show("Ошибка парсинга комманды");
                }
            }

            if (cmdType == "CR")
            {
                var cmd = new CollectRewardCmd(Account);
                if (cmd.Init(Command))
                {
                    var reply = cmd.Exec();
                    MessageBox.Show(reply);
                }
                else
                {
                    MessageBox.Show("Ошибка парсинга комманды");
                }
            }
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
