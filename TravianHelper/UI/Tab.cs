using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using TravianHelper.TravianEntities;

namespace TravianHelper.UI
{
    public class Tab : NotificationObject
    {
        private string _tabName;

        public string TabName
        {
            get => _tabName;
            set
            {
                _tabName = value;
                RaisePropertyChanged(() => TabName);
            }
        }

        private Account _account;

        public Account Account
        {
            get => _account;
            set
            {
                _account = value;
                RaisePropertyChanged(() => Account);
                RaisePropertyChanged(() => IsAccount);
            }
        }

        public bool IsAccount => Account != null;

        private Page _page;

        public Page Page
        {
            get => _page;
            set
            {
                _page = value;
                RaisePropertyChanged(() => Page);
            }
        }

        public DelegateCommand CloseCmd { get; }

        public Tab(Account acc = null, string tabName = "")
        {
            Account  = acc;
            TabName  = Account != null ? Account.Name : tabName;
            if (Account == null)
            {
                Page             = new NewPageView();
                Page.DataContext = new NewPageViewModel();
            }
            CloseCmd = new DelegateCommand(OnClose);
        }

        private void OnClose()
        {

        }
    }
}
