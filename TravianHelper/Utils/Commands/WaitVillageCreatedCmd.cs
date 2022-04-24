//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using System.Windows;
//using TravianHelper.TravianEntities;

//namespace TravianHelper.Utils.Commands
//{
//    public class WaitVillageCreatedCmd : BaseCommand
//    {
//        public WaitVillageCreatedCmd(Account acc) : base(acc)
//        {
//            Display = "WaitVillageCreated";
//        }

//        public override bool Exec(int counterCount = 0)
//        {
//            var errorMsg = $"[{Account.NameWithNote}]: Error WaitVillageCreatedCmd";
//            var counter  = 0;
//            while (counter <= counterCount)
//            {
//                try
//                {
//                    Account.Player.Update();
//                    Account.Player.UpdateVillageList();
//                    if(Account.Player.VillageList.Count != 1 || Account.Player.VillageList[0].Id < 0)
//                    {
//                        counter++;
//                        Logger.Error(errorMsg);
//                        Thread.Sleep(5000);
//                    }

//                    return true;
//                }
//                catch (Exception e)
//                {
//                    Logger.Error(e, errorMsg);
//                }
//                counter++;
//            }
//            MessageBox.Show(errorMsg);
//            return false;
//        }
//    }
//}
