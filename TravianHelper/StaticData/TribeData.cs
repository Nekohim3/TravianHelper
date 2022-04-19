using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravianHelper.StaticData
{
    public static class TribesData
    {
        public static List<TribeData> TribeList { get; set; }

        static TribesData()
        {
            TribeList = new List<TribeData>
                        {
                            new TribeData() { Id = 1, Name = "Rome" },
                            new TribeData() { Id = 2, Name = "Teuton" },
                            new TribeData() { Id = 3, Name = "Gaul" }
                        };
        }

        public static TribeData GetByTribeId(int tribeId) => TribeList.FirstOrDefault(x => x.Id == tribeId);
    }

    public class TribeData
    {
        public int    Id   { get; set; }
        public string Name { get; set; }

        public int GetUnitId(int uid) => Id == 1 ? uid : Id == 2 ? uid + 10 : uid + 20;
    }
}
