using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenQA.Selenium.Html5;
using TravianHelper.StaticData;
using TravianHelper.TravianEntities;

namespace TravianHelper.Utils.Commands
{
    public class HeroAttrCmd : BaseCommand
    {
        private int _fightPoint;

        public int FightPoint
        {
            get => _fightPoint;
            set
            {
                _fightPoint = value;
                RaisePropertyChanged(() => FightPoint);
            }
        }
        
        private int _resPoint;

        public int ResPoint
        {
            get => _resPoint;
            set
            {
                _resPoint = value;
                RaisePropertyChanged(() => ResPoint);
            }
        }

        private int _resBonusType;

        public int ResBonusType
        {
            get => _resBonusType;
            set
            {
                _resBonusType = value;
                RaisePropertyChanged(() => ResBonusType);
            }
        }

        public HeroAttrCmd(Account acc, int fightPoint, int resPoint, int resBonusType, string comment) : base(acc)
        {
            FightPoint = fightPoint;
            ResPoint = resPoint;
            ResBonusType = resBonusType;
            Display = string.IsNullOrEmpty(comment) ? $"HeroAttr:{FightPoint}:{ResPoint}:{ResBonusType}" : comment ;
        }

        public HeroAttrCmd(Account acc) : base(acc)
        {
            
        }

        public bool Init(string cmd)
        {
            try
            {
                var paramArr = cmd.Split(';');
                var cmdArgs  = paramArr[0].Split(':');
                var comment  = paramArr.Length >= 2 ? paramArr[1] : "";
                if (cmdArgs[0] == "HA")
                {
                    FightPoint    = Convert.ToInt32(cmdArgs[1]);
                    ResPoint      = Convert.ToInt32(cmdArgs[2]);
                    ResBonusType  = Convert.ToInt32(cmdArgs[3]);
                    Display       = string.IsNullOrEmpty(comment) ? $"HeroAttr:{FightPoint}:{ResPoint}:{ResBonusType}" : comment;
                    Loaded        = true;
                    return true;
                }
                else
                {
                    Logger.Error($"HeroAttrCmd Error parse wrong type");
                    return false;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, $"HeroAttrCmd Error parse");
                return false;
            }
        }

        public override string Exec(int counterCount = 10)
        {
            var errorMsg = $"[{Account.NameWithNote}]: HeroAttrCmd ({FightPoint}, {ResPoint}, {ResBonusType})";
            if (!Loaded)
            {
                return $"{errorMsg} Not loaded";
            }
            var errors  = "";
            var counter = 0;
            var oldf    = -1;
            var oldr    = -1;
            var oldrt   = -1;
            while (counter < counterCount)
            {
                try
                {
                    Account.Player.Hero.Update();
                    oldf  = Account.Player.Hero.AtkPoints;
                    oldr  = Account.Player.Hero.ResPoints;
                    oldrt = Account.Player.Hero.ResBonusType;
                    if (oldrt == -1 || oldr == -1 || oldf == -1)
                    {
                        errors += "Error hero attrs";
                    }
                    else
                    {
                        
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e, $"{Display} Error");
                }
                Thread.Sleep(500);
                counter++;
            }
            return $"{errorMsg}: {errors}";
        }
    }
}
