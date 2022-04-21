using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravianHelper.TravianEntities
{
    public class Troops : BaseTravianEntity
    {
        public List<Units> Units { get; set; }

        public bool IsMineMerchantTroop => Units.Count == 11 && Units.All(x => x.Count == 0);
        public bool IsIncomingTroop     => Units.Count == 0;
        public bool IsIncomingAttack    => Units.Count == 11 && Units.All(x => x.Count == -1);
        public bool Home                { get; set; }

        public Troops(dynamic d, long time)
        {
            Units = new List<Units>();
            foreach (var x in d)
                Units.Add(new Units(x.Name, x.Value));

            UpdateTime      = DateTime.Now;
            UpdateTimeStamp = time;
        }
    }

    public class Units
    {
        public int Id    { get; set; }
        public int Count { get; set; }

        public Units(dynamic id, dynamic c)
        {
            Id    = Convert.ToInt32(id);
            Count = c;
        }
    }
}
