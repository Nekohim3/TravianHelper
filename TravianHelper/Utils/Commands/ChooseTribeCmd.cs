using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TravianHelper.StaticData;
using TravianHelper.TravianEntities;

namespace TravianHelper.Utils.Commands
{
    public class ChooseTribeCmd : BaseCommand
    {
        private int _tribeId;

        public int TribeId
        {
            get => _tribeId;
            set
            {
                _tribeId = value;
                RaisePropertyChanged(() => TribeId);
            }
        }

        public ChooseTribeCmd(Account acc, int tribeId) : base(acc)
        {
            TribeId = tribeId;
            Display = $"ChooseTribe:{TribesData.GetByTribeId(TribeId)?.Name}";
        }

        public override bool Exec(int counterCount = 0)
        {
            var errorMsg = $"[{Account.NameWithNote}]: ErrorChooseTribe ({TribeId})";
            var counter  = 0;
            while (counter <= counterCount)
            {
                try
                {
                   if (!Account.Driver.ChooseTribe(TribeId)) throw new Exception($"{errorMsg}");
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
