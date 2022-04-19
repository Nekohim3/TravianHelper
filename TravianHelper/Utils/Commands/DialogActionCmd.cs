using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TravianHelper.TravianEntities;

namespace TravianHelper.Utils.Commands
{
    public class DialogActionCmd : BaseCommand
    {
        private int _questId;

        public int QuestId
        {
            get => _questId;
            set
            {
                _questId = value;
                RaisePropertyChanged(() => QuestId);
            }
        }

        private int _dialogId;

        public int DialogId
        {
            get => _dialogId;
            set
            {
                _dialogId = value;
                RaisePropertyChanged(() => DialogId);
            }
        }

        private string _command;

        public string Command
        {
            get => _command;
            set
            {
                _command = value;
                RaisePropertyChanged(() => Command);
            }
        }

        private string _input;

        public string Input
        {
            get => _input;
            set
            {
                _input = value;
                RaisePropertyChanged(() => Input);
            }
        }
        public DialogActionCmd(Account acc, int qid, int did, string cmd, string input = "") : base(acc)
        {
            QuestId  = qid;
            DialogId = did;
            Command  = cmd;
            Input    = input;
            Display  = $"DialogAction:{QuestId}:{DialogId}:{Command}:{Input}";
        }

        public override bool Exec(int counterCount = 0)
        {
            var errorMsg = $"[{Account.NameWithNote}]: Error DialogActionCmd({QuestId}, {DialogId}, {Command}, {Input})";
            var counter  = 0;
            while (counter <= counterCount)
            {
                try
                {
                    if (!Account.Driver.DialogAction(QuestId, DialogId, Command, Input)) throw new Exception(errorMsg);
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
