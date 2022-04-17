using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.ViewModel;
using TravianHelper.Utils;

namespace TravianHelper.TravianEntities
{
    public class Hero : BaseTravianEntity
    {
        private int _advPoints;

        public int AdvPoints
        {
            get => _advPoints;
            set
            {
                _advPoints = value;
                RaisePropertyChanged(() => AdvPoints);
            }
        }

        private int _freePoints;

        public int FreePoints
        {
            get => _freePoints;
            set
            {
                _freePoints = value;
                RaisePropertyChanged(() => FreePoints);
            }
        }

        private int _health;

        public int Health
        {
            get => _health;
            set
            {
                _health = value;
                RaisePropertyChanged(() => Health);
            }
        }

        private bool _isMoving;

        public bool IsMoving
        {
            get => _isMoving;
            set
            {
                _isMoving = value;
                RaisePropertyChanged(() => IsMoving);
            }
        }

        private int _atkPoints;

        public int AtkPoints
        {
            get => _atkPoints;
            set
            {
                _atkPoints = value;
                RaisePropertyChanged(() => AtkPoints);
            }
        }

        private int _resPoints;

        public int ResPoints
        {
            get => _resPoints;
            set
            {
                _resPoints = value;
                RaisePropertyChanged(() => ResPoints);
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

        private ObservableCollection<HeroItem> _items = new ObservableCollection<HeroItem>();

        public ObservableCollection<HeroItem> Items
        {
            get => _items;
            set
            {
                _items = value;
                RaisePropertyChanged(() => Items);
            }
        }

        public bool HasBuildItem => Items.Count(x => x.ItemType == 138) != 0;
        public bool HasNpcItem   => Items.Count(x => x.ItemType == 139) != 0;

        public Hero(Account acc)
        {
            Account = acc;
        }

        public void Update(dynamic data = null, long time = -1)
        {
            Logger.Info($"[{Account.Name}]: Hero update start");
            if (data == null && time == -1)
            {
                Logger.Info($"[{Account.Name}]: Hero update load data");
                Account.Driver.GetCache_Hero(Account.PlayerId);
                return;
            }

            AdvPoints  = data.data.adventurePoints;
            FreePoints = data.data.freePoints;
            Health     = Convert.ToInt32(Convert.ToDouble(data.data.health));
            IsMoving   = data.data.isMoving;
            AtkPoints  = data.data.fightStrengthPoints;
            ResPoints  = data.data.resBonusPoints;

            UpdateTime      = DateTime.Now;
            UpdateTimeStamp = time;

            Logger.Info($"[{Account.Name}]: Hero update SUCC");
        }
        public void UpdateItems(dynamic data = null, long time = -1)
        {
            Logger.Info($"[{Account.Name}]: HeroItems update start");
            if (data == null && time == -1)
            {
                Logger.Info($"[{Account.Name}]: HeroItems update load data");
                Account.Driver.GetCache_CollectionHeroItemOwn();
                return;
            }

            if (data == null) return;

            Items.Clear();

            foreach (var x in data.data.cache)
                Items.Add(new HeroItem(Convert.ToInt32(x.data.amount),
                                       Convert.ToInt32(x.data.id),
                                       Convert.ToInt32(x.data.itemType),
                                       x.data.inSlot,
                                       x.data.images[0].ToString().IndexOf("horse") != -1,
                                       time));

            Logger.Info($"[{Account.Name}]: HeroItems update SUCC");
        }

    }
}
