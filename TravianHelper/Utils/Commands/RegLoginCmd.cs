using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using OpenQA.Selenium;
using TravianHelper.TravianEntities;

namespace TravianHelper.Utils.Commands
{
    public class RegLoginCmd : BaseCommand
    {
        public RegLoginCmd(Account acc) : base(acc)
        {
            Display = "EnterRegData";
        }

        public override bool Exec(int counterCount = 0)
        {
            var errorMsg = $"[{Account.NameWithNote}]: Error EnterRegData";
            try
            {
                Account.Driver.Chrome.SwitchTo().Frame(Account.Driver.Chrome.FindElementsByTagName("iframe").FirstOrDefault(x => x.GetAttribute("Class") == "mellon-iframe"));
                Account.Driver.Chrome.SwitchTo().Frame(Account.Driver.Chrome.FindElementByTagName("iframe"));
                Account.Driver.Chrome.FindElement(By.Name("email")).SendKeys(Account.Email);
                Account.Driver.Chrome.FindElement(By.Name("password[password]")).SendKeys(Account.Password);
                Account.Driver.JsExec.ExecuteScript("arguments[0].click();", Account.Driver.Chrome.FindElement(By.Name("termsAccepted")));
                Account.Driver.Chrome.FindElement(By.Name("submit")).Click();
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
