using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using TravianHelper.StaticData;
using TravianHelper.TravianEntities;

namespace TravianHelper.Utils.Commands
{
    public class SendTroopsCmd : BaseCommand
    {
        public int SourceVilId { get; set; }
        public int DestVilId { get; set; }
        public int MoveType { get; set; } //47 осада, 5 подкреп, 3 атака
        public bool RedeployHero { get; set; }
        public string SpyMission { get; set; }
        public int C1 { get; set; }
        public int C2 { get; set; }
        public int C3 { get; set; }
        public int C4 { get; set; }
        public int C5 { get; set; }
        public int C6 { get; set; }
        public int C7 { get; set; }
        public int C8 { get; set; }
        public int C9 { get; set; }
        public int C10 { get; set; }
        public int C11 { get; set; }
        public bool Reg { get; set; }

        public DateTime SendTime { get; set; }
        public bool     Working  { get; set; }

        public SendTroopsCmd(Account acc, int svid, int dvid, int mt, bool rh, string sm, int c1, int c2, int c3, int c4, int c5, int c6, int c7, int c8, int c9, int c10, int c11, bool reg = false) : base(acc)
        {
            SourceVilId = svid;
            DestVilId = dvid;
            MoveType = mt;
            RedeployHero = rh;
            SpyMission = sm;
            C1 = c1;
            C2 = c2;
            C3 = c3;
            C4 = c4;
            C5 = c5;
            C6 = c6;
            C7 = c7;
            C8 = c8;
            C9 = c9;
            C10 = c10;
            C11 = c11;
            Reg = reg;
            Display = $"SendTroops";
        }

        public SendTroopsCmd(Account acc): base (acc)
        {
            
        }

        public bool Init(string cmd)
        {
            try
            {
                var paramArr = cmd.Split(';');
                var cmdArgs  = paramArr[0].Split(':');
                var comment  = paramArr.Length >= 2 ? paramArr[1] : "";
                if (cmdArgs[0] == "ST")
                {
                    SourceVilId  = Convert.ToInt32(cmdArgs[1]);
                    DestVilId    = Convert.ToInt32(cmdArgs[2]);
                    MoveType     = Convert.ToInt32(cmdArgs[3]);
                    RedeployHero = Convert.ToInt32(cmdArgs[4]) == 1;
                    SpyMission   = cmdArgs[5];
                    C1           = Convert.ToInt32(cmdArgs[6]);
                    C2           = Convert.ToInt32(cmdArgs[7]);
                    C3           = Convert.ToInt32(cmdArgs[8]);
                    C4           = Convert.ToInt32(cmdArgs[9]);
                    C5           = Convert.ToInt32(cmdArgs[10]);
                    C6           = Convert.ToInt32(cmdArgs[11]);
                    C7           = Convert.ToInt32(cmdArgs[12]);
                    C8           = Convert.ToInt32(cmdArgs[13]);
                    C9           = Convert.ToInt32(cmdArgs[14]);
                    C10           = Convert.ToInt32(cmdArgs[15]);
                    C11           = Convert.ToInt32(cmdArgs[16]);
                    SendTime = DateTime.Parse($"{cmdArgs[17]}:{cmdArgs[18]}:{cmdArgs[19]}");
                    //Display = !string.IsNullOrEmpty(comment)
                    //              ? comment
                    //              : $"BuildingUpgrade:{(BuildingType == 0 ? BuildingType.ToString() : BuildingsData.GetById(BuildingType).Name)}:{Location}:>{ToLvl}{(Finish ? $":fin" : "")}";
                    Loaded = true;
                    return true;
                }
                else
                {
                    Logger.Error($"SendTroopsCmd Error parse wrong type");
                    return false;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, $"SendTroopsCmd Error parse");
                return false;
            }
        }

