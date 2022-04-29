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

        private bool _stop;

        public bool Stop
        {
            get => _stop;
            set
            {
                _stop = value;
                RaisePropertyChanged(() => Stop);
            }
        }

        public BuildingUpgradeCmd(Account acc, int vid, int buildingType, int location, int toLvl, bool finish, string comment) : base(acc)
        {
            Vid          = vid;
            BuildingType = buildingType;
            Location     = location;
            Finish       = finish;
            ToLvl        = toLvl;
            Display = string.IsNullOrEmpty(comment) ? $"BuildingUpgrade:{(buildingType == 0 ? buildingType.ToString() : BuildingsData.GetById(buildingType).Name)}:{location}:>{toLvl}{(finish ? $":fin" : "")}" : comment;
        }

        public BuildingUpgradeCmd(Account acc) : base(acc)
        {
            
        }

        public bool Init(string cmd)
        {
            try
            {
                var paramArr = cmd.Split(';');
                var cmdArgs  = paramArr[0].Split(':');
                var comment  = paramArr.Length >= 2 ? paramArr[1] : "";
                if (cmdArgs[0] == "BU")
                {
                    Vid          = Convert.ToInt32(cmdArgs[1]);
                    BuildingType = Convert.ToInt32(cmdArgs[2]);
                    Location     = Convert.ToInt32(cmdArgs[3]);
                    ToLvl        = Convert.ToInt32(cmdArgs[4]);
                    Finish       = cmdArgs.Length == 6 && Convert.ToInt32(cmdArgs[5]) == 1;
                    Display = !string.IsNullOrEmpty(comment)
                                  ? comment
                                  : $"BuildingUpgrade:{(BuildingType == 0 ? BuildingType.ToString() : BuildingsData.GetById(BuildingType).Name)}:{Location}:>{ToLvl}{(Finish ? $":fin" : "")}";
                    Loaded = true;
                    return true;
                }
                else
                {
                    Logger.Error($"BuildingUpgradeCmd Error parse wrong type");
                    return false;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, $"BuildingUpgradeCmd Error parse");
                return false;
            }
        }

        public override string Exec(int counterCount = 10)
        {
            var errorMsg = $"[{Account.NameWithNote}]: BuildingUpgradeCmd ({Vid}, {BuildingType}, {Location}, {ToLvl}, {Finish})";
            if (!Loaded)
            {
                return $"{errorMsg} Not loaded";
            }
            var errors   = "";
            var counter  = 0;
            while(counter < counterCount)
            {
                try
                {
                    if (Account.Player.VillageList.Count > Vid)
                    {
                        var vil = Account.Player.VillageList[Vid];
                        vil.UpdateVillageAndBuildingListAndQueueAndVoucher(vil.Id);
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
                                if (Location != 0)
                                {
                                    var b = vil.BuildingList.FirstOrDefault(x => x.Location == Location);
                                    if (b != null)
                                    {
                                        if (b.BuildingType == 0)
                                        {
                                            //Строить
                                            while (!vil.Storage.IsGreaterOrEq(BuildingsData.GetById(BuildingType).BuildRes))
                                            {
                                                for (var i = 0; i < 10; i++)
                                                {
                                                    Thread.Sleep(500);
                                                    if (Stop)
                                                    {
                                                        return $"{errorMsg} Canceled while wait for  enough resources";
                                                    }
                                                }
                                                
                                                Account.Player.UpdateQuestList();
                                                foreach (var x in Account.Player.QuestList.ToList().Where(x => x.IsCompleted))
                                                {
                                                    Account.Driver.CollectReward(Account.Player.VillageList.First().Id, x.Id);
                                                }
                                                vil.Update();
                                            }
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
                                    var r  = new Random();
                                    Location = r.Next(vil.BuildingList.Where(x => x.BuildingType == 0).Min(x => x.Location), vil.BuildingList.Where(x => x.BuildingType == 0).Max(x => x.Location));
                                    Account.Driver.BuildingUpgrade(vil.Id, Location, BuildingType);
                                }
                                
                            }
                            else
                            {
                                var r = new Random();
                                Location = r.Next(vil.BuildingList.Where(x => x.BuildingType == 0).Min(x => x.Location), vil.BuildingList.Where(x => x.BuildingType == 0).Max(x => x.Location));
                                Account.Driver.BuildingUpgrade(vil.Id, Location, BuildingType);
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
                                    while (!vil.Storage.IsGreaterOrEq(building.UpgradeCost))
                                    {
                                        for (var i = 0; i < 10; i++)
                                        {
                                            Thread.Sleep(500);
                                            if (Stop)
                                            {
                                                return $"{errorMsg} Canceled while wait for  enough resources";
                                            }
                                        }

                                        Account.Player.UpdateQuestList();
                                        foreach (var x in Account.Player.QuestList.ToList().Where(x => x.IsCompleted))
                                        {
                                            Account.Driver.CollectReward(Account.Player.VillageList.First().Id, x.Id);
                                        }
                                        vil.Update();
                                    }
                                    Account.Driver.BuildingUpgrade(vil.Id, building.Location, BuildingType);
                                }
                            }
                            else //building.Level + 1 > ToLvl
                            {
                                return "Done";
                            }
                        }


                        if (Finish) //Voucher
                        {
                            vil.UpdateBuildingQueue();
                            if (vil.Queue.QueueList.Count(x => x.QueueId == (BuildingType > 4 ? 1 : 2) && (x.FinishTime - vil.Queue.UpdateTimeStamp) > 299) != 0)
                            {
                                if (Account.Player.HasFinishNowFree)
                                {
                                    Account.Driver.FinishNow(vil.Id, BuildingType > 4 ? 1 : 2, -1);
                                }
                                else
                                {
                                    Account.Driver.FinishNow(vil.Id, BuildingType > 4 ? 1 : 2, 1);
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
