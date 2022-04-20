using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Practices.Prism.Commands;
using Newtonsoft.Json;
using OpenQA.Selenium;
using SmorcIRL.TempMail;
using TravianHelper.Settings;
using TravianHelper.Utils;
using Proxy = TravianHelper.Settings.Proxy;

namespace TravianHelper.TravianEntities
{
    public class Account : BaseTravianEntity
    {
        #region Properties

        private object _lock = new object();

        private string _name;

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                RaisePropertyChanged(() => Name);
            }
        }

        public string NameWithNote => $"{Name} {(string.IsNullOrEmpty(Note) ? "" : "(")}{Note}{(string.IsNullOrEmpty(Note) ? "" : ")")}";

        private string _email;

        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                RaisePropertyChanged(() => Email);
            }
        }

        private string _refLink;

        public string RefLink
        {
            get => _refLink;
            set
            {
                _refLink = value;
                RaisePropertyChanged(() => RefLink);
            }
        }

        private string _password;

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                RaisePropertyChanged(() => Password);
            }
        }

        private string _note;

        public string Note
        {
            get => _note;
            set
            {
                _note = value;
                RaisePropertyChanged(() => Note);
            }
        }

        private bool _regComplete;

        public bool RegComplete
        {
            get => _regComplete;
            set
            {
                _regComplete = value;
                RaisePropertyChanged(() => RegComplete);
            }
        }

        private int _currentTaskId;

        public int CurrentTaskId
        {
            get => _currentTaskId;
            set
            {
                _currentTaskId = value;
                RaisePropertyChanged(() => CurrentTaskId);
            }
        }

        private Player _player;
        [JsonIgnore]
        public Player Player
        {
            get => _player;
            set
            {
                _player = value;
                RaisePropertyChanged(() => Player);
            }
        }

        [JsonIgnore] public int PlayerId => Driver?.GetPlayerId() ?? -1;

        #region Account settings

        private int? _proxyId;

        public int? ProxyId
        {
            get => _proxyId;
            set
            {
                _proxyId = value;
                RaisePropertyChanged(() => ProxyId);
            }
        }

        private Proxy _proxy;
        [JsonIgnore]
        public Proxy Proxy
        {
            get
            {
                if (_proxy == null && ProxyId.HasValue)
                    _proxy = g.Db.GetCollection<Proxy>().AsQueryable().FirstOrDefault(x => x.Id == ProxyId);
                return _proxy;
            }
            set
            {
                _proxy = value;
                RaisePropertyChanged(() => Proxy);
            }
        }

        private int? _serverId;

        public int? ServerId
        {
            get => _serverId;
            set
            {
                _serverId = value;
                RaisePropertyChanged(() => ServerId);
            }
        }

        private ServerConfig _server;
        [JsonIgnore]
        public ServerConfig Server
        {
            get
            {
                if (_server == null && ServerId.HasValue)
                    _server = g.Db.GetCollection<ServerConfig>().AsQueryable().FirstOrDefault(x => x.Id == ServerId);
                return _server;
            }
            set
            {
                _server = value;
                RaisePropertyChanged(() => Server);
            }
        }

        #endregion

        #region BuildSettings

        private int _fastBuildDelayMin;

        public int FastBuildDelayMin
        {
            get => _fastBuildDelayMin;
            set
            {
                _fastBuildDelayMin = value;
                RaisePropertyChanged(() => FastBuildDelayMin);
            }
        }

        private int _fastBuildDelayMax;

        public int FastBuildDelayMax
        {
            get => _fastBuildDelayMax;
            set
            {
                _fastBuildDelayMax = value;
                RaisePropertyChanged(() => FastBuildDelayMax);
            }
        }

        private bool _useRandomDelay;

        public bool UseRandomDelay
        {
            get => _useRandomDelay;
            set
            {
                _useRandomDelay = value;
                RaisePropertyChanged(() => UseRandomDelay);
            }
        }

        private bool _useSingleBuild;

        public bool UseSingleBuild
        {
            get => _useSingleBuild;
            set
            {
                _useSingleBuild = value;
                RaisePropertyChanged(() => UseSingleBuild);
            }
        }

        private bool _useMultiBuild;

        public bool UseMultiBuild
        {
            get => _useMultiBuild;
            set
            {
                _useMultiBuild = value;
                RaisePropertyChanged(() => UseMultiBuild);
            }
        }

        #endregion

        #region GoodsSettings
        
        private bool _sellGoods;

        public bool SellGoods
        {
            get => _sellGoods;
            set
            {
                _sellGoods = value;
                RaisePropertyChanged(() => SellGoods);
            }
        }

        #endregion

        #region AdventureSettings

        private bool _useOin;

        public bool UseOin
        {
            get => _useOin;
            set
            {
                _useOin = value;
                RaisePropertyChanged(() => UseOin);
            }
        }

        private int _minHpForHeal;

        public int MinHpForHeal
        {
            get => _minHpForHeal;
            set
            {
                _minHpForHeal = value;
                RaisePropertyChanged(() => MinHpForHeal);
            }
        }

        private int _healTo;

        public int HealTo
        {
            get => _healTo;
            set
            {
                _healTo = value;
                RaisePropertyChanged(() => HealTo);
            }
        }

        private int _minHpForAdv;

        public int MinHpForAdv
        {
            get => _minHpForAdv;
            set
            {
                _minHpForAdv = value;
                RaisePropertyChanged(() => MinHpForAdv);
            }
        }

        private bool _autoResurrect;

        public bool AutoResurrect
        {
            get => _autoResurrect;
            set
            {
                _autoResurrect = value;
                RaisePropertyChanged(() => AutoResurrect);
            }
        }
        #endregion

        #region RobberSettings

        private bool _sendSettlers;

        public bool SendSettlers
        {
            get => _sendSettlers;
            set
            {
                _sendSettlers = value;
                RaisePropertyChanged(() => SendSettlers);
            }
        }

        private bool _sendHero;

        public bool SendHero
        {
            get => _sendHero;
            set
            {
                _sendHero = value;
                RaisePropertyChanged(() => SendHero);
            }
        }
        
        #endregion

        #endregion

        #region Ignored Properties

        private bool? _running;
        [JsonIgnore]
        public bool? Running
        {
            get => _running;
            set
            {
                _running = value;
                RaisePropertyChanged(() => Running);
            }
        }

        private bool _loaded;
        [JsonIgnore]
        public bool Loaded
        {
            get => _loaded;
            set
            {
                _loaded = value;
                RaisePropertyChanged(() => Loaded);
            }
        }

        private Driver _driver;
        [JsonIgnore]
        public Driver Driver
        {
            get => _driver;
            set
            {
                _driver = value;
                RaisePropertyChanged(() => Driver);
            }
        }
        //worker threads

        private FastBuildWorker _fastBuildWorker;
        [JsonIgnore]
        public FastBuildWorker FastBuildWorker
        {
            get => _fastBuildWorker;
            set
            {
                _fastBuildWorker = value;
                RaisePropertyChanged(() => FastBuildWorker);
            }
        }

        private RobberWorker _robberWorker;
        [JsonIgnore]
        public RobberWorker RobberWorker
        {
            get => _robberWorker;
            set
            {
                _robberWorker = value;
                RaisePropertyChanged(() => RobberWorker);
            }
        }

        private AutoAdv _autoAdv;
        [JsonIgnore]
        public AutoAdv AutoAdv
        {
            get => _autoAdv;
            set
            {
                _autoAdv = value;
                RaisePropertyChanged(() => AutoAdv);
            }
        }

        private MailClient _mailClient;
        [JsonIgnore]
        public MailClient MailClient
        {
            get => _mailClient;
            set
            {
                _mailClient = value;
                RaisePropertyChanged(() => MailClient);
            }
        }

        private bool _installCurrentTask;
        [JsonIgnore]
        public bool InstallCurrentTask
        {
            get => _installCurrentTask;
            set
            {
                _installCurrentTask = value;
                RaisePropertyChanged(() => InstallCurrentTask);
            }
        }

        private OldTaskListWorker _oldTaskListWorker;
        [JsonIgnore]
        public OldTaskListWorker OldTaskListWorker
        {
            get => _oldTaskListWorker;
            set
            {
                _oldTaskListWorker = value;
                RaisePropertyChanged(() => OldTaskListWorker);
            }
        }

        

        #endregion

        public DelegateCommand RegCmd { get; }

        public Account()
        {
            RegCmd            = new DelegateCommand(OnReg);
            Player            = new Player(this);
            FastBuildWorker   = new FastBuildWorker(this);
            AutoAdv           = new AutoAdv(this);
            RobberWorker      = new RobberWorker(this);
            OldTaskListWorker = new OldTaskListWorker(this);
        }

        private void OnReg()
        {
            Driver.Registration();
        }

        public void Start()
        {
            if(Running.HasValue) return;
            if (Proxy == null && ProxyId.HasValue)
            {
                MessageBox.Show("Не найден прокси!");
                Running = null;
                return;
            }

            if (Server == null)
            {
                MessageBox.Show("Не найден мир!");
                Running = null;
                return;
            }

            Running = false;
            Logger.Info($"[{Name}]: Account start");
            Driver = new Driver();
            if (Player == null) Player = new Player(this);
            ThreadPool.QueueUserWorkItem(s =>
            {
                Driver.Init(this);
                try
                {
                    if (RegComplete || string.IsNullOrEmpty(RefLink))
                        Driver.Chrome.Navigate().GoToUrl($"https://{Server.Server}.{Server.Domain}");
                    else
                        Driver.Chrome.Navigate().GoToUrl(RefLink);
                }
                catch (Exception e)
                {
                    Logger.Error("Браузер не смог загрузить страницу");
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        g.TabManager.CloseTab(g.TabManager.TabList.FirstOrDefault(x => x.IsAccount && x.Account.Id == Id));
                    });
                    return;
                }

                Application.Current.Dispatcher.Invoke(() => {
                                                          Running = true;
                                                      });
                var counter = 0;
                while (Running == true && counter < 5)
                {
                    try
                    {
                        var ele = Driver.Wait(By.Id("villageList"));
                        if (ele != null)
                        {
                            if (!RegComplete)
                            {
                                RegComplete = true;
                                g.Db.GetCollection<Account>().Update(this);
                            }

                            if (RegComplete)
                            {
                                UpdateAll();
                                OldTaskListWorker.Init();
                                Application.Current.Dispatcher.Invoke(() => { Loaded = true; });
                                break;
                            }
                        }

                    }
                    catch (Exception e)
                    {
                        counter++;
                    }
                    Thread.Sleep(1000);
                }
            });
        }

        public void Stop()
        {
            if(!Running.HasValue) return;
            Logger.Info($"[{Name}]: Account stop");
            Application.Current.Dispatcher.Invoke(() =>
            {
                Running                 = false;
                FastBuildWorker.Working = false;
                AutoAdv.Working         = false;
                RobberWorker.Working    = false;

                Driver.Dispose();
                Driver  = null;
                Running = null;
            });
        }


        public bool UpdateAll()
        {
            Driver.GetCache_All();
            Driver.GetCache(new List<string> { "Collection:Quest:", "Collection:HeroItem:own" });
            return true;
        }

        public void Update(dynamic data = null, long time = -1)
        {
            if (data == null) return;
            lock (_lock)
            {
                var playerData = Driver.GetDataArrayByName(data.cache, "Player:");
                if (playerData != null)
                    foreach (var x in playerData)
                        Player.Update(x, time);

                var heroData = Driver.GetDataArrayByName(data.cache, "Hero:");
                if (heroData != null)
                    foreach (var x in heroData)
                        Player.Hero.Update(x, time);

                var questListData = Driver.GetDataArrayByName(data.cache, "Collection:Quest:<>");
                if (questListData != null)
                    foreach (var x in questListData)
                        Player.UpdateQuestList(x, time);

                var villageListData = Driver.GetDataArrayByName(data.cache, "Collection:Village:own");
                if (villageListData != null)
                    foreach (var x in villageListData)
                        Player.UpdateVillageList(x, time);

                var villageData = Driver.GetDataArrayByName(data.cache, "Village:");
                if (villageData != null)
                {
                    foreach (var x in villageData)
                    {
                        var vid = Convert.ToInt32(x.name.ToString().Split(':')[1]);
                        var village = Player.VillageList.FirstOrDefault(c => c.Id == vid);
                        if (village != null)
                            village.Update(x, time);
                        else
                            Player.VillageList.Add(new Village(this, x, time));
                    }
                }

                var buildingListData = Driver.GetDataArrayByName(data.cache, "Collection:Building:");
                if (buildingListData != null)
                {
                    foreach (var x in buildingListData)
                    {
                        var vid = Convert.ToInt32(x.name.ToString().Split(':')[2]);
                        var village = Player.VillageList.FirstOrDefault(c => c.Id == vid);
                        if (village != null)
                            village.UpdateBuildingList(x, time);
                    }
                }

                var buildingData = Driver.GetDataArrayByName(data.cache, "Building:");
                if (buildingData != null)
                {
                    foreach (var x in buildingData)
                    {
                        var vid = Convert.ToInt32(x.data.villageId);
                        var bid = Convert.ToInt32(x.name.ToString().Split(':')[1]);
                        var village = Player.VillageList.FirstOrDefault(c => c.Id == vid);
                        if (village != null)
                        {
                            var building = village.BuildingList.FirstOrDefault(c => c.Id == bid);
                            if (building != null)
                                building.Update(x, time);
                            else
                                village.BuildingList.Add(new Building(this, village, x, time));
                        }
                    }
                }

                var buildingQueueData = Driver.GetDataArrayByName(data.cache, "BuildingQueue:<>");
                if (buildingQueueData != null)
                {
                    foreach (var x in buildingQueueData)
                    {
                        var vid = Convert.ToInt32(x.data.villageId);
                        var village = Player.VillageList.FirstOrDefault(c => c.Id == vid);
                        if (village != null)
                        {
                            village.UpdateBuildingQueue(x, time);
                        }
                    }
                }

                var stationaryTroopsData = Driver.GetDataArrayByName(data.cache, "Collection:Troops:stationary:<>");
                if (stationaryTroopsData != null)
                {
                    foreach (var x in stationaryTroopsData)
                    {
                        var vid = Convert.ToInt32(x.name.ToString().Split(':')[3]);
                        var village = Player.VillageList.FirstOrDefault(c => c.Id == vid);
                        if (village != null)
                        {
                            village.UpdateStationaryTroops(x, time);
                        }
                    }
                }

                var movingTroopsData = Driver.GetDataArrayByName(data.cache, "Collection:Troops:moving:<>");
                if (movingTroopsData != null)
                {
                    foreach (var x in movingTroopsData)
                    {
                        var vid = Convert.ToInt32(x.name.ToString().Split(':')[3]);
                        var village = Player.VillageList.FirstOrDefault(c => c.Id == vid);
                        if (village != null)
                        {
                            village.UpdateMovingTroops(x, time);
                        }
                    }
                }

                var heroItemsData = Driver.GetDataArrayByName(data.cache, "Collection:HeroItem:own");
                if (heroItemsData != null)
                    foreach (var x in heroItemsData)
                        Player.Hero.UpdateItems(x);
            }
        }

        public void Save()
        {
            g.Db.GetCollection<Account>().Update(this);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
