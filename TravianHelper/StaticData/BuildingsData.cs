using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravianHelper.TravianEntities;

namespace TravianHelper.StaticData
{

    public static class BuildingsData
    {
        public static List<BuildingData> BuildingList { get; set; }
        public static List<BuildingData> InnerBuildingList { get; set; }
        public static List<BuildingData> OuterBuildingList { get; set; }

        static BuildingsData()
        {
            BuildingList = new List<BuildingData>();
            BuildingList.Add(new BuildingData(1, new Resource(-1, -1, -1, -1, -1), true, AdditionalRestriction.None, "Лесопилка"));
            BuildingList.Add(new BuildingData(2, new Resource(-1, -1, -1, -1, -1), true, AdditionalRestriction.None, "Карьер"));
            BuildingList.Add(new BuildingData(3, new Resource(-1, -1, -1, -1, -1), true, AdditionalRestriction.None, "Рудник"));
            BuildingList.Add(new BuildingData(4, new Resource(-1, -1, -1, -1, -1), true, AdditionalRestriction.None, "Пшено"));
            BuildingList.Add(new BuildingData(5, new Resource(520, 380, 290, 90, -1), true, AdditionalRestriction.None, "Лесопильный завод", (15, 5), (1, 10)));
            BuildingList.Add(new BuildingData(6, new Resource(440, 480, 320, 50, -1), true, AdditionalRestriction.None, "Кирпичный завод", (15, 5), (2, 10)));
            BuildingList.Add(new BuildingData(7, new Resource(200, 450, 510, 120, -1), true, AdditionalRestriction.None, "Литейная", (15, 5), (3, 10)));
            BuildingList.Add(new BuildingData(8, new Resource(500, 440, 380, 1240, -1), true, AdditionalRestriction.None, "Мельница", (4, 5)));
            BuildingList.Add(new BuildingData(9, new Resource(1200, 1480, 870, 1600, -1), true, AdditionalRestriction.None, "Пекарня", (15, 5), (4, 10), (8, 5)));
            BuildingList.Add(new BuildingData(10, new Resource(140, 180, 100, 0, -1), true, AdditionalRestriction.None, "Склад", (15, 1)));
            BuildingList.Add(new BuildingData(11, new Resource(80, 100, 70, 20, -1), true, AdditionalRestriction.None, "Амбар", (15, 1)));
            BuildingList.Add(new BuildingData(13, new Resource(180, 250, 500, 160, -1), true, AdditionalRestriction.None, "Кузница", (15, 3), (22, 1)));
            BuildingList.Add(new BuildingData(14, new Resource(1750, 2250, 1530, 240, -1), true, AdditionalRestriction.None, "Арена", (16, 15)));
            BuildingList.Add(new BuildingData(15, new Resource(70, 40, 60, 20, -1), true, AdditionalRestriction.None, "Главное здание"));
            BuildingList.Add(new BuildingData(16, new Resource(110, 160, 90, 70, -1), true, AdditionalRestriction.None, "Пункт сбора"));
            BuildingList.Add(new BuildingData(17, new Resource(80, 70, 120, 70, -1), true, AdditionalRestriction.None, "Рынок", (15, 3), (10, 1)));
            BuildingList.Add(new BuildingData(18, new Resource(180, 130, 150, 80, -1), true, AdditionalRestriction.None, "Посольство", (15, 1)));
            BuildingList.Add(new BuildingData(19, new Resource(210, 140, 260, 120, -1), true, AdditionalRestriction.None, "Казарма", (15, 3), (16, 1)));
            BuildingList.Add(new BuildingData(20, new Resource(260, 140, 220, 100, -1), true, AdditionalRestriction.None, "Конюшня", (13, 3), (22, 5)));
            BuildingList.Add(new BuildingData(21, new Resource(460, 510, 600, 320, -1), true, AdditionalRestriction.None, "Мастерская", (15, 5), (22, 10)));
            BuildingList.Add(new BuildingData(22, new Resource(220, 160, 90, 40, -1), true, AdditionalRestriction.None, "Академия", (15, 3), (19, 3)));
            BuildingList.Add(new BuildingData(23, new Resource(40, 50, 30, 10, -1), true, AdditionalRestriction.None, "Тайник"));
            BuildingList.Add(new BuildingData(24, new Resource(1250, 1110, 1260, 600, -1), true, AdditionalRestriction.None, "Ратуша", (15, 10), (22, 10)));
            BuildingList.Add(new BuildingData(25, new Resource(580, 450, 350, 180, -1), false, AdditionalRestriction.None, "Резиденция", (26, 0), (15, 5)));
            BuildingList.Add(new BuildingData(26, new Resource(550, 800, 750, 250, -1), false, AdditionalRestriction.NotWorldWonderVillage, "Дворец", (25, 0), (15, 5), (18, 1)));
            BuildingList.Add(new BuildingData(27, new Resource(720, 685, 645, 250, -1), true, AdditionalRestriction.None, "Сокровищница"));
            BuildingList.Add(new BuildingData(28, new Resource(1400, 1330, 1200, 400, -1), true, AdditionalRestriction.None, "Торговая палата", (17, 20), (20, 10)));
            BuildingList.Add(new BuildingData(29, new Resource(630, 420, 780, 360, -1), true, AdditionalRestriction.City, "Большая казарма", (19, 20)));
            BuildingList.Add(new BuildingData(30, new Resource(780, 420, 660, 300, -1), true, AdditionalRestriction.City, "Большая конюшня", (20, 20)));
            BuildingList.Add(new BuildingData(31, new Resource(70, 90, 170, 70, -1), true, AdditionalRestriction.None, "Cтена"));
            BuildingList.Add(new BuildingData(32, new Resource(120, 200, 0, 80, -1), true, AdditionalRestriction.None, "Земляной вал"));
            BuildingList.Add(new BuildingData(33, new Resource(160, 100, 80, 60, -1), true, AdditionalRestriction.None, "Изгородь"));
            BuildingList.Add(new BuildingData(34, new Resource(155, 130, 125, 70, -1), true, AdditionalRestriction.Capital, "Каменотес", (15, 5)));
            BuildingList.Add(new BuildingData(35, new Resource(1460, 930, 1250, 1740, -1), true, AdditionalRestriction.Capital, "Пивоварня", (11, 20), (16, 10)));
            BuildingList.Add(new BuildingData(36, new Resource(80, 120, 70, 90, -1), true, AdditionalRestriction.None, "Капканщик", (16, 1)));
            BuildingList.Add(new BuildingData(38, new Resource(650, 800, 450, 200, -1), true, AdditionalRestriction.WorldWonderVillage, "Большой склад", (15, 10)));
            BuildingList.Add(new BuildingData(39, new Resource(400, 500, 350, 100, -1), true, AdditionalRestriction.WorldWonderVillage, "Большой амбар", (15, 10)));
            BuildingList.Add(new BuildingData(40, new Resource(66700, 69050, 72200, 13200, -1), true, AdditionalRestriction.WorldWonderVillage, "Чудо света"));
            BuildingList.Add(new BuildingData(41, new Resource(780, 420, 660, 540, -1), true, AdditionalRestriction.None, "Водопой", (16, 10), (20, 20)));
            BuildingList.Add(new BuildingData(42, new Resource(740, 850, 960, 620, -1), true, AdditionalRestriction.Capital, "Ров"));
            BuildingList.Add(new BuildingData(43, new Resource(120, 200, 0, 80, -1), true, AdditionalRestriction.None, "Натарская стена"));
            BuildingList.Add(new BuildingData(45, new Resource(720, 685, 645, 250, -1), true, AdditionalRestriction.NotWorldWonderVillage, "Скрытая сокровищница", (27, 0), (15, 3)));

            InnerBuildingList = BuildingList.Where(x => x.Id > 4).ToList();
            OuterBuildingList = BuildingList.Where(x => x.Id <= 4).ToList();
        }

