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
    public class ProxySettingsViewModel : NotificationObject
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
                IsEditMode = false;
                RaiseCanExecChanged();
                CurrentProxy = _selectedProxy;
                UsedAccountList.Clear();
                if (SelectedProxy != null)
                    UsedAccountList.AddRange(g.Db.GetCollection<Account>().AsQueryable().Where(x => x.ProxyId == _selectedProxy.Id));
            }
        }

        private Proxy _currentProxy;

        public Proxy CurrentProxy
        {
            get => _currentProxy;
            set
            {
                _currentProxy = value;
                RaisePropertyChanged(() => CurrentProxy);
            }
        }

        private ObservableCollection<Account> _usedAccountList = new ObservableCollection<Account>();

        public ObservableCollection<Account> UsedAccountList
        {
            get => _usedAccountList;
            set
            {
                _usedAccountList = value;
                RaisePropertyChanged(() => UsedAccountList);
            }
        }

        private Action _update;

        public DelegateCommand AddCmd    { get; }
        public DelegateCommand EditCmd   { get; }
        public DelegateCommand DeleteCmd { get; }
        public DelegateCommand SaveCmd   { get; }
        public DelegateCommand CancelCmd { get; }

        public ProxySettingsViewModel(Action update)
        {
            _update   = update;
            AddCmd    = new DelegateCommand(OnAdd);
            EditCmd   = new DelegateCommand(OnEdit,   () => SelectedProxy != null && g.Db.GetCollection<Account>().AsQueryable().Count(x => x.ProxyId == SelectedProxy.Id) == 0);
            DeleteCmd = new DelegateCommand(OnDelete, () => SelectedProxy != null && g.Db.GetCollection<Account>().AsQueryable().Count(x => x.ProxyId == SelectedProxy.Id) == 0);
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
            ProxyList.Clear();
            ProxyList.AddRange(g.Db.GetCollection<Proxy>().AsQueryable());
        }

        private void OnAdd()
        {
            IsEditMode   = true;
            CurrentProxy = new Proxy();
        }

        private void OnEdit()
        {
            IsEditMode   = true;
            CurrentProxy = g.Db.GetCollection<Proxy>().AsQueryable().FirstOrDefault(x => x.Id == SelectedProxy.Id);
            if (CurrentProxy == null)
                IsEditMode = false;
        }

        private void OnDelete()
        {
            IsEditMode = true;
            if (MessageBox.Show($"Точно удалить прокси {SelectedProxy}?", "", MessageBoxButton.YesNo, MessageBoxImage.Error, MessageBoxResult.No) != MessageBoxResult.Yes) return;
            g.Db.GetCollection<Proxy>().Delete(SelectedProxy);
            _update?.Invoke();
            IsEditMode = false;
        }

        private void OnSave()
        {
            if (string.IsNullOrEmpty(CurrentProxy.Ip))
            {
                MessageBox.Show("Неправильно введен ip");
                return;
            }
            var spip = CurrentProxy.Ip.Split('.');
            if (spip.Length != 4)
            {
                MessageBox.Show("Неправильно введен ip");
                return;
            }

            if (!int.TryParse(spip[0], out var c1) || 
                !int.TryParse(spip[1], out var c2) || 
                !int.TryParse(spip[2], out var c3) || 
                !int.TryParse(spip[3], out var c4) || 
                c1 < 0 || c1 > 255 ||
                c2 < 0 || c2 > 255 ||
                c3 < 0 || c3 > 255 ||
                c4 < 0 || c4 > 255)
            {
                MessageBox.Show("Неправильно введен ip");
                return;
            }
            if (CurrentProxy.Id == 0)
            {
                g.Db.GetCollection<Proxy>().Insert(CurrentProxy);
            }
            else
            {
                g.Db.GetCollection<Proxy>().Update(CurrentProxy);
            }
            _update?.Invoke();
            IsEditMode = false;
        }

        private void OnCancel()
        {
            CurrentProxy = SelectedProxy;
            IsEditMode   = false;
        }
    }
}
