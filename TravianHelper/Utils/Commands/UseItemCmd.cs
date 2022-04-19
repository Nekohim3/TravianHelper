using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using TravianHelper.TravianEntities;

namespace TravianHelper.Utils.Commands
{
    public class UseItemCmd : BaseCommand
    {
        private int _itemType;

        public int ItemType
        {
            get => _itemType;
            set
            {
                _itemType = value;
                RaisePropertyChanged(() => ItemType);
            }
        }

        private int _amount;

        public int Amount
        {
            get => _amount;
            set
            {
                _amount = value;
                RaisePropertyChanged(() => Amount);
            }
        }

        private int _vid;

        public int Vid
        {
            get => _vid;
            set
            {
                _vid = value;
                RaisePropertyChanged(() => Vid);
            }
        }

        public UseItemCmd(Account acc, int vid, int itemType, int amount) : base(acc)
        {
            Display = "UseItem";
            ItemType = itemType;
            Amount = amount;
            Vid = vid;
        }

        public override bool Exec(int counterCount = 0)
        {
            var errorMsg = $"[{Account.NameWithNote}]: Error UseItemCmd ({ItemType}, {Amount})";
            var counter  = 0;
            while (counter <= counterCount)
            {
                try
                {
                    Account.Player.Hero.Update();
                    Account.Player.Hero.UpdateItems();
                    var id = Account.Player.Hero.Items.FirstOrDefault(x => x.ItemType == ItemType)?.Id ?? -1;
                    if (id == -1)
                    {
                        counter++;
                        Logger.Error(errorMsg);
                        Thread.Sleep(3000);
                        continue;
                    }

                    if (!Account.Driver.UseHeroItem(Amount, id, Account.Player.VillageList[id].Id))
                    {
                        counter++;
                        Logger.Error(errorMsg);
                        Thread.Sleep(3000);
                        continue;
                    }

                    return true;
                }
                catch (Exception e)
                {
                    Logger.Error(e, errorMsg);
                }
                counter++;
            }
            MessageBox.Show(errorMsg);
            return false;
        }
    }
}
