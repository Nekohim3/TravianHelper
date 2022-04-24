//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows;
//using TravianHelper.TravianEntities;

//namespace TravianHelper.Utils.Commands
//{
//    public class MapSolveCmd : BaseCommand
//    {
//        public MapSolveCmd(Account acc) : base(acc)
//        {
//            Display = "SolveMap";
//        }

//        public override bool Exec(int counterCount = 0)
//        {
//            var errorMsg = $"[{Account.NameWithNote}]: Error MapSolveCmd";
//            var counter  = 0;
//            while (counter <= counterCount)
//            {
//                try
//                {
//                    new MapSolver().Solve(Account);
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
