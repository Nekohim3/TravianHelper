using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.ViewModel;
using TravianHelper.TravianEntities;

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
                if (SelectedTab != null)
                    if (!SelectedTab.IsAccount)
                        ((NewPageViewModel)SelectedTab.Page.DataContext).UpdateAll();
            }
        }

        public TabManager()
        {
            TabList = new ObservableCollection<Tab>();
            TabList.Add(new Tab(null, "+"));
            SelectedTab = TabList.FirstOrDefault();
        }

        public void OpenTab(Account acc)
        {
            TabList.Insert(TabList.Count - 1, new Tab(acc));
            SelectedTab = TabList.FirstOrDefault(x => x.Account == acc);
            acc.Start();
        }

        public void CloseTab(Tab tab)
        {
            if (!tab.IsAccount) return;
            var ind = TabList.IndexOf(tab);
            TabList.Remove(tab);
            SelectedTab = ind == 0 ? TabList[0] : TabList[ind - 1];
            tab.Account.Stop();
        }
    }
}
