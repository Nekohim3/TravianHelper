using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using TravianHelper.TravianEntities;

namespace TravianHelper.Utils.Commands
{
    public class GetRegLinkCmd : BaseCommand
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

        public GetRegLinkCmd(Account acc) : base(acc)
        {
            Display = $"GetRegLink";
        }

        public override bool Exec(int counterCount = 0)
        {
            var errorMsg = $"[{Account.NameWithNote}]: Error GetRegLink";
            var counter  = 0;

            var msgArr   = Account.MailClient.GetMessages(1).GetAwaiter().GetResult();
            while (msgArr.Length == 0 && counter < 10)
            {
                Thread.Sleep(5000);
                msgArr = Account.MailClient.GetMessages(1).GetAwaiter().GetResult();
                counter++;
            }

            if (msgArr.Length == 0)
            {
                Logger.Error($"{errorMsg} Cant get messageList");
                MessageBox.Show(errorMsg);
                return false;
            }
            Thread.Sleep(5000);
            counter = 0;
            
            while (counter <= counterCount)
            {
                try
                {
                    var msg = msgArr.FirstOrDefault(x => x.Subject.ToLower().Contains("travian kingdoms"));
                    if (msg == null)
                    {
                        Logger.Error($"{errorMsg} Cant get travian message");
                        counter++;
                        Thread.Sleep(5000);
                        continue;
                    }
                    var msgSource = Account.MailClient.GetMessageSource(msg.Id).GetAwaiter().GetResult().Data;
                    if (!string.IsNullOrEmpty(msgSource))
                    {
                        var str = DecodeQuotedPrintables(msgSource);
                        Link = str.Substring(str.IndexOf($"http://www.kingdoms.com/{Account.Server.Region}/#action=activation;token="), 90 + Account.Server.Region.Length);
                    }
                    else
                    {
                        Logger.Error($"{errorMsg} Cant get msgSource");
                        counter++;
                        Thread.Sleep(5000);
                        continue;
                    }
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

        public static string DecodeQuotedPrintables(string input, string charSet = "")
        {
            if (string.IsNullOrEmpty(charSet))
            {
                var charSetOccurences = new Regex(@"=\?.*\?Q\?", RegexOptions.IgnoreCase);
                var charSetMatches = charSetOccurences.Matches(input);
                foreach (Match match in charSetMatches)
                {
                    charSet = match.Groups[0].Value.Replace("=?", "").Replace("?Q?", "");
                    input = input.Replace(match.Groups[0].Value, "").Replace("?=", "");
                }
            }

            Encoding enc = new ASCIIEncoding();
            if (!string.IsNullOrEmpty(charSet))
            {
                try
                {
                    enc = Encoding.GetEncoding(charSet);
                }
                catch
                {
                    enc = new ASCIIEncoding();
                }
            }

            //decode iso-8859-[0-9]
            var occurences = new Regex(@"=[0-9A-Z]{2}", RegexOptions.Multiline);
            var matches = occurences.Matches(input);
            foreach (Match match in matches)
            {
                try
                {
                    byte[] b = new byte[] { byte.Parse(match.Groups[0].Value.Substring(1), System.Globalization.NumberStyles.AllowHexSpecifier) };
                    char[] hexChar = enc.GetChars(b);
                    input = input.Replace(match.Groups[0].Value, hexChar[0].ToString());
                }
                catch { }
            }

            //decode base64String (utf-8?B?)
            occurences = new Regex(@"\?utf-8\?B\?.*\?", RegexOptions.IgnoreCase);
            matches = occurences.Matches(input);
            foreach (Match match in matches)
            {
                byte[] b = Convert.FromBase64String(match.Groups[0].Value.Replace("?utf-8?B?", "").Replace("?UTF-8?B?", "").Replace("?", ""));
                string temp = Encoding.UTF8.GetString(b);
                input = input.Replace(match.Groups[0].Value, temp);
            }

            input = input.Replace("=\r\n", "");
            return input;
        }
    }
}
