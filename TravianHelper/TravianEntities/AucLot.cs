using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravianHelper.TravianEntities
{
    public class AucLot : BaseTravianEntity
    {
        private int _aucId;

        public int AucId
        {
            get => _aucId;
            set
            {
                _aucId = value;
                RaisePropertyChanged(() => AucId);
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

        private int _timeEnd;

        public int TimeEnd
        {
            get => _timeEnd;
            set
            {
                _timeEnd = value;
                RaisePropertyChanged(() => TimeEnd);
            }
        }

        public AucLot(int aucId, int price, int timeEnd, long time)
        {
            AucId           = aucId;
            Price           = price;
            TimeEnd         = timeEnd;
            UpdateTimeStamp = time;
        }
    }
}
