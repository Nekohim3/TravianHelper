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
    public class CollectRewardCmd : BaseCommand
    {
        private int _questId;

        public int QuestId
        {
            get => _questId;
            set
            {
                _questId = value;
                RaisePropertyChanged(() => QuestId);
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

        public CollectRewardCmd(Account acc, int vid, int qid) : base(acc)
        {
            QuestId = qid;
            Vid     = vid;
            Display = $"CollectReward:{qid}";
        }

        public override bool Exec(int counterCount = 0)
        {
            var errorMsg = $"[{Account.NameWithNote}]: CollectRewardCmd ({QuestId})";
            var counter  = 0;
            while (counter <= counterCount)
            {
                try
                {
                    if (QuestId == 0)
                    {
                        Account.Player.UpdateQuestList();
                        foreach (var x in Account.Player.QuestList.Where(x => x.IsCompleted))
                            Account.Driver.CollectReward(Account.Player.VillageList[Vid].Id, x.Id);
                    }
                    else
                    {
                        if (!Account.Driver.CollectReward(Account.Player.VillageList[Vid].Id, QuestId))
                        {
                            counter++;
                            Logger.Error(errorMsg);
                            Thread.Sleep(1000);
                            continue;
                        }
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
