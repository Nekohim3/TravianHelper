using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using TravianHelper.TravianEntities;

namespace TravianHelper.Utils.Commands
{
    public class SendTroopsCmd : BaseCommand
    {
        public int    SourceVilId  { get; set; }
        public int    DestVilId    { get; set; }
        public int    MoveType     { get; set; }
        public bool   RedeployHero { get; set; }
        public string SpyMission   { get; set; }
        public int    C1           { get; set; }
        public int    C2           { get; set; }
        public int    C3           { get; set; }
        public int    C4           { get; set; }
        public int    C5           { get; set; }
        public int    C6           { get; set; }
        public int    C7           { get; set; }
        public int    C8           { get; set; }
        public int    C9           { get; set; }
        public int    C10          { get; set; }
        public int    C11          { get; set; }
        public bool   Reg          { get; set; }

        public SendTroopsCmd(Account acc, int svid, int dvid, int mt, bool rh, string sm, int c1, int c2, int c3, int c4, int c5, int c6, int c7, int c8, int c9, int c10, int c11, bool reg = false) : base(acc)
        {
            SourceVilId  = svid;
            DestVilId    = dvid;
            MoveType     = mt;
            RedeployHero = rh;
            SpyMission   = sm;
            C1           = c1;
            C2           = c2;
            C3           = c3;
            C4           = c4;
            C5           = c5;
            C6           = c6;
            C7           = c7;
            C8           = c8;
            C9           = c9;
            C10          = c10;
            C11          = c11;
            Reg          = reg;
            Display      = $"SendTroops";
        }

        public override bool Exec(int counterCount = 0)
        {
            var errorMsg = $"[{Account.NameWithNote}]: Error SendTroopsCmd";
            var counter  = 0;
            while (counter <= counterCount)
            {
                try
                {
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

                    if (!Account.Driver.SendTroops(vil.Id, DestVilId, MoveType, RedeployHero, SpyMission, C1, C2, C3, C4, C5, C6, C7, C8, C9, C10, C11))
                    {
                        counter++;
                        Logger.Error($"{errorMsg} SendTroops");
                        Thread.Sleep(3000);
                        continue;
                    }

                    return true;
                }
                catch (Exception e)
                {
                    Logger.Error(e, errorMsg);
                }
                counter++;
            }
            MessageBox.Show(errorMsg);
            return false;
        }
    }
}
