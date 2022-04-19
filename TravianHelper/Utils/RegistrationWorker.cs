using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Practices.Prism.ViewModel;
using OpenQA.Selenium;
using TravianHelper.TravianEntities;

namespace TravianHelper.Utils
{
    public class TaskListWorker : NotificationObject
    {
        private bool _working;

        public bool Working
        {
            get => _working;
            set
            {
                if (Account.UseSingleBuild || Account.UseMultiBuild)
                {
                    if (_working == value)
                        return;
                    _working = value;
                    if (_working)
                        Run();
                    else
                        NotBlockWait = false;
                }
                else
                {
                    if (_working && !value)
                    {
                        _working = false;
                        NotBlockWait = false;
                    }
                }


                RaisePropertyChanged(() => Working);
            }
        }

        private bool _notBlockWait;

        public bool NotBlockWait
        {
            get => _notBlockWait;
            set
            {
                _notBlockWait = value;
                RaisePropertyChanged(() => NotBlockWait);
            }
        }

        private DateTime _lastMultiCheck = DateTime.MinValue;

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

        private Thread _workThread;

        public TaskListWorker(Account acc)
        {
            Account = acc;
            NotBlockWait = true;
        }

        private void Run()
        {
            _workThread = new Thread(ThreadFunc);
            _workThread.Start();
        }

        private void ThreadFunc()
        {
            while (Working)
            {
                
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                NotBlockWait = true;
            });
        }
    }
}
