using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Practices.Prism.ViewModel;
using TravianHelper.TravianEntities;

namespace TravianHelper.Utils
{
    public class RobberWorker : NotificationObject
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

        public RobberWorker(Account acc)
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
            try
            {
                while (Working)
                {
                    var vil = Account.Player.VillageList.First();
                    vil.UpdateStationaryTroops();
                    vil.UpdateMovingTroops();
                    Account.Player.Hero.Update();
                    if ((vil.MovingTroopList.Count == 0 || vil.MovingTroopList.Count == 1) &&
                        vil.StationaryTroopList.Count != 0 && vil.StationaryTroopList.FirstOrDefault(x => x.Units.Count(c => c.Id != 11) != 0)?.Units.Sum(c => c.Count) > 5)
                    {
                        var cellList   = Account.Driver.GetCache_MapDetails(vil.Id);
                        var robberList = new List<int>();
                        if (cellList != null)
                        {
                            foreach (var q in cellList.cache)
                                if (q.data.npcInfo != null)
                                    robberList.Add(Convert.ToInt32(q.name.ToString().Split(':')[1]));

                            if (robberList.Count != 0)
                            {
                                if (!Account.SendHero || Account.SendHero && !Account.Player.Hero.IsMoving)
                                {
                                    var destVid = robberList.First();
                                    Account.Driver.SendTroops(vil.Id, destVid, 3, false, "resources",
                                                              vil.StationaryTroopList[0].Units.Count(x => x.Id == 1) == 0 ? 0 : vil.StationaryTroopList[0].Units.First(x => x.Id == 1).Count,
                                                              vil.StationaryTroopList[0].Units.Count(x => x.Id == 2) == 0 ? 0 : vil.StationaryTroopList[0].Units.First(x => x.Id == 2).Count,
                                                              vil.StationaryTroopList[0].Units.Count(x => x.Id == 3) == 0 ? 0 : vil.StationaryTroopList[0].Units.First(x => x.Id == 3).Count,
                                                              vil.StationaryTroopList[0].Units.Count(x => x.Id == 4) == 0 ? 0 : vil.StationaryTroopList[0].Units.First(x => x.Id == 4).Count,
                                                              vil.StationaryTroopList[0].Units.Count(x => x.Id == 5) == 0 ? 0 : vil.StationaryTroopList[0].Units.First(x => x.Id == 5).Count,
                                                              vil.StationaryTroopList[0].Units.Count(x => x.Id == 6) == 0 ? 0 : vil.StationaryTroopList[0].Units.First(x => x.Id == 6).Count,
                                                              0,
                                                              0,
                                                              0,
                                                              Account.SendSettlers
                                                                  ? vil.StationaryTroopList[0].Units.Count(x => x.Id == 10) == 0 ? 0 : vil.StationaryTroopList[0].Units.First(x => x.Id == 10).Count
                                                                  : 0,
                                                              Account.SendHero ? 1 : 0
                                                             );
                                }
                            }
                        }
                    }

                    for (var i = 0; i < 50 && Working; i++)
                        Thread.Sleep(500);
                }
            }
            catch (Exception e)
            {
                Working = false;

            }

            Application.Current.Dispatcher.Invoke(() =>
                                                  {
                                                      NotBlockWait = true;
                                                  });
        }
    }
}
