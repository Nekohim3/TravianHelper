//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Windows;
//using Newtonsoft.Json.Linq;
//using TravianHelper.TravianEntities;

//namespace TravianHelper.Utils.Commands
//{
//    public class SetSettingsCmd : BaseCommand
//    {
//        public SetSettingsCmd(Account acc) : base(acc)
//        {
//            Display = "SetSettings";
//        }

//        public override bool Exec(int counterCount = 0)
//        {
//            var errorMsg = $"[{Account.NameWithNote}]: ErrorSetSettings";
//            var counter  = 0;
//            while (counter <= counterCount)
//            {
//                try
//                {
//                    var data = Account.Driver.Post(JObject.Parse(
//                                         "{\"controller\":\"player\",\"action\":\"changeSettings\",\"params\":{\"newSettings\":{\"premiumConfirmation\":3,\"lang\":\"ru\",\"onlineStatusFilter\":2,\"extendedSimulator\":false,\"musicVolume\":0,\"soundVolume\":0,\"uiSoundVolume\":50,\"muteAll\":true,\"timeZone\":\"3.0\",\"timeFormat\":0,\"attacksFilter\":2,\"mapFilter\":123,\"enableTabNotifications\":true,\"disableAnimations\":true,\"enableHelpNotifications\":true,\"enableWelcomeScreen\":true,\"notpadsVisible\":false}},\"session\":\"" +
//                                         Account.Driver.GetSession() + "\"}"), out var error);
//                    if (data == null) throw new Exception($"{errorMsg} Null data");
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
