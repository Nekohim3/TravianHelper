using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.ViewModel;
using TravianHelper.TravianEntities;

namespace TravianHelper.Utils.Commands
{
    public abstract class BaseCommand : NotificationObject
    {
        private string _display;

        public string Display
        {
            get => _display;
            set
            {
                _display = value;
                RaisePropertyChanged(() => Display);
            }
        }

        private TaskState _state;

        public TaskState State
        {
            get => _state;
            set
            {
                _state = value;
                RaisePropertyChanged(() => State);
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
            }
        }

        protected BaseCommand(Account acc)
        {
            
        }

        public abstract bool Exec(int counterCount = 0);
    }
}
