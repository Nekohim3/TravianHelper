using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravianHelper.TravianEntities
{
    public class HeroItem : BaseTravianEntity
    {
        private int _amount;

        public int Amount
        {
            get => _amount;
            set
            {
                _amount = value;
                RaisePropertyChanged(() => Amount);
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

        private int _itemType;

        public int ItemType
        {
            get => _itemType;
            set
            {
                _itemType = value;
                RaisePropertyChanged(() => ItemType);
            }
        }

        private int _slot;

        public int Slot
        {
            get => _slot;
            set
            {
                _slot = value;
                RaisePropertyChanged(() => Slot);
            }
        }

        private bool _horse;

        public bool Horse
        {
            get => _horse;
            set
            {
                _horse = value;
                RaisePropertyChanged(() => Horse);
            }
        }

        public HeroItem(dynamic amount, dynamic id, dynamic itemType, dynamic slot, dynamic horse, long time)
        {
            Amount   = amount;
            Id       = id;
            ItemType = itemType;
            Slot     = Convert.ToInt32(slot);
            Horse    = horse;

            UpdateTime      = DateTime.Now;
            UpdateTimeStamp = time;
        }
    }
}
