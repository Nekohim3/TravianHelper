using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.ViewModel;

namespace TravianHelper.UI
{
    public class TabManager : NotificationObject
    {
        private ObservableCollection<Tab> _tabList;

        public ObservableCollection<Tab> TabList
        {
            get => _tabList;
            set
            {
                _tabList = value;
                RaisePropertyChanged(() => TabList);
            }
        }

        private Tab _selectedTab;

        public Tab SelectedTab
        {
            get => _selectedTab;
            set
            {
                _selectedTab = value;
                RaisePropertyChanged(() => SelectedTab);
            }
        }

        public TabManager()
        {
            TabList = new ObservableCollection<Tab>();
            TabList.Add(new Tab(new Account(){Name = "Nekohime"}));
            TabList.Add(new Tab(null, "+"));
        }
    }
}
