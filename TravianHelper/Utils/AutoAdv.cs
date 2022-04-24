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
    public class AutoAdv : NotificationObject
    {
        private bool _working;

        public bool Working
        {
            get => _working;
            set
            {
                if (_working == value)
                    return;
                _working = value;
                if (_working)
                    Run();
                else
                    NotBlockWait = false;
                
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

        public AutoAdv(Account acc)
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
                if (Account.Running == true && Account.Loaded && Account.RegComplete)
                {
                    Account.Player.Hero.Update();
                    Account.Player.Hero.UpdateItems();
                    if (Account.Player.Hero.FreePoints != 0)
                    {
                        var data = Account.Driver.Post(RPG.HeroAttribute(Account.Driver.GetSession(), 0, 0, 0, Account.Player.Hero.FreePoints, 0), out var error);
                    }

                    if (Account.SellGoods)
                    {
                        var goods = Account.Player.Hero.Items.FirstOrDefault(x => x.ItemType == 120);
                        if (goods != null)
                            Account.Driver.UseHeroItem(goods.Amount, goods.Id, Account.Player.VillageList.First().Id);
                    }

                    if (Account.UseOin)
                    {
                        var maz   = Account.Player.Hero.Items.FirstOrDefault(x => x.ItemType == 112);
                        var mazAm = maz?.Amount;
                        if (mazAm > 0 && Account.Player.Hero.Health <= Account.MinHpForHeal)
                        {
                            Account.Driver.UseHeroItem(Account.HealTo - Account.Player.Hero.Health > mazAm.Value ? mazAm.Value : Account.HealTo - Account.Player.Hero.Health, maz.Id,
                                                       Account.Player.VillageList.FirstOrDefault().Id);
                        }
                    }

                    if (Account.Player.Hero.AdvPoints > 0 && !Account.Player.Hero.IsMoving)
                    {
                        if (Account.Player.Hero.Items.Count(x => x.Horse && x.Slot == 0) != 0)
                        {
                            Account.Driver.UseHeroItem(1, Account.Player.Hero.Items.FirstOrDefault(x => x.Horse).Id, Account.Player.VillageList.FirstOrDefault().Id);
                        }

                        if (Account.Player.Hero.Health >= Account.MinHpForAdv)
                        {
                            var data = Account.Driver.Post(RPG.SendHeroAdv(Account.Driver.GetSession()), out var error);
                        }
                    }

                }

                for(var i = 0; i < 60 && Working; i++)
                    Thread.Sleep(500);
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                NotBlockWait = true;
            });
        }
    }
}
