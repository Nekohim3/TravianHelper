using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using TravianHelper.Settings;
using TravianHelper.TravianEntities;

namespace TravianHelper.UI
{
    public class AccountSettingsViewModel : NotificationObject
    {
        private bool _isEditMode;

        public bool IsEditMode
        {
            get => _isEditMode;
            set
            {
                _isEditMode = value;
                RaisePropertyChanged(() => IsEditMode);
            }
        }

        private string _name;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                RaisePropertyChanged(() => Name);
                if (!CustomMail)
                    Mail = $"{_name}gj@candassociates.com";
            }
        }

        private string _mail;

        public string Mail
        {
            get => _mail;
            set
            {
                _mail = value;
                RaisePropertyChanged(() => Mail);
            }
        }

        private ObservableCollection<Account> _accountList = new ObservableCollection<Account>();

        public ObservableCollection<Account> AccountList
        {
            get => _accountList;
            set
            {
                _accountList = value;
                RaisePropertyChanged(() => AccountList);
            }
        }

        private Account _selectedAccount;

        public Account SelectedAccount
        {
            get => _selectedAccount;
            set
            {
                _selectedAccount = value;
                RaisePropertyChanged(() => SelectedAccount);
                IsEditMode = false;
                RaiseCanExecChanged();
                CurrentAccount = SelectedAccount;
            }
        }

        private Account _currentAccount;

        public Account CurrentAccount
        {
            get => _currentAccount;
            set
            {
                _currentAccount = value;
                RaisePropertyChanged(() => CurrentAccount);
            }
        }

        private bool _customMail;

        public bool CustomMail
        {
            get => _customMail;
            set
            {
                _customMail = value;
                RaisePropertyChanged(() => CustomMail);
                if(!CustomMail)
                    Mail = $"{_name}gj@candassociates.com";
            }
        }

        private ObservableCollection<Proxy> _proxyList = new ObservableCollection<Proxy>();

        public ObservableCollection<Proxy> ProxyList
        {
            get => _proxyList;
            set
            {
                _proxyList = value;
                RaisePropertyChanged(() => ProxyList);
            }
        }

        private Proxy _selectedProxy;

        public Proxy SelectedProxy
        {
            get => _selectedProxy;
            set
            {
                _selectedProxy = value;
                RaisePropertyChanged(() => SelectedProxy);
            }
        }

        private ObservableCollection<ServerConfig> _serverList = new ObservableCollection<ServerConfig>();

        public ObservableCollection<ServerConfig> ServerList
        {
            get => _serverList;
            set
            {
                _serverList = value;
                RaisePropertyChanged(() => ServerList);
            }
        }

        private ServerConfig _selectedServer;

        public ServerConfig SelectedServer
        {
            get => _selectedServer;
            set
            {
                _selectedServer = value;
                RaisePropertyChanged(() => SelectedServer);
            }
        }

        private Action _update;

        public DelegateCommand AddCmd { get; }
        public DelegateCommand EditCmd { get; }
        public DelegateCommand DeleteCmd { get; }
        public DelegateCommand SaveCmd { get; }
        public DelegateCommand CancelCmd { get; }

        public AccountSettingsViewModel(Action update)
        {
            _update   = update;
            AddCmd    = new DelegateCommand(OnAdd);
            EditCmd   = new DelegateCommand(OnEdit,   () => SelectedAccount != null);
            DeleteCmd = new DelegateCommand(OnDelete, () => SelectedAccount != null);
            SaveCmd   = new DelegateCommand(OnSave);
            CancelCmd = new DelegateCommand(OnCancel);
            Init();
        }

        private void RaiseCanExecChanged()
        {
            EditCmd.RaiseCanExecuteChanged();
            DeleteCmd.RaiseCanExecuteChanged();
        }

        public void Init()
        {
            AccountList.Clear();
            AccountList.AddRange(g.Db.GetCollection<Account>().AsQueryable());ProxyList.Clear();
            ProxyList.AddRange(g.Db.GetCollection<Proxy>().AsQueryable());
            ServerList.Clear();
            ServerList.AddRange(g.Db.GetCollection<ServerConfig>().AsQueryable());
        }

        private void OnAdd()
        {
            IsEditMode     = true;
            CurrentAccount = new Account();
            CustomMail     = false;
            Name           = "";
            Mail           = "";
        }

        private void OnEdit()
        {
            IsEditMode = true;
            CurrentAccount = g.Db.GetCollection<Account>().AsQueryable().FirstOrDefault(x => x.Id == SelectedAccount.Id);
            if (CurrentAccount == null)
                IsEditMode = false;
            else
            {
                SelectedProxy  = ProxyList.FirstOrDefault(x => x.Id  == CurrentAccount.ProxyId);
                SelectedServer = ServerList.FirstOrDefault(x => x.Id == CurrentAccount.ServerId);
                CustomMail     = false;
                Name           = CurrentAccount.Name;
                Mail           = CurrentAccount.Email;
            }
        }

        private void OnDelete()
        {
            IsEditMode = true;
            if (MessageBox.Show($"Точно удалить аккаунт {SelectedAccount}?", "", MessageBoxButton.YesNo, MessageBoxImage.Error, MessageBoxResult.No) != MessageBoxResult.Yes) return;
            g.Db.GetCollection<Account>().Delete(SelectedAccount);
            _update?.Invoke();
            IsEditMode = false;
        }

        private void OnSave()
        {
            CurrentAccount.ProxyId  = SelectedProxy?.Id;
            CurrentAccount.ServerId = SelectedServer?.Id;
            CurrentAccount.Email    = Mail;
            CurrentAccount.Name     = Name;
            if (CurrentAccount.Id == 0)
            {
                g.Db.GetCollection<Account>().Insert(CurrentAccount);
            }
            else
            {
                g.Db.GetCollection<Account>().Update(CurrentAccount);
            }
            _update?.Invoke();
            IsEditMode = false;
        }

        private void OnCancel()
        {
            CurrentAccount = SelectedAccount;
            IsEditMode = false;
        }
    }
}
