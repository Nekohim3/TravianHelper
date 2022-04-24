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
            Vid = vid;
            Display = $"CollectReward:{qid}";
        }

        public override string Exec(int counterCount = 10)
        {
            var errorMsg = $"[{Account.NameWithNote}]: CollectRewardCmd ({QuestId})";
            var errors   = "";
            var counter  = 0;
            while (counter <= counterCount)
            {
                try
                {
                    if (Account.Player.UpdateQuestList())
                    {
                        foreach (var x in Account.Player.QuestList.Where(x => x.IsCompleted))
                            Account.Driver.CollectReward(Account.Player.VillageList[Vid].Id, x.Id);
                        return "Done";
                    }
                    else
                    {
                        errors += "Error while get quests;";
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e, errorMsg);
                }

                counter++;
            }
            return $"{errorMsg}: {errors}";
        }
    }
}
