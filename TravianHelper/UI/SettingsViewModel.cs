using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.ViewModel;
using TravianHelper.TravianEntities;

namespace TravianHelper.UI
{
    public class SettingsViewModel : NotificationObject
    {
        private Account _account;

        public Account Account
        {
            get => _account;
            set
            {
                _account = value;
                RaisePropertyChanged(() => Account);
            }
        }

        #region BuildSettings

        public int FastBuildDelayMin
        {
            get => Account.FastBuildDelayMin;
            set
            {
                Account.FastBuildDelayMin = value;
                RaisePropertyChanged(() => FastBuildDelayMin);
                Account.Save();
            }
        }

        public int FastBuildDelayMax
        {
            get => Account.FastBuildDelayMax;
            set
            {
                Account.FastBuildDelayMax = value;
                RaisePropertyChanged(() => FastBuildDelayMax);
                Account.Save();
            }
        }

        public bool RandomDelay
        {
            get => Account.UseRandomDelay;
            set
            {
                Account.UseRandomDelay = value;
                RaisePropertyChanged(() => RandomDelay);
                Account.Save();
            }
        }

        public bool UseSingleBuild
        {
            get => Account.UseSingleBuild;
            set
            {
                Account.UseSingleBuild = value;
                RaisePropertyChanged(() => UseSingleBuild);
                Account.Save();
            }
        }
        

        public bool UseMultiBuild
        {
            get => Account.UseMultiBuild;
            set
            {
                Account.UseMultiBuild = value;
                RaisePropertyChanged(() => UseMultiBuild);
                Account.Save();
            }
        }

        #endregion

        #region AdventureSettings

        public bool SellGoods
        {
            get => Account.SellGoods;
            set
            {
                Account.SellGoods = value;
                RaisePropertyChanged(() => SellGoods);
                Account.Save();
            }
        }

        public bool UseOin
        {
            get => Account.UseOin;
            set
            {
                Account.UseOin = value;
                RaisePropertyChanged(() => UseOin);
                Account.Save();
            }
        }

        public int MinHpForHeal
        {
            get => Account.MinHpForHeal;
            set
            {
                Account.MinHpForHeal = value;
                RaisePropertyChanged(() => MinHpForHeal);
                Account.Save();
            }
        }

        public int HealTo
        {
            get => Account.HealTo;
            set
            {
                Account.HealTo = value;
                RaisePropertyChanged(() => HealTo);
                Account.Save();
            }
        }

        public int MinHpForAdv
        {
            get => Account.MinHpForAdv;
            set
            {
                Account.MinHpForAdv = value;
                RaisePropertyChanged(() => MinHpForAdv);
                Account.Save();
            }
        }

        public bool AutoResurrect
        {
            get => Account.AutoResurrect;
            set
            {
                Account.AutoResurrect = value;
                RaisePropertyChanged(() => AutoResurrect);
                Account.Save();
            }
        }
        #endregion

        #region RobberSettings

        public bool SendSettlers
        {
            get => Account.SendSettlers;
            set
            {
                Account.SendSettlers = value;
                RaisePropertyChanged(() => SendSettlers);
                Account.Save();
            }
        }

        public bool SendHero
        {
            get => Account.SendHero;
            set
            {
                Account.SendHero = value;
                RaisePropertyChanged(() => SendHero);
                Account.Save();
            }
        }

        #endregion

        private string _title;

        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                RaisePropertyChanged(() => Title);
            }
        }

        public SettingsViewModel(Account acc)
        {
            Account = acc;
            Title   = $"Настройки для {acc.Name}";
        }
    }
}
