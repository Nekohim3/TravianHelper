using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.ViewModel;

namespace TravianHelper
{
    public class Account : NotificationObject
    {
        private string _name;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                RaisePropertyChanged(() => Name);
            }
        }

        private bool _running;

        public bool Running
        {
            get => _running;
            set
            {
                _running = value;
                RaisePropertyChanged(() => Running);
            }
        }
    }
    public class MainWindowViewModel : NotificationObject
    {
        private ObservableCollection<Account> _accList;

        public ObservableCollection<Account> AccList
        {
            get => _accList;
            set
            {
                _accList = value;
                RaisePropertyChanged(() => AccList);
            }
        }

        private Account _selected;

        public Account Selected
        {
            get => _selected;
            set
            {
                _selected = value;
                RaisePropertyChanged(() => Selected);
            }
        }
        public MainWindowViewModel()
        {
            AccList = new ObservableCollection<Account>();
            AccList.Add(new Account() { Name = "12345678", Running = true });
            AccList.Add(new Account() { Name = "qweasdzx", Running = false });
            AccList.Add(new Account() { Name = "bhgfop", Running = true });
        }
    }
}
