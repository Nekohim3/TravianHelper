using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using SmorcIRL.TempMail;
using TravianHelper.TravianEntities;

namespace TravianHelper.Utils.Commands
{
    public class MailRegCmd : BaseCommand
    {
        public MailRegCmd(Account acc) : base(acc)
        {
            Display = "Mail reg";
        }

        public override bool Exec(int counterCount = 0)
        {
            Account.MailClient = new MailClient();
            var counter = 0;
            while (counter <= counterCount)
            {
                try
                {
                    Account.MailClient.Register($"{Account.Email}", Account.Password);
                    return true;
                }
                catch (Exception e)
                {
                    Logger.Error(e, $"[{Account.NameWithNote}]: MailRegError");
                }
                counter++;
            }
            MessageBox.Show("MailRegError");
            return false;
        }
    }
}
