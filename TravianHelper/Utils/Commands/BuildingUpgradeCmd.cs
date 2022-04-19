using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravianHelper.StaticData;
using TravianHelper.TravianEntities;

namespace TravianHelper.Utils.Commands
{
    public class BuildingUpgradeCmd : BaseCommand
    {
        private int _buildingType;

        public int BuildingType
        {
            get => _buildingType;
            set
            {
                _buildingType = value;
                RaisePropertyChanged(() => BuildingType);
            }
        }

        private int _location;

        public int Location
        {
            get => _location;
            set
            {
                _location = value;
                RaisePropertyChanged(() => Location);
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

        private bool _finish;

        public bool Finish
        {
            get => _finish;
            set
            {
                _finish = value;
                RaisePropertyChanged(() => Finish);
            }
        }

        private int _price;

        public int Price
        {
            get => _price;
            set
            {
                _price = value;
                RaisePropertyChanged(() => Price);
            }
        }

        public BuildingUpgradeCmd(Account acc, int vid, int buildingType, int location, bool finish, int price) : base(acc)
        {
            Vid          = vid;
            BuildingType = buildingType;
            Location     = location;
            Finish       = finish;
            Price        = price;
            Display      = $"BuildingUpgrade:{(buildingType == 0 ? buildingType.ToString() : BuildingsData.GetById(buildingType).Name)}:{location}{(finish ? $":fin{(price != 0 ? "$" : "")}" : "")}";
        }

        public override bool Exec(int counterCount = 0)
        {
            return false;
        }
    }
}
