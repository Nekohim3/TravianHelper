using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.ViewModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TravianHelper.TravianEntities;

namespace TravianHelper.Utils
{
    public class Unit
    {
        public int Id { get; set; }
        public int Count { get; set; }

        public Unit(int id, int count)
        {
            Id = id;
            Count = count;
        }
    }

    public class Field
    {
        public string ResType { get; set; }
        public bool HasVillage { get; set; }
        public bool IsOasis { get; set; }
        public Resource OasisBonus { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public List<Unit> UnitList { get; set; }

        public Field()
        {

        }

        public Field(dynamic data, int x, int y)
        {
            ResType = data.resType;
            HasVillage = data.hasVillage != "0";
            IsOasis = data.isOasis;
            if (IsOasis)
            {
                UnitList = new List<Unit>();
                OasisBonus = new Resource(data.oasisBonus["1"], data.oasisBonus["2"], data.oasisBonus["3"], data.oasisBonus["4"], -1);
                if (data.troops.tribeId.ToString() == "4" && data.troops.status.ToString() == "home")
                {
                    foreach (var v in data.troops.units)
                    {
                        UnitList.Add(new Unit(Convert.ToInt32(v.Name), Convert.ToInt32(v.Value)));
                    }
                }
            }

            X = x;
            Y = y;
        }
    }

    public class MapParser : NotificationObject
    {
        private List<Field> _fieldList;

        public List<Field> FieldList
        {
            get => _fieldList;
            set
            {
                _fieldList = value;
                RaisePropertyChanged(() => FieldList);
            }
        }

        private Account _acc;

        public Account Account
        {
            get => _acc;
            set
            {
                _acc = value;
                RaisePropertyChanged(() => Account);
            }
        }

        private List<Poi> _coordList;

        public List<Poi> CoordList
        {
            get => _coordList;
            set
            {
                _coordList = value;
                RaisePropertyChanged(() => CoordList);
            }
        }

        public MapParser(Account acc)
        {
            Account = acc;
            var r = 60;
            var lst = new List<Poi>();
            CoordList = new List<Poi>();
            for (var i = 0; i < 360; i++)
            {
                var p = new Poi(Convert.ToInt32(r * Math.Cos(i * Math.PI / 180)), Convert.ToInt32(r * Math.Sin(i * Math.PI / 180)));
                if (lst.Count(x => x.X == p.X && x.Y == p.Y) == 0)
                    lst.Add(p);
            }

            for (var i = -r; i <= r; i++)
            {
                for (var j = -r; j <= r; j++)
                {
                    if (InCircle(i, j, r))
                        CoordList.Add(new Poi(i, j));
                }
            }
        }

        public bool InCircle(int x, int y, int r)
        {
            return x * x + y * y < r * r;
        }

        public void Parse()
        {
            //return;
            FieldList = new List<Field>();
            var vids = new List<string>();
            var ii = 0;
            var iii = 0;
            while (ii < CoordList.Count)
            {
                while (vids.Count < 1000 && ii < CoordList.Count)
                {
                    vids.Add(GetMapDetailStringBtPos(CoordList[ii].X, CoordList[ii].Y));
                    ii++;
                }

                var data = Account.Driver.Post(GetCache_MapDetails(Account.Driver.GetSession(), vids.ToArray()), out var err);
                foreach (var x in data.cache)
                {
                    var vidCoord = CoordList[iii];
                    FieldList.Add(new Field(x.data, vidCoord.X, vidCoord.Y));
                    iii++;
                }
                vids.Clear();
                //Thread.Sleep(10000);
            }
            File.WriteAllText("testmap.txt", JsonConvert.SerializeObject(FieldList, Formatting.Indented));
        }

        private string[] GetVillageIds()
        {
            var start = 536887296 - 32768 * 5 - 5;
            var arr = new List<string>();
            for (var i = 0; i < 10; i++)
                for (var j = 0; j < 10; j++)
                {
                    arr.Add($"MapDetails:{start + i * 32768 + j}");
                }
            return arr.ToArray();
        }

        private string GetMapDetailStringBtPos(int x, int y)
        {
            return $"MapDetails:{536887296 + y * 32768 + x}";
        }

        public JObject GetCache_MapDetails(string session, string[] vids) =>
            JObject.FromObject(new
            {
                action = "get",
                controller = "cache",
                session,
                @params = new
                {
                    names = vids
                }
            });

    }
    public class Poi
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Poi(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
