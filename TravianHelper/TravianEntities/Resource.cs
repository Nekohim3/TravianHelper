using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravianHelper.TravianEntities
{
    public class Resource : BaseTravianEntity
    {
        private int _wood;

        public int Wood
        {
            get => _wood;
            set
            {
                _wood = value;
                RaisePropertyChanged(() => Wood);
                RaisePropertyChanged(() => MultiRes);
            }
        }

        private int _clay;

        public int Clay
        {
            get => _clay;
            set
            {
                _clay = value;
                RaisePropertyChanged(() => Clay);
                RaisePropertyChanged(() => MultiRes);
            }
        }

        private int _iron;

        public int Iron
        {
            get => _iron;
            set
            {
                _iron = value;
                RaisePropertyChanged(() => Iron);
                RaisePropertyChanged(() => MultiRes);
            }
        }

        private int _crop;

        public int Crop
        {
            get => _crop;
            set
            {
                _crop = value;
                RaisePropertyChanged(() => Crop);
                RaisePropertyChanged(() => MultiRes);
            }
        }

        public int MultiRes => Wood + Clay + Iron + Crop;

        public Resource()
        {

        }

        public Resource(dynamic wood, dynamic clay, dynamic iron, dynamic crop, dynamic time)
        {
            Update(wood, clay, iron, crop, time);
        }

        public void Update(dynamic wood, dynamic clay, dynamic iron, dynamic crop, dynamic time)
        {
            Wood = wood;
            Clay = clay;
            Iron = iron;
            Crop = crop;

            UpdateTimeStamp = time;
            UpdateTime = DateTime.Now;
        }

        public bool IsGreaterOrEq(Resource res) => Wood >= res.Wood && Clay >= res.Clay && Iron >= res.Iron && Crop >= res.Crop;

        public bool IsLess(Resource res) => !IsGreaterOrEq(res);

        public Resource AddProduction(Resource production, int min = 5)
        {
            return new Resource()
            {
                Wood = Wood + (production.Wood / 60 * min),
                Clay = Clay + (production.Clay / 60 * min),
                Iron = Iron + (production.Iron / 60 * min),
                Crop = Crop + (production.Crop / 60 * min),
            };
        }

        public Resource Npc(Resource need)
        {
            return new Resource()
            {
                Wood = need.Wood + (MultiRes - need.MultiRes) / 4,
                Clay = need.Clay + (MultiRes - need.MultiRes) / 4,
                Iron = need.Iron + (MultiRes - need.MultiRes) / 4,
                Crop = need.Crop + (MultiRes - need.MultiRes) / 4,
            };
        }

        public override string ToString()
        {
            return $"{Wood}:{Clay}:{Iron}:{Crop}|{MultiRes}";
        }
    }
}
