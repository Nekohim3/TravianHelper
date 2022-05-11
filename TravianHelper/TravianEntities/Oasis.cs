using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravianHelper.TravianEntities
{
    public class Oasis : BaseTravianEntity
    {
        private bool _isWild;

        public bool IsWild
        {
            get => _isWild;
            set
            {
                _isWild = value;
                RaisePropertyChanged(() => IsWild);
            }
        }

        private Resource _bonus;

        public Resource Bonus
        {
            get => _bonus;
            set
            {
                _bonus = value;
                RaisePropertyChanged(() => Bonus);
            }
        }

        private int _villageId;

        public int VillageId
        {
            get => _villageId;
            set
            {
                _villageId = value;
                RaisePropertyChanged(() => VillageId);
            }
        }

        private int _usedByVillage;

        public int UsedByVillage
        {
            get => _usedByVillage;
            set
            {
                _usedByVillage = value;
                RaisePropertyChanged(() => UsedByVillage);
            }
        }

        public Oasis(bool isWild, int villageId, int usedByVillage, int w, int cl, int i, int cr)
        {
            IsWild        = isWild;
            Bonus         = new Resource(w, cl, i, cr, -1);
            VillageId     = villageId;
            UsedByVillage = usedByVillage;
        }
    }
}
