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
    public class ResWorker : NotificationObject
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

        public int VilInd { get; set; }

        private Thread _workThread;

        public ResWorker(Account acc)
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
                    Account.Player.UpdateVillageList();
                    var vil = Account.Player.VillageList[VilInd];

                    if (vil != null)
                    {
                        vil.UpdateBuildingQueue();
                        if (vil.Queue.QueueList.Count == 0)
                        {
                            vil.UpdateBuildingList();
                            var buildingResMinLevel = vil.BuildingList.Where(x => x.BuildingType == 1 || x.BuildingType == 2 || x.BuildingType == 3).OrderBy(x => x.Level).FirstOrDefault().Level;// || x.BuildingType == 4 && x.Level < 3).OrderBy(x => x.UpgradeCost.MultiRes).ToList();//поля
                            var buildings =
                                vil.BuildingList.Where(x => x.BuildingType == 1 || x.BuildingType == 2 || x.BuildingType == 3 || (x.BuildingType == 4 && x.Level <= buildingResMinLevel - 2)).OrderBy(x => x.UpgradeCost.MultiRes).ToList();
                            buildings = buildings.Where(x => x.UpgradeCost.IsLess(vil.Storage)).ToList();
                            if (buildings.Count != 0)
                            {
                                var building = buildings.First();
                                if(building.UpgradeCost.IsLess(vil.Capacity))
                                    Account.Driver.BuildingUpgrade(vil.Id, building.Location, building.BuildingType);
                                else
                                {
                                    if (building.UpgradeCost.Crop > vil.Capacity.Crop)
                                    {
                                        var gran = vil.BuildingList.FirstOrDefault(x => x.BuildingType == 11);
                                        if (gran != null && gran.UpgradeCost.IsLess(vil.Storage))
                                            Account.Driver.BuildingUpgrade(vil.Id, gran.Location, gran.BuildingType);
                                    }
                                    else
                                    {
                                        var wh = vil.BuildingList.FirstOrDefault(x => x.BuildingType == 10);
                                        if (wh != null && wh.UpgradeCost.IsLess(vil.Storage))
                                            Account.Driver.BuildingUpgrade(vil.Id, wh.Location, wh.BuildingType);
                                    }
                                }
                            }
                        }
                    }

                    for (var i = 0; i < 1; i++)
                        if (Working)
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
