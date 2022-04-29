using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravianHelper.TravianEntities
{
    public class UnitQueue : BaseTravianEntity
    {
        public List<Units> UnitList = new List<Units>();

        public UnitQueue(dynamic d, dynamic time)
        {
            UnitList.Clear();
            foreach (var x in d.buildingTypes)
            {
                foreach (var c in x)
                {
                    foreach (var v in c)
                    {
                        UnitList.Add(new Units(v.unitType, v.count));
                    }
                }
            }

            try
            {
                UpdateTimeStamp = time;
                UpdateTime = DateTime.Now;
            }
            catch (Exception e)
            {
                
            }
        }
    }

    
}
