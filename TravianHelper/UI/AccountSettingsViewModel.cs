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

        private ObservableCollection<Account> _accountList;

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

        public DelegateCommand AddCmd { get; }
        public DelegateCommand EditCmd { get; }
        public DelegateCommand DeleteCmd { get; }
        public DelegateCommand SaveCmd { get; }
        public DelegateCommand CancelCmd { get; }

        public AccountSettingsViewModel()
        {
            AddCmd = new DelegateCommand(OnAdd);
            EditCmd = new DelegateCommand(OnEdit, () => SelectedAccount != null);
            DeleteCmd = new DelegateCommand(OnDelete, () => SelectedAccount != null);
            SaveCmd = new DelegateCommand(OnSave);
            CancelCmd = new DelegateCommand(OnCancel);
            Init();
        }

        private void RaiseCanExecChanged()
        {
            EditCmd.RaiseCanExecuteChanged();
            DeleteCmd.RaiseCanExecuteChanged();
        }

        public void Init(Account sel = null)
        {
            AccountList.Clear();
            AccountList.AddRange(g.Db.GetCollection<Account>().AsQueryable());
            SelectedAccount = sel == null ? AccountList.FirstOrDefault() : AccountList.FirstOrDefault(x => x.Id == sel.Id);
        }

        private void OnAdd()
        {
            IsEditMode = true;
            SelectedAccount = new Account();
        }

        private void OnEdit()
        {
            IsEditMode = true;
            CurrentAccount = g.Db.GetCollection<Account>().AsQueryable().FirstOrDefault(x => x.Id == SelectedAccount.Id);
            if (CurrentAccount == null)
                IsEditMode = false;
        }

        private void OnDelete()
        {
            IsEditMode = true;
            if (MessageBox.Show($"Точно удалить аккаунт {SelectedAccount}?", "", MessageBoxButton.YesNo, MessageBoxImage.Error, MessageBoxResult.No) != MessageBoxResult.Yes) return;
            g.Db.GetCollection<Account>().Delete(SelectedAccount);
            Init();
            IsEditMode = false;
        }

        private void OnSave()
        {
            if (CurrentAccount.Id == 0)
            {
                g.Db.GetCollection<Account>().Insert(CurrentAccount);
            }
            else
            {
                g.Db.GetCollection<Account>().Update(CurrentAccount);
            }
            Init(SelectedAccount);
            IsEditMode = false;
        }

        private void OnCancel()
        {
            CurrentAccount = SelectedAccount;
            IsEditMode = false;
        }
    }
}
