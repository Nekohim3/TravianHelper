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
    public class RecruitUnitsCmd : BaseCommand
    {
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

        private int _unitId;

        public int UnitId
        {
            get => _unitId;
            set
            {
                _unitId = value;
                RaisePropertyChanged(() => UnitId);
            }
        }

        private int _amount;

        public int Amount
        {
            get => _amount;
            set
            {
                _amount = value;
                RaisePropertyChanged(() => Amount);
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

        public RecruitUnitsCmd(Account acc, int vid, int unitId, int amount, string comment) : base(acc)
        {
            Vid     = vid;
            UnitId  = unitId;
            Amount  = amount;
            Display = string.IsNullOrEmpty(comment) ? $"RecruitUnitsCmd:{unitId}:{amount}" : comment;
        }

        public RecruitUnitsCmd(Account acc) : base(acc)
        {

        }

        public bool Init(string cmd)
        {
            try
            {
                var paramArr = cmd.Split(';');
                var cmdArgs = paramArr[0].Split(':');
                var comment = paramArr.Length >= 2 ? paramArr[1] : "";
                if (cmdArgs[0] == "RU")
                {
                    Vid     = Convert.ToInt32(cmdArgs[1]);
                    UnitId  = Convert.ToInt32(cmdArgs[2]);
                    if (UnitId < 11 || UnitId > 20)
                    {
                        Logger.Error($"RecruitUnitsCmd Error parse wrong UnitId");
                        return false;
                    }
                    Amount  = Convert.ToInt32(cmdArgs[3]);
                    Display = string.IsNullOrEmpty(comment) ? $"RecruitUnitsCmd:{UnitId}:{Amount}" : comment;
                    Loaded  = true;
                    return true;
                }
                else
                {
                    Logger.Error($"RecruitUnitsCmd Error parse wrong type");
                    return false;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, $"RecruitUnitsCmd Error parse");
                return false;
            }
        }

        public override string Exec(int counterCount = 10)
        {
            var errorMsg = $"[{Account.NameWithNote}]: RecruitUnitsCmd ({Vid}, {UnitId}, {Amount})";
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
                        var buildingType = 0;
                        var location     = 0;

                        if (UnitId == 1 || UnitId == 2 || UnitId == 3 || UnitId == 4)
                        {
                            var building = vil.BuildingList.FirstOrDefault(x => x.BuildingType == 19 && !x.IsRuin);
                            if (building == null)
                            {
                                errors += "Не найдена казарма;";
                            }

                            buildingType = 19;
                            location     = building.Location;
                        }
                        else if (UnitId == 5 || UnitId == 6)
                        {
                            var building = vil.BuildingList.FirstOrDefault(x => x.BuildingType == 20 && !x.IsRuin);
                            if (building == null)
                            {
                                errors += "Не найдена конюшня;";
                            }

                            buildingType = 20;
                            location     = building.Location;
                        }
                        else if (UnitId == 7 || UnitId == 8)
                        {
                            var building = vil.BuildingList.FirstOrDefault(x => x.BuildingType == 21 && !x.IsRuin);
                            if (building == null)
                            {
                                errors += "Не найдена мастерская;";
                            }

                            buildingType = 21;
                            location     = building.Location;
                        }
                        else if (UnitId == 9 || UnitId == 10) //вождь
                        {
                            var residence = vil.BuildingList.FirstOrDefault(x => x.BuildingType == 25 && !x.IsRuin);
                            var dvorec    = vil.BuildingList.FirstOrDefault(x => x.BuildingType == 26 && !x.IsRuin);
                            if (residence == null && dvorec == null)
                            {
                                errors += "Не найдена реза или дворец;";
                            } 
                            else if (residence != null)
                            {
                                buildingType = 25;
                                location     = residence.Location;
                            }
                            else
                            {
                                buildingType = 26;
                                location     = dvorec.Location;
                            }
                        }

                        if (buildingType == 0 || location == 0)
                        {
                            errors += "Не найдена постройка;";
                        }
                        else
                        {
                            var unitId = TribesData.GetByTribeId(Account.Player.TribeId).GetUnitId(UnitId);
                            Account.Driver.RecruitUnits(vil.Id, location, buildingType, unitId.ToString(), Amount);

                            var checkCounter = 0;
                            while (checkCounter < 3)
                            {
                                vil.UpdateUnitQueue();
                                var queue = vil.UnitQueue.UnitList.FirstOrDefault(x => x.Id == unitId);
                                if (queue != null)
                                {
                                    if (queue.Count >= Amount)
                                        return "Done";
                                }
                                checkCounter++;
                                Thread.Sleep(500);
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
