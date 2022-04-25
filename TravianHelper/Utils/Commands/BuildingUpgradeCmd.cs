using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TravianHelper.StaticData;
using TravianHelper.TravianEntities;

namespace TravianHelper.Utils.Commands
{
    public class BuildingUpgradeCmd : BaseCommand
    {
        private int _buildingType;

        public int BuildingType
        {
            get => _buildingType;
            set
            {
                _buildingType = value;
                RaisePropertyChanged(() => BuildingType);
            }
        }

        private int _location;

        public int Location
        {
            get => _location;
            set
            {
                _location = value;
                RaisePropertyChanged(() => Location);
            }
        }

        private int _vid;

        public int Vid
        {
            get => _vid;
            set
            {
                _vid = value;
                RaisePropertyChanged(() => Vid);
            }
        }

        private bool _finish;

        public bool Finish
        {
            get => _finish;
            set
            {
                _finish = value;
                RaisePropertyChanged(() => Finish);
            }
        }

        private int _toLvl;

        public int ToLvl
        {
            get => _toLvl;
            set
            {
                _toLvl = value;
                RaisePropertyChanged(() => ToLvl);
            }
        }

        public BuildingUpgradeCmd(Account acc, int vid, int buildingType, int location, int toLvl, bool finish) : base(acc)
        {
            Vid          = vid;
            BuildingType = buildingType;
            Location     = location;
            Finish       = finish;
            ToLvl        = toLvl;
            Display      = $"BuildingUpgrade:{(buildingType == 0 ? buildingType.ToString() : BuildingsData.GetById(buildingType).Name)}:{location}:>{toLvl}{(finish ? $":fin" : "")}";
        }

        public override string Exec(int counterCount = 10)
        {
            var errorMsg = $"[{Account.NameWithNote}]: CollectRewardCmd ({Vid}, {BuildingType}, {Location}, {ToLvl}, {Finish})";
            var errors   = "";
            var counter  = 0;
            while(counter < counterCount)
            {
                try
                {
                    if (Account.Player.VillageList.Count >= Vid)
                    {
                        var vil = Account.Player.VillageList[Vid];
                        vil.UpdateBuildingListAndQueueAndVoucher();
                        if (vil.Queue.QueueList.Count != 0)
                        {
                            Thread.Sleep(1000);
                            continue;
                        }
                        var building = vil.BuildingList.FirstOrDefault(x => x.BuildingType == BuildingType && !x.IsRuin);
                        if (building == null)
                        {
                            if (ToLvl == 1)
                            {
                                var b = vil.BuildingList.FirstOrDefault(x => x.Location == Location);
                                if (b != null)
                                {
                                    if (b.BuildingType == 0)
                                    {
                                        //Строить
                                        Account.Driver.BuildingUpgrade(vil.Id, Location, BuildingType);
                                    }
                                    else
                                    {
                                        errors += "Location busy;";
                                        if (counter >= 2)
                                        {
                                            return errors;
                                        }
                                    }
                                }
                                else
                                {
                                    //не должно суда дойти
                                }
                            }
                            else
                            {
                                errors += "Building not found;";
                                if (counter >= 2)
                                {
                                    return $"{errorMsg}: {errors}";
                                }
                            }
                        }
                        else
                        {
                            if (building.Level == ToLvl)//11 11
                                return "Done";
                            if (building.Level + 1 <= ToLvl)//11+1 12>
                            {
                                if (building.IsUpgraded)
                                {
                                    //улучшается
                                }
                                else
                                {
                                    //не улучшается (надо улучшить или не прошел пакет)
                                    Account.Driver.BuildingUpgrade(vil.Id, Location, BuildingType);
                                }
                            }
                            else //building.Level + 1 > ToLvl
                            {
                                return "Done";
                            }
                        }

                        if (Finish)//Voucher
                        {
                            if (vil.Queue.QueueList.Count(x => x.QueueId == (Location > 18 ? 1 : 2)) != 0)
                            {
                                if (Account.Player.HasFinishNowFree)
                                {
                                    Account.Driver.FinishNow(vil.Id, Location > 18 ? 1 : 2, -1);
                                }
                                else
                                {
                                    Account.Driver.FinishNow(vil.Id, Location > 18 ? 1 : 2, 1);
                                }
                            }
                        }
                    }
                    else
                    {
                        Account.Player.UpdateVillageList();
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e, $"{Display} Error");
                }
                Thread.Sleep(500);
                counter++;
            }
            return $"{errorMsg}: {errors}";
        }
    }
}
