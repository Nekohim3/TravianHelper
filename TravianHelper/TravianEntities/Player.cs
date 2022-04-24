using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.ViewModel;
using TravianHelper.Utils;

namespace TravianHelper.TravianEntities
{
    public class Player : BaseTravianEntity
    {
        private int _gold;

        public int Gold
        {
            get => _gold;
            set
            {
                _gold = value;
                RaisePropertyChanged(() => Gold);
            }
        }

        private int _id;

        public int Id
        {
            get => _id;
            set
            {
                _id = value;
                RaisePropertyChanged(() => Id);
            }
        }

        private string _name;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                RaisePropertyChanged(() => Name);
            }
        }

        private int _silver;

        public int Silver
        {
            get => _silver;
            set
            {
                _silver = value;
                RaisePropertyChanged(() => Silver);
            }
        }

        private int _tribeId;

        public int TribeId
        {
            get => _tribeId;
            set
            {
                _tribeId = value;
                RaisePropertyChanged(() => TribeId);
            }
        }

        private bool _hasFinishNowFree;

        public bool HasFinishNowFree
        {
            get => _hasFinishNowFree;
            set
            {
                _hasFinishNowFree = value;
                RaisePropertyChanged(() => HasFinishNowFree);
            }
        }

        private Account _account;

        public Account Account
        {
            get => _account;
            set
            {
                _account = value;
                RaisePropertyChanged(() => Account);
            }
        }

        private Hero _hero;

        public Hero Hero
        {
            get => _hero;
            set
            {
                _hero = value;
                RaisePropertyChanged(() => Hero);
            }
        }

        private List<Quest> _questList = new List<Quest>();

        public List<Quest> QuestList
        {
            get => _questList;
            set
            {
                _questList = value;
                RaisePropertyChanged(() => QuestList);
            }
        }

        private List<Village> _villageList = new List<Village>();

        public List<Village> VillageList
        {
            get => _villageList;
            set
            {
                _villageList = value;
                RaisePropertyChanged(() => VillageList);
            }
        }

        public Player(Account acc)
        {
            Account = acc;
            Hero    = new Hero(acc);
        }

        public void Update(dynamic data = null, long time = -1)
        {
            Logger.Info($"[{Account.Name}]: Player update start");
            if (data == null && time == -1)
            {
                Logger.Info($"[{Account.Name}]: Player update load data");
                Account.Driver.GetCache_Player(Account.PlayerId);
                return;
            }

            Name    = data.data.name;
            Id      = data.data.playerId;
            Gold    = data.data.gold;
            Silver  = data.data.silver;
            TribeId = Convert.ToInt32(data.data.tribeId);

            UpdateTime      = DateTime.Now;
            UpdateTimeStamp = time;

            Logger.Info($"[{Account.Name}]: Player update SUCC");
        }

        public bool UpdateQuestList(dynamic data = null, long time = -1)
        {
            Logger.Info($"[{Account.Name}]: QuestList update start");
            if (data == null && time == -1)
            {
                Logger.Info($"[{Account.Name}]: QuestList update load data");
                return Account.Driver.GetCache_Quest();
            }

            QuestList.Clear();
            foreach (var x in data.data.cache)
                QuestList.Add(new Quest(x.data.id, x.data.finalStep, x.data.finishedSteps, x.data.progress, x.data.status, time));

            Logger.Info($"[{Account.Name}]: QuestList update SUCC");
            return true;
        }

        public void UpdateVillageList(dynamic data = null, long time = -1)
        {
            Logger.Info($"[{Account.Name}]: VillageList update start");
            if (data == null && time == -1)
            {
                Logger.Info($"[{Account.Name}]: VillageList update load data");
                Account.Driver.GetCache_VillageList();
                return;
            }

            var idVillageLst = new List<int>();
            foreach (var x in data.data.cache)
            {
                if (VillageList.Count(c => c.Id == (int)x.data.villageId) == 0)
                    VillageList.Add(new Village(Account, x, time));
                else
                    VillageList.First(c => c.Id == (int)x.data.villageId).Update(x, time);
                idVillageLst.Add((int)x.data.villageId);
            }

            foreach (var x in VillageList.ToList().Where(x => !idVillageLst.Contains(x.Id)))
                VillageList.Remove(x);
            VillageList = VillageList.OrderBy(x => x.Name).ToList();
            Logger.Info($"[{Account.Name}]: VillageList update SUCC");
        }


    }
}