        public static BuildingData GetById(int id) => BuildingList.FirstOrDefault(x => x.Id == id);

        public static List<(int id, int lvl)> GetAllDep(int id)
        {
            var b = GetById(id);
            if (b == null) return null;
            var lst = new List<(int id, int lvl)>();
            foreach (var x in b.Dependencies)
            {
                lst.Add(x);
                GetAllDep(x.id, lst);
            }

            var list = new List<(int id, int lvl)>();

            foreach (var x in lst)
            {
                var item = list.FirstOrDefault(c => c.id == x.id);

                if (item == default(ValueTuple<int, int>))
                    list.Add(x);
                else if (x.lvl > item.lvl)
                {
                    for (var i = 0; i < list.Count; i++)
                    {
                        if (list[i].id != x.id) continue;
                        list[i] = (x.id, x.lvl);
                    }
                }
            }

            return list;
        }

        private static void GetAllDep(int id, List<(int id, int lvl)> lst)
        {
            var b = GetById(id);
            if (b == null) return;
            foreach (var x in b.Dependencies)
            {
                lst.Add(x);
                GetAllDep(x.id, lst);
            }
        }
    }

    public enum AdditionalRestriction
    {
        None,
        City,
        Capital,
        WorldWonderVillage,
        NotWorldWonderVillage
    }
    public class BuildingData
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Resource BuildRes { get; set; }
        public bool CanBuildIm { get; set; }
        public AdditionalRestriction AddRest { get; set; }
        public List<(int id, int lvl)> Dependencies { get; set; }

        public BuildingData(int id, Resource res, bool canBuildIm, AdditionalRestriction rest, string name, params (int id, int lvl)[] args)
        {
            Id = id;
            Name = name;
            CanBuildIm = canBuildIm;
            AddRest = rest;
            BuildRes = res;
            Dependencies = args.ToList();
        }
    }
}
