using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TravianHelper.Settings;

namespace TravianHelper.TravianEntities
{
    public class Account : BaseTravianEntity
    {
        #region Properties

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

        #region BuildSettings

        private int _fastBuildDelay;

        public int FastBuildDelay
        {
            get => _fastBuildDelay;
            set
            {
                _fastBuildDelay = value;
                RaisePropertyChanged(() => FastBuildDelay);
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

        private int _maxHpForHeal;

        public int MaxHpForHeal
        {
            get => _maxHpForHeal;
            set
            {
                _maxHpForHeal = value;
                RaisePropertyChanged(() => MaxHpForHeal);
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

        //Driver
        //worker threads


        #endregion

        public Account()
        {
            
        }

        public void Start()
        {

        }

        public void Stop()
        {

        }

        public override string ToString()
        {
            return Name;
        }
    }
}
