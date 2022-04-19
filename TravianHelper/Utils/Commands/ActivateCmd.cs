using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using OpenQA.Selenium;
using TravianHelper.TravianEntities;

namespace TravianHelper.Utils.Commands
{
    public class ActivateCmd : BaseCommand
    {
        private string _link;

        public string Link
        {
            get => _link;
            set
            {
                _link = value;
                RaisePropertyChanged(() => Link);
            }
        }
        public ActivateCmd(Account acc, string link) : base(acc)
        {
            Display = "Activate";
        }

        public override bool Exec(int counterCount = 0)
        {
            var errorMsg = $"[{Account.NameWithNote}]: Error activate";
            try
            {
                Account.Driver.JsExec.ExecuteScript("window.open()");
                Account.Driver.Chrome.SwitchTo().Window(Account.Driver.Chrome.WindowHandles.Last());
                Account.Driver.Chrome.Navigate().GoToUrl(Link);
                Thread.Sleep(5000);
                Account.Driver.Chrome.SwitchTo().Frame(Account.Driver.Chrome.FindElementsByTagName("iframe").FirstOrDefault(x => x.GetAttribute("Class") == "mellon-iframe"));
                Account.Driver.Chrome.SwitchTo().Frame(Account.Driver.Chrome.FindElementByTagName("iframe"));
                Account.Driver.Chrome.FindElement(By.Name("activate")).Click();
                Thread.Sleep(5000);
                Account.Driver.Chrome.Close();
                Account.Driver.Chrome.SwitchTo().Window(Account.Driver.Chrome.WindowHandles.First());
                Thread.Sleep(5000);
                return true;
            }
            catch (Exception e)
            {
                Logger.Error(e, errorMsg); MessageBox.Show(errorMsg);
                return false;
            }
        }
    }
}
