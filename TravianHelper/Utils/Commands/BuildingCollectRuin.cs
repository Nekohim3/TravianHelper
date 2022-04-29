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
    public class BuildingCollectRuinCmd : BaseCommand
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

        public BuildingCollectRuinCmd(Account acc, int vid, int buildingType, int location, bool finish, string comment) : base(acc)
        {
            Vid = vid;
            BuildingType = buildingType;
            Location = location;
            Finish = finish;
            Display = string.IsNullOrEmpty(comment) ? $"BuildingCollectRuin:{(buildingType == 0 ? buildingType.ToString() : BuildingsData.GetById(buildingType).Name)}:{location}:{(finish ? $":fin" : "")}" : comment;
        }

        public BuildingCollectRuinCmd(Account acc) : base(acc)
        {

        }

        public bool Init(string cmd)
        {
            try
            {
                var paramArr = cmd.Split(';');
                var cmdArgs = paramArr[0].Split(':');
                var comment = paramArr.Length >= 2 ? paramArr[1] : "";
                if (cmdArgs[0] == "BC")
                {
                    Vid = Convert.ToInt32(cmdArgs[1]);
                    BuildingType = Convert.ToInt32(cmdArgs[2]);
                    Location = Convert.ToInt32(cmdArgs[3]);
                    Finish = cmdArgs.Length == 5 && Convert.ToInt32(cmdArgs[4]) == 1;
                    Display = !string.IsNullOrEmpty(comment)
                                  ? comment
                                  : $"BuildingCollectRuin:{(BuildingType == 0 ? BuildingType.ToString() : BuildingsData.GetById(BuildingType).Name)}:{Location}:{(Finish ? $":fin" : "")}";
                    Loaded = true;
                    return true;
                }
                else
                {
                    Logger.Error($"BuildingCollectRuin Error parse wrong type");
                    return false;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, $"BuildingCollectRuin Error parse");
                return false;
            }
        }

        public override string Exec(int counterCount = 10)
        {
            var errorMsg = $"[{Account.NameWithNote}]: BuildingCollectRuin ({Vid}, {BuildingType}, {Location}, {Finish})";
            if (!Loaded)
            {
                return $"{errorMsg} Not loaded";
            }
            var errors = "";
            var counter = 0;
            while (counter < counterCount)
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
                        var building = vil.BuildingList.FirstOrDefault(x => x.BuildingType == BuildingType && x.IsRuin);
                        if (building == null)
                        {
                            errors += "Building not found;";
                            if (counter >= 3)
                            {
                                return $"Done";//ну не найдены руины ну и хуй с ними
                            }
                        }
                        else
                        {
                            Account.Driver.BuildingUpgrade(vil.Id, building.Location, BuildingType);
                            //throw new Exception("Надо сделать");
                        }

                        if (Finish) //Voucher
                        {
                            vil.UpdateBuildingQueue();
                            if (vil.Queue.QueueList.Count(x => x.QueueId == 5 && (x.FinishTime - vil.Queue.UpdateTimeStamp) > 299) != 0)
                            {
                                if (Account.Player.HasFinishNowFree)
                                {
                                    Account.Driver.FinishNow(vil.Id, 5, -1);
                                }
                                else
                                {
                                    Account.Driver.FinishNow(vil.Id, 5, 1);
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
