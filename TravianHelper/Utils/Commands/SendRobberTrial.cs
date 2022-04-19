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
    public class GetRobberVidTrial : BaseCommand
    {
        private int _destVid;

        public int DestVid
        {
            get => _destVid;
            set
            {
                _destVid = value;
                RaisePropertyChanged(() => DestVid);
            }
        }
        public GetRobberVidTrial(Account acc) : base(acc)
        {
            Display = "GetRobberVidTrial";
        }

        public override bool Exec(int counterCount = 0)
        {
            var errorMsg = $"[{Account.NameWithNote}]: Error GetRobberVidTrial";
            var counter  = 0;
            while (counter <= counterCount)
            {
                try
                {
                    var newList = new List<int>();
                    var data    = Account.Driver.GetCache_MapDetails(Account.Player.VillageList.First().Id);
                    foreach (var q in data.cache)
                        if (q.data.npcInfo != null)
                            newList.Add(Convert.ToInt32(q.name.ToString().Split(':')[1]));
                    
                    if (newList.Count == 0)
                    {
                        counter++;
                        Logger.Error(errorMsg);
                        Thread.Sleep(5000);
                        continue;
                    }

                    var d1 = Math.Abs(Account.Player.VillageList.First().Id - newList[0]);
                    var d2 = Math.Abs(Account.Player.VillageList.First().Id - newList[1]);
                    DestVid = d1 >= d2 ? newList[0] : newList[1];
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
