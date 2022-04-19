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
    public class ServerSettingsViewModel : NotificationObject
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
                IsEditMode = false;
                RaiseCanExecChanged();
                CurrentServer = _selectedServer;
            }
        }

        private ServerConfig _currentServer;

        public ServerConfig CurrentServer
        {
            get => _currentServer;
            set
            {
                _currentServer = value;
                RaisePropertyChanged(() => CurrentServer);
            }
        }

        private Action _update;

        public DelegateCommand AddCmd    { get; }
        public DelegateCommand EditCmd   { get; }
        public DelegateCommand DeleteCmd { get; }
        public DelegateCommand SaveCmd   { get; }
        public DelegateCommand CancelCmd { get; }

        public ServerSettingsViewModel(Action update)
        {
            _update   = update;
            AddCmd    = new DelegateCommand(OnAdd);
            EditCmd   = new DelegateCommand(OnEdit,   () => SelectedServer != null);
            DeleteCmd = new DelegateCommand(OnDelete, () => SelectedServer != null);
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
            ServerList.Clear();
            ServerList.AddRange(g.Db.GetCollection<ServerConfig>().AsQueryable());
        }

        private void OnAdd()
        {
            IsEditMode = true;
            CurrentServer = new ServerConfig();
        }

        private void OnEdit()
        {
            IsEditMode = true;
            CurrentServer = g.Db.GetCollection<ServerConfig>().AsQueryable().FirstOrDefault(x => x.Id == SelectedServer.Id);
            if (CurrentServer == null)
            {
                IsEditMode = false;
            }
        }

        private void OnDelete()
        {
            if (MessageBox.Show($"Точно удалить мир {SelectedServer}?", "", MessageBoxButton.YesNo, MessageBoxImage.Error, MessageBoxResult.No) != MessageBoxResult.Yes) return;
            g.Db.GetCollection<ServerConfig>().Delete(SelectedServer);
            _update?.Invoke();
        }

        private void OnSave()
        {
            if (string.IsNullOrEmpty(CurrentServer.Region))
            {
                if(MessageBox.Show("Регион не указан. Авто регистрация не будет работать. Все равно добавить мир?", "", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                    return;
            }
            if (CurrentServer.Id == 0)
            {
                g.Db.GetCollection<ServerConfig>().Insert(CurrentServer);
            }
            else
            {
                g.Db.GetCollection<ServerConfig>().Update(CurrentServer);
            }
            _update?.Invoke();
            IsEditMode = false;
        }

        private void OnCancel()
        {
            IsEditMode = false;
            CurrentServer = SelectedServer;
        }
    }
}