        public override string Exec(int counterCount = 10)
        {
            Working = true;
            var errorMsg = $"[{Account.NameWithNote}]: Error SendTroopsCmd";
            var counter = 0;
            while (counter <= counterCount && Working)
            {
                try
                {
                    if (DateTime.Now < SendTime)
                    {
                        Thread.Sleep(5000);
                        continue;
                    }

                    if (Account.Player.VillageList.Count - 1 < SourceVilId)
                    {
                        Account.Player.Update();
                        Account.Player.UpdateVillageList();
                    }

                    if (Account.Player.VillageList.Count - 1 < SourceVilId)
                    {
                        counter++;
                        Logger.Error($"{errorMsg} Vils count < SourceVilId");
                        Thread.Sleep(3000);
                        continue;
                    }

                    var vil = Account.Player.VillageList[SourceVilId];
                    if (vil.Id < 0 && !Reg)
                    {
                        counter++;
                        Logger.Error($"{errorMsg} Vil id < 0 !Reg");
                        Thread.Sleep(3000);
                        continue;
                    }
                    vil.UpdateStationaryTroops();
                    var troop = vil.StationaryTroopList.FirstOrDefault(x => x.Home);
                    if (troop != null)
                    {
                        var res = Account.Driver.SendTroops(vil.Id, DestVilId, MoveType, RedeployHero, SpyMission,
                                                  C1 == -1 ? 0 : C1 == 0 ? troop.Units.FirstOrDefault(x => x.Id == 1)?.Count ?? 0 : C1,
                                                  C2 == -1 ? 0 : C2 == 0 ? troop.Units.FirstOrDefault(x => x.Id == 2)?.Count ?? 0 : C2,
                                                  C3 == -1 ? 0 : C3 == 0 ? troop.Units.FirstOrDefault(x => x.Id == 3)?.Count ?? 0 : C3,
                                                  C4 == -1 ? 0 : C4 == 0 ? troop.Units.FirstOrDefault(x => x.Id == 4)?.Count ?? 0 : C4,
                                                  C5 == -1 ? 0 : C5 == 0 ? troop.Units.FirstOrDefault(x => x.Id == 5)?.Count ?? 0 : C5,
                                                  C6 == -1 ? 0 : C6 == 0 ? troop.Units.FirstOrDefault(x => x.Id == 6)?.Count ?? 0 : C6,
                                                  C7 == -1 ? 0 : C7 == 0 ? troop.Units.FirstOrDefault(x => x.Id == 7)?.Count ?? 0 : C7,
                                                  C8 == -1 ? 0 : C8 == 0 ? troop.Units.FirstOrDefault(x => x.Id == 8)?.Count ?? 0 : C8,
                                                  C9 == -1 ? 0 : C9 == 0 ? troop.Units.FirstOrDefault(x => x.Id == 9)?.Count ?? 0 : C9,
                                                  C10 == -1 ? 0 :C10 == 0 ? troop.Units.FirstOrDefault(x => x.Id == 10)?.Count ?? 0 : C10,
                                                  C11 == -1 ? 0 : C11 == 0 ? troop.Units.FirstOrDefault(x => x.Id == 11)?.Count ?? 0 : C11);
                        if (res)
                        {
                            return "Done";
                        }
                        else
                        {
                            counter++;
                            Logger.Error($"{errorMsg} ST error");
                            Thread.Sleep(3000);
                            continue;
                        }
                    }
                    else
                    {
                        counter++;
                        Logger.Error($"{errorMsg} cant find home troop");
                        Thread.Sleep(3000);
                        continue;
                    }
                    //if (!Account.Driver.SendTroops(vil.Id, DestVilId, MoveType, RedeployHero, SpyMission, C1, C2, C3, C4, C5, C6, C7, C8, C9, C10, C11))
                    //{
                    //    counter++;
                    //    Logger.Error($"{errorMsg} SendTroops");
                    //    Thread.Sleep(3000);
                    //    continue;
                    //}

                    //return true;
                }
                catch (Exception e)
                {
                    Logger.Error(e, errorMsg);
                }
                counter++;
            }
            MessageBox.Show(errorMsg);
            return "";
        }
    }
}
