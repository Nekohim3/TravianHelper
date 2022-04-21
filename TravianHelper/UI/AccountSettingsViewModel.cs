using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using TravianHelper.Settings;
using TravianHelper.TravianEntities;
using MessageBox = System.Windows.MessageBox;

namespace TravianHelper.UI
{
    public class AccountSettingsViewModel : NotificationObject
    {
        private char[] _chars = new[]
                               {
                                   '0',
                                   '1',
                                   '2',
                                   '3',
                                   '4',
                                   '5',
                                   '6',
                                   '7',
                                   '8',
                                   '9',
                                   'q',
                                   'w',
                                   'e',
                                   'r',
                                   't',
                                   'y',
                                   'u',
                                   'i',
                                   'o',
                                   'p',
                                   'a',
                                   's',
                                   'd',
                                   'f',
                                   'g',
                                   'h',
                                   'j',
                                   'k',
                                   'l',
                                   'z',
                                   'x',
                                   'c',
                                   'v',
                                   'b',
                                   'n',
                                   'm'
                               };
        private bool     _isEditMode;

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
                {
                    var r = new Random(_name.Sum(x => x));
                    Mail = $"{(_name.Length > 4 ? _name.Substring(0, 4) : _name)}{_chars[r.Next() % _chars.Length]}{_chars[r.Next() % _chars.Length]}{_chars[r.Next() % _chars.Length]}@{g.Domain}".ToLower();
                }
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

        public DelegateCommand RunCmd { get; }
        public DelegateCommand RunAndSwitchCmd { get; }

        public AccountSettingsViewModel(Action update)
        {
            _update   = update;
            AddCmd    = new DelegateCommand(OnAdd, () => ServerList.Count > 0);
            EditCmd   = new DelegateCommand(OnEdit,   () => SelectedAccount != null && (!g.TabManager.TabList.FirstOrDefault(x => x.IsAccount && x.Account.Id == SelectedAccount.Id)?.Account.Running.HasValue ?? true));
            DeleteCmd = new DelegateCommand(OnDelete, () => SelectedAccount != null && (!g.TabManager.TabList.FirstOrDefault(x => x.IsAccount && x.Account.Id == SelectedAccount.Id)?.Account.Running.HasValue ?? true));
            SaveCmd   = new DelegateCommand(OnSave);
            CancelCmd = new DelegateCommand(OnCancel);
            RunCmd = new DelegateCommand(OnRun, () => SelectedAccount != null && (!g.TabManager.TabList.FirstOrDefault(x => x.IsAccount && x.Account.Id == SelectedAccount.Id)?.Account.Running.HasValue ?? true));
            RunAndSwitchCmd = new DelegateCommand(OnRunAndSwitch, () => SelectedAccount != null && (!g.TabManager.TabList.FirstOrDefault(x => x.IsAccount && x.Account.Id == SelectedAccount.Id)?.Account.Running.HasValue ?? true));

            Init();
        }

        private void RaiseCanExecChanged()
        {
            AddCmd.RaiseCanExecuteChanged();
            EditCmd.RaiseCanExecuteChanged();
            DeleteCmd.RaiseCanExecuteChanged();
            RunCmd.RaiseCanExecuteChanged();
            RunAndSwitchCmd.RaiseCanExecuteChanged();
        }

        public void Init()
        {
            if(g.TabManager == null) return;
            AccountList.Clear();
            var accList = g.Db.GetCollection<Account>().AsQueryable().ToList();
            for (var i = 0; i < accList.Count; i++)
            {
                var acc = g.TabManager.TabList.FirstOrDefault(c => c.IsAccount && c.Account.Id == accList[i].Id);
                if (acc != null)
                    accList[i] = acc.Account;
            }

            AccountList.AddRange(accList);
            ProxyList.Clear();
            ProxyList.Add(new Proxy(-1, "Не использовать", 0, "", "", ""));
            ProxyList.AddRange(g.Db.GetCollection<Proxy>().AsQueryable());
            SelectedProxy = ProxyList.FirstOrDefault();
            ServerList.Clear();
            ServerList.AddRange(g.Db.GetCollection<ServerConfig>().AsQueryable());
            SelectedServer = ServerList.FirstOrDefault();
            RaiseCanExecChanged();
        }

        private void OnRun()
        {
            g.TabManager.OpenTab(SelectedAccount);
            RaiseCanExecChanged();
        }

        private void OnRunAndSwitch()
        {
            g.TabManager.OpenTab(SelectedAccount, true);
            RaiseCanExecChanged();
        }

        private void OnAdd()
        {
            IsEditMode     = true;
            CurrentAccount = new Account();
            CustomMail     = false;
            Name           = "";
            Mail           = "";

            CurrentAccount.Password = AccountList.LastOrDefault()?.Password;
            CurrentAccount.RefLink  = AccountList.LastOrDefault()?.RefLink;
        }

        private void OnEdit()
        {
            IsEditMode = true;
            CurrentAccount = g.Db.GetCollection<Account>().AsQueryable().FirstOrDefault(x => x.Id == SelectedAccount.Id);
            if (CurrentAccount == null)
                IsEditMode = false;
            else
            {
                SelectedProxy  = ProxyList.FirstOrDefault(x => x.Id == CurrentAccount.ProxyId) ?? ProxyList.FirstOrDefault();
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
            if (Name.Length < 5 || Name.Length > 8)
            {
                MessageBox.Show("В имени аккаунта допускается от 5 до 8 символов");
                return;
            }
            CurrentAccount.ProxyId  = SelectedProxy?.Id > 0 ? SelectedProxy?.Id : null;
            CurrentAccount.ServerId = SelectedServer?.Id;
            CurrentAccount.Email    = Mail;
            CurrentAccount.Name     = Name;
            var lst = AccountList.ToList();
            if (CurrentAccount.Id != 0)
                lst = lst.Where(x => x.Id != CurrentAccount.Id).ToList();
            if (lst.Count(x => x.Name == CurrentAccount.Name) != 0)
            {
                MessageBox.Show("Аккаунт с таким именем уже существует");
                return;
            }
            if (lst.Count(x => x.Email == CurrentAccount.Email) != 0)
            {
                MessageBox.Show("Аккаунт с таким мылом уже существует");
                return;
            }

            if (CurrentAccount.Id == 0)
            {
                CurrentAccount.FastBuildDelayMin = 5;
                CurrentAccount.FastBuildDelayMax = 15;
                CurrentAccount.UseRandomDelay    = true;
                CurrentAccount.UseSingleBuild    = true;
                CurrentAccount.UseMultiBuild     = false;

                CurrentAccount.SellGoods     = true;
                CurrentAccount.UseOin        = true;
                CurrentAccount.MinHpForHeal  = 30;
                CurrentAccount.HealTo        = 100;
                CurrentAccount.MinHpForAdv   = 15;
                CurrentAccount.AutoResurrect = false;

                CurrentAccount.SendSettlers  = true;
                CurrentAccount.SendHero      = false;
                
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
