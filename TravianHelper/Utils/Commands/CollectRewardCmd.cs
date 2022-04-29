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

        public CollectRewardCmd(Account acc, int vid, int qid, string comment) : base(acc)
        {
            QuestId = qid;
            Vid     = vid;
            Display = string.IsNullOrEmpty(comment) ? $"CollectReward{(QuestId == 0 ? "" : $":{QuestId}")}" : comment;
        }

        public CollectRewardCmd(Account acc) : base(acc)
        {

        }

        public bool Init(string cmd)
        {
            try
            {
                var paramArr = cmd.Split(';');
                var cmdArgs  = paramArr[0].Split(':');
                var comment  = paramArr.Length >= 2 ? paramArr[1] : "";
                if (cmdArgs[0] == "CR")
                {
                    Vid     = Convert.ToInt32(cmdArgs[1]);
                    if (cmdArgs.Length >= 3)
                        QuestId = Convert.ToInt32(cmdArgs[2]);
                    Display = string.IsNullOrEmpty(comment) ? $"CollectReward{(QuestId == 0 ? "" : $":{QuestId}")}" : comment;
                    return true;
                }
                else
                {
                    Logger.Error($"CollectRewardCmd Error parse wrong type");
                    return false;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, $"CollectRewardCmd Error parse");
                return false;
            }
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
                    if (Account.Player.VillageList.Count > Vid)
                    {
                        if (Account.Player.UpdateQuestList())
                        {
                            if (QuestId == 0)
                            {
                                foreach (var x in Account.Player.QuestList.Where(x => x.IsCompleted))
                                    Account.Driver.CollectReward(Account.Player.VillageList[Vid].Id, x.Id);
                                return "Done";
                            }
                            else
                            {
                                if (Account.Player.QuestList.Count(x => x.Id == QuestId) == 0)
                                {
                                    errors  += "Quest not found;";
                                    counter += 3;
                                }
                                else
                                {
                                    Account.Driver.CollectReward(Account.Player.VillageList[Vid].Id, QuestId);
                                    return "Done";
                                }
                            }
                        }
                        else
                        {
                            errors += "Error while get quests;";
                        }
                    }
                    else
                    {
                        Account.Player.UpdateVillageList();
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
