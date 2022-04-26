using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Practices.Prism.ViewModel;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using RestSharp;
using SmorcIRL.TempMail;
using TravianHelper.SeleniumHost;
using TravianHelper.Settings;
using TravianHelper.TravianEntities;
using Cookie = System.Net.Cookie;
using Proxy = TravianHelper.Settings.Proxy;

namespace TravianHelper.Utils
{
    public enum ResponseType
    {
        Cache,
        Response
    }

    public class Driver : NotificationObject
    {
        private SeleniumHostWPF     _host;
        public  Account             Account            { get; set; }
        public  ChromeDriverService Service            { get; set; }
        private ChromeOptions       Options            { get; set; }
        public  ChromeDriver        Chrome             { get; set; }
        public  IJavaScriptExecutor JsExec             { get; set; }
        public  Actions             Act                { get; set; }
        public  int                 DriverPid          { get; set; }
        public  int                 ChromePid          { get; set; }
        public  IntPtr              ChromeWindowHandle { get; set; }
        private DateTime            _lastRespDate = DateTime.MinValue;

        private bool _nonReg;

        public bool NonReg
        {
            get => _nonReg;
            set
            {
                _nonReg = value;
                RaisePropertyChanged(() => NonReg);
            }
        }

        private Thread _regTh { get; set; }

        public SeleniumHostWPF Host
        {
            get => _host;
            set
            {
                _host = value;
                RaisePropertyChanged(() => Host);
            }
        }

        public  RestClientOptions   RestOptions { get; set; }
        public  RestClient          RestClient  { get; set; }
        public  MailClient          MClient     { get; set; }

        public void Init(Account acc)
        {
            Account = acc;
            if (!Directory.Exists($"{g.UserDataPath}\\{Account.Name}"))
                Directory.CreateDirectory($"{g.UserDataPath}\\{Account.Name}");
            Logger.Info($"[{Account.Name}]: Start driver initialization");
            if(Directory.Exists($"{g.UserDataPath}\\{Account.Name}\\Default"))
                if(File.Exists($"{g.UserDataPath}\\{Account.Name}\\Default\\Secure Preferences"))
                    File.Delete($"{g.UserDataPath}\\{Account.Name}\\Default\\Secure Preferences");
            Service = ChromeDriverService.CreateDefaultService();
            Service.HideCommandPromptWindow = true;
            Options = new ChromeOptions();
            if (Account.Proxy != null)
            {
                if(!string.IsNullOrEmpty(Account.Proxy.UserName) && !string.IsNullOrEmpty(Account.Proxy.Password))
                    Options.AddHttpProxy(Account.Proxy.Ip, Account.Proxy.Port, Account.Proxy.UserName, Account.Proxy.Password, Account.Name);
                else
                {
                    //Options.Proxy              = new OpenQA.Selenium.Proxy();
                    //Options.Proxy.Kind         = ProxyKind.Manual;
                    //Options.Proxy.IsAutoDetect = false;
                    //Options.Proxy.HttpProxy    = $"{Account.Proxy.Ip}:{Account.Proxy.Port}";
                    //Options.Proxy.SslProxy    = $"{Account.Proxy.Ip}:{Account.Proxy.Port}";
                    //Options.AddArgument($"--proxy-server=http://{Account.Proxy.Ip}:{Account.Proxy.Port}");
                    throw new Exception();
                }
            }
            Options.AddExcludedArgument("enable-automation");
            Options.AddArgument("--disable-infobars");
            Options.AddFingerPrintDefenderExt(Account.Name);
            Options.PageLoadStrategy = PageLoadStrategy.Eager;

            Options.AddArgument($"user-data-dir={g.UserDataPath}\\{Account.Name}");

            Chrome                   = new ChromeDriver(Service, Options);

            DriverPid = Service.ProcessId;
            var chromeProcess = Process.GetProcessById(DriverPid).GetChildren().First(x => x.ProcessName != "conhost");
            ChromePid          = chromeProcess.Id;
            ChromeWindowHandle = chromeProcess.MainWindowHandle;

            Application.Current.Dispatcher.Invoke(() =>
            {
                Host = new SeleniumHostWPF
                {
                    DriverService = Service
                };
            });


            JsExec = Chrome;
            Act = new Actions(Chrome);

            RestOptions = new RestClientOptions($"https://{Account.Server.Server}.{Account.Server.Domain}")
                          {
                              UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/61.0.3163.100 Safari/537.36 OPR/48.0.2685.50"
                          };
            if (Account.Proxy != null)
            {
                RestOptions.Proxy = new WebProxy(new Uri($"http://{Account.Proxy.Ip}:{Account.Proxy.Port}"), true, null, new NetworkCredential(Account.Proxy.UserName, Account.Proxy.Password));
            }

            RestClient = new RestClient(RestOptions);
            NonReg     = true;
            Logger.Info($"[{Account.Name}]: End driver initialization");
        }

        public void Dispose()
        {
            Logger.Info($"[{Account.Name}]: Start driver deinitialization");
            Host.DriverService = null;
            Host               = null;
            Act                = null;
            JsExec             = null;
            
            Chrome.Dispose();
            Chrome  = null;
            Options = null;
            Service.Dispose();
            Service         = null;
            Account.Running = null;
            Logger.Info($"[{Account.Name}]: End driver deinitialization");
        }

        public IWebElement Wait(By by, int timeout = 10)
        {
            try
            {
                var wait = new WebDriverWait(Chrome, TimeSpan.FromSeconds(timeout));
                var el   = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(by));

                Logger.Info($"Browser Wait element={by} timeout={timeout} succ");
                return el;
            }
            catch
            {
                Logger.Info($"Browser Wait element={by} timeout={timeout} err");
                return null;
            }
        }

        public string GetSession()
        {
            var cookies     = Chrome.Manage().Cookies.AllCookies;
            var sessionJson = cookies.FirstOrDefault(x => x.Name == "t5SessionKey");
            if (sessionJson == null) return "";
            var     decodedSessionJson        = WebUtility.UrlDecode(sessionJson.Value);
            dynamic dynamicDecodedSessionJson = JObject.Parse(decodedSessionJson);
            return dynamicDecodedSessionJson.key;
        }

        public int GetPlayerId()
        {
            var cookies     = Chrome.Manage().Cookies.AllCookies;
            var sessionJson = cookies.FirstOrDefault(x => x.Name == "t5SessionKey");
            if (sessionJson == null) return -1;
            var     decodedSessionJson        = WebUtility.UrlDecode(sessionJson.Value);
            dynamic dynamicDecodedSessionJson = JObject.Parse(decodedSessionJson);
            return dynamicDecodedSessionJson.id;
        }

        //public dynamic PostJo(JObject json, ResponseType type = ResponseType.Cache, int counterCount = 3)
        //{
        //    var counter = 0;
        //    while (counter < counterCount)
        //    {
        //        try
        //        {
        //            while ((DateTime.Now - _lastRespDate).TotalMilliseconds < 300)
        //                Thread.Sleep(10);

        //            var req = new RestRequest("/api/", Method.Post);
        //            req.AddParameter("c", (string)(json as dynamic).controller.ToString(), ParameterType.QueryString);
        //            req.AddParameter("a", (string)(json as dynamic).action.ToString(), ParameterType.QueryString);
        //            req.AddParameter("t", GetTimeStamp(), ParameterType.QueryString);
        //            var data = Rem(json.ToString());
        //            var buffer = Encoding.UTF8.GetBytes(data);
        //            req.AddHeader("Accept",          "application/json, text/plain, */*");
        //            req.AddHeader("Accept-Encoding", "gzip, deflate, br");
        //            req.AddHeader("Accept-Language", "ru-RU,ru;q=0.8,en-US;q=0.6,en;q=0.4");
        //            req.AddHeader("ContentType",     "application/json;charset=UTF-8");
        //            req.AddHeader("Host",            $"{Account.Server.Server}.{Account.Server.Domain}");
        //            req.AddHeader("Referer",         $"https://{Account.Server.Server}.{Account.Server.Domain}/");
        //            req.AddHeader("Origin",          $"https://{Account.Server.Server}.{Account.Server.Domain}");
        //            req.AddHeader("Content-Type",    "application/json");
        //            var cookies = Chrome.Manage().Cookies.AllCookies;

        //            RestOptions.CookieContainer = new CookieContainer();
        //            foreach (var cookie in cookies)
        //                RestOptions.CookieContainer?.Add(new Cookie(cookie.Name, cookie.Value, cookie.Path, cookie.Domain));
        //            req.AddBody(data, "application/json");

        //            var res = RestClient.ExecuteAsync(req).GetAwaiter().GetResult();
        //            _lastRespDate = DateTime.Now;
        //            if (res?.Content == null)
        //            {
        //                counter++;
        //                Logger.Info($"Post1 error {counter}");
        //                Thread.Sleep(1000);
        //                continue;
        //            }
        //            Logger.Data(res.Content);
        //            try
        //            {
        //                var jo = JObject.Parse(res.Content) as dynamic;
        //                if (type == ResponseType.Response && jo.response != null)
        //                    return jo;
        //                if (type == ResponseType.Cache && jo.cache != null && jo.Count != 0) 
        //                    return jo;

        //                counter++;
        //                Logger.Info($"Post2 error {counter}");
        //                Thread.Sleep(1000);
        //            }
        //            catch (Exception e)
        //            {
        //                counter++;
        //                Logger.Info($"Post3 error {counter}");
        //                Thread.Sleep(1000);
        //            }

        //        }
        //        catch (Exception e)
        //        {
        //            Logger.Info(e.ToString());
        //            counter++;
        //        }
        //    }

        //    return null;
        //}

        public dynamic Post(JObject json, out string error, ResponseType type = ResponseType.Cache)
        {
            try
            {
                while ((DateTime.Now - _lastRespDate).TotalMilliseconds < 1000)
                    Thread.Sleep(10);

                var req = new RestRequest("/api/", Method.Post);
                req.AddParameter("c", (string) (json as dynamic).controller.ToString(), ParameterType.QueryString);
                req.AddParameter("a", (string) (json as dynamic).action.ToString(),     ParameterType.QueryString);
                req.AddParameter("t", GetTimeStamp(),                                   ParameterType.QueryString);
                var data   = Rem(json.ToString());
                req.AddHeader("Accept",          "application/json, text/plain, */*");
                req.AddHeader("Accept-Encoding", "gzip, deflate, br");
                req.AddHeader("Accept-Language", "ru-RU,ru;q=0.8,en-US;q=0.6,en;q=0.4");
                req.AddHeader("ContentType",     "application/json;charset=UTF-8");
                req.AddHeader("Host",            $"{Account.Server.Server}.{Account.Server.Domain}");
                req.AddHeader("Referer",         $"https://{Account.Server.Server}.{Account.Server.Domain}/");
                req.AddHeader("Origin",          $"https://{Account.Server.Server}.{Account.Server.Domain}");
                req.AddHeader("Content-Type",    "application/json");
                var cookies = Chrome.Manage().Cookies.AllCookies;

                RestOptions.CookieContainer = new CookieContainer();
                foreach (var cookie in cookies)
                    RestOptions.CookieContainer?.Add(new Cookie(cookie.Name, cookie.Value, cookie.Path, cookie.Domain));
                req.AddBody(data, "application/json");

                var res = RestClient.ExecuteAsync(req).GetAwaiter().GetResult();
                _lastRespDate = DateTime.Now;
                if (res?.Content == null)
                {
                    error = "NullContent";
                    return null;
                }

                Logger.Data(res.Content);
                try
                {
                    var jo = JObject.Parse(res.Content) as dynamic;
                    if (jo.error != null || jo.response != null && jo.response.Count != 0 && jo.response.errors != null && jo.response.errors.Count != null)
                    {
                        error = "Error";
                        return null;
                    }

                    if (type == ResponseType.Response && jo.response != null)
                    {
                        error = "";
                        return jo;
                    }

                    if (type == ResponseType.Cache && jo.cache != null && jo.cache.Count != 0)
                    {
                        error = "";
                        return jo;
                    }
                    
                    Logger.Info($"Post2 error");
                    error = "Post2";
                    return null;
                }
                catch (Exception e)
                {
                    Logger.Error(e, $"Post3 error");
                    error = "Post3";
                    return null;
                }

            }
            catch (Exception e)
            {
                Logger.Error(e, $"Post4 error");
                error = "Post4";
                return null;
            }
        }

        public string Rem(string str) => str.Replace("\r", "").Replace("\n", "").Replace(" ", "");
        private string GetTimeStamp() => ((long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds).ToString();

        public dynamic GetDataByName(dynamic data, string name)
        {
            foreach (var x in data)
            {
                string[] dataNames = x.name.ToString().Split(':');
                var      names     = name.Split(':');
                if (dataNames.Length != names.Length) continue;
                var eqc = 0;
                for (var i = 0; i < names.Length; i++)
                    if (string.IsNullOrEmpty(names[i])) eqc++;
                    else if (names[i] == "<>") eqc++;
                    else if (names[i] == dataNames[i]) eqc++;
                if (eqc == names.Length) return x;
            }

            return null;
        }

        public List<dynamic> GetDataArrayByName(dynamic data, string name)
        {
            var lst = new List<dynamic>();
            foreach (var x in data)
            {
                string[] dataNames = x.name.ToString().Split(':');
                var      names     = name.Split(':');
                if (dataNames.Length != names.Length) continue;
                var eqc = 0;
                for (var i = 0; i < names.Length; i++)
                    if (string.IsNullOrEmpty(names[i])) eqc++;
                    else if (names[i] == "<>") eqc++;
                    else if (names[i] == dataNames[i]) eqc++;
                if (eqc == names.Length) lst.Add(x);
            }

            return lst.Count != 0 ? lst : null;
        }

        public void Registration()
        {
            if(!NonReg) return;
            NonReg = false;
            _regTh = new Thread(RegThFunc);
            _regTh.Start();
        }
        public void RegThFunc()
        {
            MClient = new MailClient();
            var counter = 0;
            while (counter <= 3)
            {
                try
                {
                    MClient.Register($"{Account.Email}", Account.Password).GetAwaiter().GetResult();
                    break;
                }
                catch (Exception e)
                {
                    try
                    {
                        MClient.Login(Account.Email, Account.Password).GetAwaiter().GetResult();
                        break;
                    }
                    catch (Exception exception)
                    {

                    }
                    counter++;
                    Thread.Sleep(5000);
                }
            }

            if (counter > 3)
            {
                MessageBox.Show("Мыло не нравится");
                return;
            }

            Login($"{Account.Email}", Account.Password);
            Thread.Sleep(5000);
            ChooseTribe(2);
            Thread.Sleep(3000);
            Post(JObject.Parse(
                                 "{\"controller\":\"player\",\"action\":\"changeSettings\",\"params\":{\"newSettings\":{\"premiumConfirmation\":3,\"lang\":\"ru\",\"onlineStatusFilter\":2,\"extendedSimulator\":false,\"musicVolume\":0,\"soundVolume\":0,\"uiSoundVolume\":50,\"muteAll\":true,\"timeZone\":\"3.0\",\"timeFormat\":0,\"attacksFilter\":2,\"mapFilter\":123,\"enableTabNotifications\":true,\"disableAnimations\":true,\"enableHelpNotifications\":true,\"enableWelcomeScreen\":true,\"notpadsVisible\":false}},\"session\":\"" +
                                 GetSession() + "\"}"), out var error);
            DialogAction(1, 1, "setName", Account.Name);
            var msgArr = MClient.GetMessages(1).GetAwaiter().GetResult();
            while (msgArr.Length == 0)
            {
                Thread.Sleep(5000);
                msgArr = MClient.GetMessages(1).GetAwaiter().GetResult();
            }

            Thread.Sleep(5000);

            var msg = "";
            counter = 0;
            while (counter <= 5)
            {
                try
                {
                    msg = MClient.GetMessageSource(msgArr.FirstOrDefault(x => x.Subject.ToLower().Contains("travian kingdoms")).Id).GetAwaiter().GetResult().Data;
                    break;
                }
                catch (Exception e)
                {
                    counter++;
                    Thread.Sleep(5000);
                }
            }

            if (counter > 5)
            {
                MessageBox.Show("Error mail reg");
                return;
            }


            var str = DecodeQuotedPrintables(msg);

            var link = str.Substring(str.IndexOf($"http://www.kingdoms.com/{Account.Server.Region}/#action=activation;token="), 90 + Account.Server.Region.Length);
            JsExec.ExecuteScript("window.open()");
            Chrome.SwitchTo().Window(Chrome.WindowHandles.Last());
            Chrome.Navigate().GoToUrl(link);
            Thread.Sleep(5000);
            Activate();
            Thread.Sleep(5000);
            Chrome.Close();
            Chrome.SwitchTo().Window(Chrome.WindowHandles.First());
            Thread.Sleep(5000);
            DialogAction(1, 1, "activate");
            Thread.Sleep(1500);
            Account.Player.Update();
            Account.Player.UpdateVillageList();
            var vid = Account.Player.VillageList.First().Id;
            SendTroops(vid, 536920065, 3, false, "resources", 12, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1);
            Thread.Sleep(10000);
            DialogAction(1, 2, "backToVillage");
            Thread.Sleep(1500);
            BuildingUpgrade(vid, 33, 22);
            Thread.Sleep(6000);
            BuildingUpgrade(vid, 29, 19);
            Thread.Sleep(1500);
            RecruitUnits(vid, 29, 19, "12", 3);//////////////////////////
            Thread.Sleep(1500);
            DialogAction(30, 1, "attack");
            Thread.Sleep(10000);
            DialogAction(34, 1, "activate");
            Thread.Sleep(1500);
            DialogAction(34, 1, "face");
            Thread.Sleep(1500);
            DialogAction(35, 1, "activate");
            Thread.Sleep(1500);
            BuildingUpgrade(vid, 2, 4);
            Thread.Sleep(6000);
            DialogAction(203, 1, "activate");
            Thread.Sleep(1500);
            DialogAction(203, 1, "become_governor");
            Thread.Sleep(1500);
            DialogAction(204, 1, "activate");
            Thread.Sleep(3000);
            new MapSolver().Solve(Account);
            Thread.Sleep(2000);
            DialogAction(302, 1, "activate");
            Thread.Sleep(5000);
            var counter1 = 0;
            while (Account.Player.VillageList.Count != 1 || Account.Player.VillageList[0].Id < 0)
            {
                Account.Player.Update();
                Account.Player.UpdateVillageList();

                counter1++;
                if (counter1 > 10)
                {
                    Account.Name = "REG ERROR";
                    Account.Save();
                    return;
                }

                Thread.Sleep(5000);
            }

            vid = Account.Player.VillageList.First().Id;

            var newList = new List<int>();
            var destv = -1;
            counter1 = 0;
            while (newList.Count == 0)
            {
                newList.Clear();
                var data = Post(RPG.GetCache_MapDetails(GetSession(), vid), out error);
                if (string.IsNullOrEmpty(error))
                {
                    foreach (var q in data.cache)
                        if (q.data.npcInfo != null)
                            newList.Add(Convert.ToInt32(q.name.ToString().Split(':')[1]));

                }

                counter1++;
                if (counter1 > 10)
                {
                    Account.Name = "REG ERROR";
                    Account.Save();
                    return;
                }

                Thread.Sleep(5000);
            }

            var d1 = Math.Abs(vid - newList[0]);
            var d2 = Math.Abs(vid - newList[1]);
            destv = d1 >= d2 ? newList[0] : newList[1];

            if (destv == -1) return;
            SendTroops(vid, destv, 3, false, "resources", 3, 6, 0, 0, 0, 0, 0, 0, 0, 0, 1);
            Thread.Sleep(20000);
            DialogAction(303, 1, "activate");
            Thread.Sleep(2000);
            Account.Player.Hero.Update();
            Account.Player.Hero.UpdateItems();
            UseHeroItem(1, Account.Player.Hero.Items.First(x => x.ItemType == 120).Id, vid);
            Thread.Sleep(10000);
            DialogAction(399, 1, "activate");
            Thread.Sleep(3000);
            DialogAction(399, 1, "finish");
            Thread.Sleep(1500);
            CollectReward(vid, 205);
            Thread.Sleep(1500);
            UpdateVillageName(vid, Account.Name);
            Thread.Sleep(1500);
            CollectReward(vid, 202);
            Thread.Sleep(3000);
            Chrome.Navigate().Refresh();
            Thread.Sleep(5000);
            Account.RegComplete = true;
            Account.Save();
            Account.UpdateAll();
            Account.OldTaskListWorker.Init();
            NonReg = true;
        }

        public string DecodeQuotedPrintables(string input, string charSet = "")
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

        public void Login(string email, string pass)
        {
            Chrome.SwitchTo().Frame(Chrome.FindElementsByTagName("iframe").FirstOrDefault(x => x.GetAttribute("Class") == "mellon-iframe"));
            Chrome.SwitchTo().Frame(Chrome.FindElementByTagName("iframe"));
            Chrome.FindElement(By.Name("email")).SendKeys(email);
            Chrome.FindElement(By.Name("password[password]")).SendKeys(pass);
            JsExec.ExecuteScript("arguments[0].click();", Chrome.FindElement(By.Name("termsAccepted")));
            Chrome.FindElement(By.Name("submit")).Click();
        }

        public void Activate()
        {
            Chrome.SwitchTo().Frame(Chrome.FindElementsByTagName("iframe").FirstOrDefault(x => x.GetAttribute("Class") == "mellon-iframe"));
            Chrome.SwitchTo().Frame(Chrome.FindElementByTagName("iframe"));
            Chrome.FindElement(By.Name("activate")).Click();
        }

        #region TReq

        public bool BuildingUpgrade(int villageId, int locationId, int buildingType)
        {
            Logger.Info($"[{Account.Name}]: BuildingUpgrade ({villageId}, {locationId}, {buildingType})");
            try
            {
                var data = Post(RPG.BuildingUpgrade(GetSession(), villageId, locationId, buildingType), out var error);
                if (!string.IsNullOrEmpty(error))
                {
                    Logger.Info($"[{Account.Name}]: BuildingUpgrade ({villageId}, {locationId}, {buildingType}) Update FAILED {error}");
                    return false;
                }

                Account.Update(data, (long)data.time);
            }
            catch (Exception e)
            {
                Logger.Info($"[{Account.Name}]: BuildingUpgrade ({villageId}, {locationId}, {buildingType}) Update FAILED with exception:\n{e}\n{e.InnerException}\n{e.InnerException?.InnerException}");
                return false;
            }

            return true;
        }

        public bool BuildingDestroy(int villageId, int locationId)
        {
            Logger.Info($"[{Account.Name}]: BuildingDestroy ({villageId}, {locationId})");
            try
            {
                var data = Post(RPG.BuildingDestroy(GetSession(), villageId, locationId), out var error);
                if (!string.IsNullOrEmpty(error))
                {
                    Logger.Info($"[{Account.Name}]: BuildingDestroy ({villageId}, {locationId}) Update FAILED {error}");
                    return false;
                }

                Account.Update(data, (long)data.time);
            }
            catch (Exception e)
            {
                Logger.Info($"[{Account.Name}]: BuildingDestroy ({villageId}, {locationId}) Update FAILED with exception:\n{e}\n{e.InnerException}\n{e.InnerException?.InnerException}");
                return false;
            }

            return true;
        }

        public bool NpcTrade(int villageId, Resource res)
        {
            Logger.Info($"[{Account.Name}]: NpcTrade ({villageId}, {res})");
            try
            {
                var data = Post(RPG.NpcTrade(GetSession(), villageId, res.Wood, res.Clay, res.Iron, res.Crop), out var error);
                if (!string.IsNullOrEmpty(error))
                {
                    Logger.Info($"[{Account.Name}]: NpcTrade ({villageId}, {res}) Update FAILED {error}");
                    return false;
                }

                Account.Update(data, (long)data.time);
            }
            catch (Exception e)
            {
                Logger.Info($"[{Account.Name}]: NpcTrade ({villageId}, {res}) Update FAILED with exception:\n{e}\n{e.InnerException}\n{e.InnerException?.InnerException}");
                return false;
            }

            return true;
        }

        public bool FinishNow(int villageId, int queueType, int price)
        {
            Logger.Info($"[{Account.Name}]: FinishNow ({villageId}, {queueType}, {price})");
            try
            {
                var data = Post(RPG.FinishBuild(GetSession(), villageId, price, queueType), out var error);
                if (!string.IsNullOrEmpty(error))
                {
                    Logger.Info($"[{Account.Name}]: FinishNow ({villageId}, {queueType}, {price}) Update FAILED {error}");
                    return false;
                }
                

                Account.Update(data, (long)data.time);
            }
            catch (Exception e)
            {
                Logger.Info($"[{Account.Name}]: FinishNow ({villageId}, {queueType}, {price}) Update FAILED with exception:\n{e}\n{e.InnerException}\n{e.InnerException?.InnerException}");
                return false;
            }

            return true;
        }

        public bool SendTroops(int villageId, int destVid, int movType, bool redeployHero, string spyMission, int t1, int t2, int t3, int t4, int t5, int t6, int t7, int t8, int t9, int t10, int t11)
        {
            Logger.Info($"[{Account.Name}]: SendTroops ({villageId}, {destVid}, {movType}, {redeployHero}, {spyMission}, {t1}, {t2}, {t3}, {t4}, {t5}, {t6}, {t7}, {t8}, {t9}, {t10}, {t11})");
            try
            {
                var data = Post(RPG.SendTroops(GetSession(), villageId, destVid, movType, redeployHero, spyMission, t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11), out var error);
                if (!string.IsNullOrEmpty(error))
                {
                    Logger.Info($"[{Account.Name}]: SendTroops ({villageId}, {destVid}, {movType}, {redeployHero}, {spyMission}, {t1}, {t2}, {t3}, {t4}, {t5}, {t6}, {t7}, {t8}, {t9}, {t10}, {t11}) Update FAILED {error}");
                    return false;
                }

                Account.Update(data, (long)data.time);
            }
            catch (Exception e)
            {
                Logger.Info($"[{Account.Name}]: SendTroops ({villageId}, {destVid}, {movType}, {redeployHero}, {spyMission}, {t1}, {t2}, {t3}, {t4}, {t5}, {t6}, {t7}, {t8}, {t9}, {t10}, {t11}) Update FAILED with exception:\n{e}\n{e.InnerException}\n{e.InnerException?.InnerException}");
                return false;
            }

            return true;
        }

        public bool RecruitUnits(int villageId, int locationId, int buildingType, string unitId, int count)
        {
            Logger.Info($"[{Account.Name}]: RecruitUnits ({villageId}, {locationId}, {buildingType}, {unitId}, {count})");
            try
            {
                var data = Post(RPG.RecruitUnits(GetSession(), villageId, locationId, buildingType, unitId, count), out var error);
                if (!string.IsNullOrEmpty(error))
                {
                    Logger.Info($"[{Account.Name}]: RecruitUnits ({villageId}, {locationId}, {buildingType}, {unitId}, {count}) Update FAILED {error}");
                    return false;
                }

                Account.Update(data, (long)data.time);
            }
            catch (Exception e)
            {
                Logger.Info($"[{Account.Name}]: RecruitUnits ({villageId}, {locationId}, {buildingType}, {unitId}, {count}) Update FAILED with exception:\n{e}\n{e.InnerException}\n{e.InnerException?.InnerException}");
                return false;
            }

            return true;
        }

        public bool ChooseTribe(int tribeId)
        {
            Logger.Info($"[{Account.Name}]: ChooseTribe {tribeId}");
            try
            {
                var data = Post(RPG.ChooseTribe(GetSession(), tribeId), out var error);
                if (!string.IsNullOrEmpty(error))
                {
                    Logger.Info($"[{Account.Name}]: ChooseTribe {tribeId} Update FAILED {error}");
                    return false;
                }
            }
            catch (Exception e)
            {
                Logger.Info($"[{Account.Name}]: ChooseTribe {tribeId} Update FAILED with exception:\n{e}\n{e.InnerException}\n{e.InnerException?.InnerException}");
                return false;
            }

            return true;
        }

        public bool DialogAction(int qid, int did, string cmd, string input = "")
        {
            Logger.Info($"[{Account.Name}]: DialogAction ({qid}, {did}, {cmd}, {input})");
            try
            {
                var data = Post(RPG.DialogAction(GetSession(), qid, did, cmd, input), out var error);
                if (!string.IsNullOrEmpty(error))
                {
                    Logger.Info($"[{Account.Name}]: DialogAction ({qid}, {did}, {cmd}, {input}) Update FAILED {error}");
                    return false;
                }
            }
            catch (Exception e)
            {
                Logger.Info($"[{Account.Name}]: DialogAction ({qid}, {did}, {cmd}, {input}) Update FAILED with exception:\n{e}\n{e.InnerException}\n{e.InnerException?.InnerException}");
                return false;
            }

            return true;
        }

        public bool UseHeroItem(int amount, int id, int villageId)
        {
            Logger.Info($"[{Account.Name}]: UseHeroItem ({amount}, {id}, {villageId})");
            try
            {
                var data = Post(RPG.UseHeroItem(GetSession(), amount, id, villageId), out var error);
                if (!string.IsNullOrEmpty(error))
                {
                    Logger.Info($"[{Account.Name}]: UseHeroItem ({amount}, {id}, {villageId}) Update FAILED {error}");
                    return false;
                }

                Account.Update(data, (long)data.time);
            }
            catch (Exception e)
            {
                Logger.Info($"[{Account.Name}]: UseHeroItem ({amount}, {id}, {villageId}) Update FAILED with exception:\n{e}\n{e.InnerException}\n{e.InnerException?.InnerException}");
                return false;
            }

            return true;
        }

        public bool CollectReward(int villageId, int questId)
        {
            Logger.Info($"[{Account.Name}]: CollectReward ({villageId}, {questId})");
            try
            {
                var data = Post(RPG.CollectReward(GetSession(), villageId, questId), out var error);
                if (!string.IsNullOrEmpty(error))
                {
                    Logger.Info($"[{Account.Name}]: CollectReward ({villageId}, {questId}) Update FAILED {error}");
                    return false;
                }

                Account.Update(data, (long)data.time);
            }
            catch (Exception e)
            {
                Logger.Info($"[{Account.Name}]: CollectReward ({villageId}, {questId}) Update FAILED with exception:\n{e}\n{e.InnerException}\n{e.InnerException?.InnerException}");
                return false;
            }

            return true;
        }

        public bool UpdateVillageName(int villageId, string villageName)
        {
            Logger.Info($"[{Account.Name}]: UpdateVillageName ({villageId}, {villageName})");
            try
            {
                var data = Post(RPG.SetVillageName(GetSession(), villageId, villageName), out var error);
                if (!string.IsNullOrEmpty(error))
                {
                    Logger.Info($"[{Account.Name}]: UpdateVillageName ({villageId}, {villageName}) Update FAILED {error}");
                    return false;
                }

                Account.Update(data, (long)data.time);
            }
            catch (Exception e)
            {
                Logger.Info($"[{Account.Name}]: UpdateVillageName ({villageId}, {villageName}) Update FAILED with exception:\n{e}\n{e.InnerException}\n{e.InnerException?.InnerException}");
                return false;
            }

            return true;
        }

        public void SolvePuzzle(JArray moves)
        {
            Logger.Info($"[{Account.Name}]: SolvePuzzle");
            try
            {
                Post(RPG.SolvePuzzle(GetSession(), moves), out var error);
            }
            catch (Exception e)
            {
                Logger.Info($"[{Account.Name}]: SolvePuzzle FAILED with exception:\n{e}\n{e.InnerException}\n{e.InnerException?.InnerException}");
            }
        }

        public JObject GetPuzzle()
        {
            Logger.Info($"[{Account.Name}]: GetPuzzle");
            try
            {
                return Post(RPG.GetPuzzle(GetSession()), out var error, ResponseType.Response);
            }
            catch (Exception e)
            {
                Logger.Info($"[{Account.Name}]: GetPuzzle FAILED with exception:\n{e}\n{e.InnerException}\n{e.InnerException?.InnerException}");
                return null;
            }
        }

        #endregion

        #region Cache

        public bool GetCache(List<string> lst)
        {
            Logger.Info($"[{Account.Name}]: GetCache ({string.Join(";", lst)})");
            try
            {
                var data = Post(RPG.GetCache(GetSession(), lst), out var error);
                if (!string.IsNullOrEmpty(error))
                {
                    Logger.Info($"[{Account.Name}]: GetCache ({string.Join(";", lst)}) Update FAILED {error}");
                    return false;
                }

                Account.Update(data, (long)data.time);
            }
            catch (Exception e)
            {
                Logger.Info($"[{Account.Name}]: GetCache ({string.Join(";", lst)}) Update FAILED with exception:\n{e}\n{e.InnerException}\n{e.InnerException?.InnerException}");
                return false;
            }

            return true;
        }

        public bool GetCache_All()
        {
            Logger.Info($"[{Account.Name}]: GetCache_All");
            try
            {
                var data = Post(RPG.GetCache_All(GetSession()), out var error);
                if (!string.IsNullOrEmpty(error))
                {
                    Logger.Info($"[{Account.Name}]: GetCache_All Update FAILED {error}");
                    return false;
                }

                Account.Update(data, (long)data.time);
            }
            catch (Exception e)
            {
                Logger.Info($"[{Account.Name}]: GetCache_All Update FAILED with exception:\n{e}\n{e.InnerException}\n{e.InnerException?.InnerException}");
                return false;
            }

            return true;
        }

        public bool GetCache_VillageList()
        {
            Logger.Info($"[{Account.Name}]: GetCache_VillageList");
            try
            {
                var data = Post(RPG.GetCache_VillageList(GetSession()), out var error);
                if (!string.IsNullOrEmpty(error))
                {
                    Logger.Info($"[{Account.Name}]: GetCache_VillageList Update FAILED {error}");
                    return false;
                }

                Account.Update(data, (long)data.time);
            }
            catch (Exception e)
            {
                Logger.Info($"[{Account.Name}]: GetCache_VillageList Update FAILED with exception:\n{e}\n{e.InnerException}\n{e.InnerException?.InnerException}");
                return false;
            }

            return true;
        }

        public bool GetCache_Voucher(int playerId)
        {
            Logger.Info($"[{Account.Name}]: GetCache_Voucher");
            try
            {
                var data = Post(RPG.GetCache_Voucher(GetSession(), playerId), out var error);
                if (!string.IsNullOrEmpty(error))
                {
                    Logger.Info($"[{Account.Name}]: GetCache_Voucher Update FAILED {error}");
                    return false;
                }

                Account.Update(data, (long)data.time);
            }
            catch (Exception e)
            {
                Logger.Info($"[{Account.Name}]: GetCache_Voucher Update FAILED with exception:\n{e}\n{e.InnerException}\n{e.InnerException?.InnerException}");
                return false;
            }

            return true;
        }

        public bool GetCache_CollectionHeroItemOwn()
        {
            Logger.Info($"[{Account.Name}]: GetCache_CollectionHeroItemOwn");
            try
            {
                var data = Post(RPG.GetCache_CollectionHeroItemOwn(GetSession()), out var error);
                if (!string.IsNullOrEmpty(error))
                {
                    Logger.Info($"[{Account.Name}]: GetCache_CollectionHeroItemOwn Update FAILED {error}");
                    return false;
                }

                Account.Update(data, (long)data.time);
            }
            catch (Exception e)
            {
                Logger.Info($"[{Account.Name}]: GetCache_CollectionHeroItemOwn Update FAILED with exception:\n{e}\n{e.InnerException}\n{e.InnerException?.InnerException}");
                return false;
            }

            return true;
        }

        public bool GetCache_Quest()
        {
            Logger.Info($"[{Account.Name}]: GetCache_Quest");
            try
            {
                var data = Post(RPG.GetCache_Quest(GetSession()), out var error);
                if (!string.IsNullOrEmpty(error))
                {
                    Logger.Info($"[{Account.Name}]: GetCache_Quest Update FAILED {error}");
                    return false;
                }

                Account.Update(data, (long)data.time);
            }
            catch (Exception e)
            {
                Logger.Info($"[{Account.Name}]: GetCache_Quest Update FAILED with exception:\n{e}\n{e.InnerException}\n{e.InnerException?.InnerException}");
                return false;
            }

            return true;
        }

        public bool GetCache_Player(int playerId)
        {
            Logger.Info($"[{Account.Name}]: GetCache_Player ({playerId})");
            try
            {
                var data = Post(RPG.GetCache_Player(GetSession(), playerId), out var error);
                if (!string.IsNullOrEmpty(error))
                {
                    Logger.Info($"[{Account.Name}]: GetCache_Player ({playerId}) Update FAILED {error}");
                    return false;
                }

                Account.Update(data, (long)data.time);
            }
            catch (Exception e)
            {
                Logger.Info($"[{Account.Name}]: GetCache_Player ({playerId}) Update FAILED with exception:\n{e}\n{e.InnerException}\n{e.InnerException?.InnerException}");
                return false;
            }

            return true;
        }

        public bool GetCache_Hero(int playerId)
        {
            Logger.Info($"[{Account.Name}]: GetCache_Hero ({playerId})");
            try
            {
                var data = Post(RPG.GetCache_Hero(GetSession(), playerId), out var error);
                if (!string.IsNullOrEmpty(error))
                {
                    Logger.Info($"[{Account.Name}]: GetCache_Hero ({playerId}) Update FAILED {error}");
                    return false;
                }

                Account.Update(data, (long)data.time);
            }
            catch (Exception e)
            {
                Logger.Info($"[{Account.Name}]: GetCache_Hero ({playerId}) Update FAILED with exception:\n{e}\n{e.InnerException}\n{e.InnerException?.InnerException}");
                return false;
            }

            return true;
        }

        public bool GetCache_BuildingQueue(int villageId)
        {
            Logger.Info($"[{Account.Name}]: GetCache_BuildingQueue ({villageId})");
            try
            {
                var data = Post(RPG.GetCache_BuildingQueue(GetSession(), villageId), out var error);
                if (!string.IsNullOrEmpty(error))
                {
                    Logger.Info($"[{Account.Name}]: GetCache_BuildingQueue ({villageId}) Update FAILED {error}");
                    return false;
                }

                Account.Update(data, (long)data.time);
            }
            catch (Exception e)
            {
                Logger.Info($"[{Account.Name}]: GetCache_BuildingQueue ({villageId}) Update FAILED with exception:\n{e}\n{e.InnerException}\n{e.InnerException?.InnerException}");
                return false;
            }

            return true;
        }

        public bool GetCache_BuildingCollection(int villageId)
        {
            Logger.Info($"[{Account.Name}]: GetCache_BuildingCollection ({villageId})");
            try
            {
                var data = Post(RPG.GetCache_BuildingCollection(GetSession(), villageId), out var error);
                if (!string.IsNullOrEmpty(error))
                {
                    Logger.Info($"[{Account.Name}]: GetCache_BuildingCollection ({villageId}) Update FAILED {error}");
                    return false;
                }

                Account.Update(data, (long)data.time);
            }
            catch (Exception e)
            {
                Logger.Info($"[{Account.Name}]: GetCache_BuildingCollection ({villageId}) Update FAILED with exception:\n{e}\n{e.InnerException}\n{e.InnerException?.InnerException}");
                return false;
            }

            return true;
        }

        public bool GetCache_MovingTroopsCollection(int villageId)
        {
            Logger.Info($"[{Account.Name}]: GetCache_MovingTroopsCollection ({villageId})");
            try
            {
                var data = Post(RPG.GetCache_MovingTroopsCollection(GetSession(), villageId), out var error);
                if (!string.IsNullOrEmpty(error))
                {
                    Logger.Info($"[{Account.Name}]: GetCache_MovingTroopsCollection ({villageId}) Update FAILED {error}");
                    return false;
                }

                Account.Update(data, (long)data.time);
            }
            catch (Exception e)
            {
                Logger.Info($"[{Account.Name}]: GetCache_MovingTroopsCollection ({villageId}) Update FAILED with exception:\n{e}\n{e.InnerException}\n{e.InnerException?.InnerException}");
                return false;
            }

            return true;
        }

        public bool GetCache_StationaryTroopsCollection(int villageId)
        {
            Logger.Info($"[{Account.Name}]: GetCache_StationaryTroopsCollection ({villageId})");
            try
            {
                var data = Post(RPG.GetCache_StationaryTroopsCollection(GetSession(), villageId), out var error);
                if (!string.IsNullOrEmpty(error))
                {
                    Logger.Info($"[{Account.Name}]: GetCache_StationaryTroopsCollection ({villageId}) Update FAILED {error}");
                    return false;
                }

                Account.Update(data, (long)data.time);
            }
            catch (Exception e)
            {
                Logger.Info($"[{Account.Name}]: GetCache_StationaryTroopsCollection ({villageId}) Update FAILED with exception:\n{e}\n{e.InnerException}\n{e.InnerException?.InnerException}");
                return false;
            }

            return true;
        }

        public dynamic GetCache_MapDetails(int villageId)
        {
            Logger.Info($"[{Account.Name}]: GetCache_MapDetails ({villageId})");
            try
            {
                var data = Post(RPG.GetCache_MapDetails(GetSession(), villageId), out var error);
                if (!string.IsNullOrEmpty(error))
                {
                    Logger.Info($"[{Account.Name}]: GetCache_MapDetails ({villageId}) Update FAILED {error}");
                    return null;
                }

                return data;
            }
            catch (Exception e)
            {
                Logger.Info($"[{Account.Name}]: GetCache_MapDetails ({villageId}) Update FAILED with exception:\n{e}\n{e.InnerException}\n{e.InnerException?.InnerException}");
                return null;
            }

            return null;
        }

        public bool GetCache_Building(int buildingId)
        {
            Logger.Info($"[{Account.Name}]: GetCache_Building ({buildingId})");
            try
            {
                var data = Post(RPG.GetCache_Building(GetSession(), buildingId), out var error);
                if (!string.IsNullOrEmpty(error))
                {
                    Logger.Info($"[{Account.Name}]: GetCache_Building ({buildingId}) Update FAILED {error}");
                    return false;
                }

                Account.Update(data, (long)data.time);
            }
            catch (Exception e)
            {
                Logger.Info($"[{Account.Name}]: GetCache_Building ({buildingId}) Update FAILED with exception:\n{e}\n{e.InnerException}\n{e.InnerException?.InnerException}");
                return false;
            }

            return true;
        }

        #endregion
    }

    public static class ChromeOptionsExtensions
    {
        public static void AddHttpProxy(this ChromeOptions options, string host, int port, string userName, string password, string accName)
        {
            if (!Directory.Exists($"{g.UserDataPath}/{accName}"))
                Directory.CreateDirectory($"{g.UserDataPath}/{accName}");

            if (!Directory.Exists($"{g.UserDataPath}/{accName}/Proxy"))
                Directory.CreateDirectory($"{g.UserDataPath}/{accName}/Proxy");

            if (File.Exists($"{g.UserDataPath}/{accName}/Proxy/ext.crx"))
                File.Delete($"{g.UserDataPath}/{accName}/Proxy/ext.crx");

            var manifestFile = $"{g.UserDataPath}/{accName}/Proxy/manifest.json";
            var scriptFile   = $"{g.UserDataPath}/{accName}/Proxy/script.js";
            var crx          = $"{g.UserDataPath}/{accName}/Proxy/ext.crx";

            File.WriteAllText(manifestFile,
                              "{ \"version\": \"1.0.0\", \"manifest_version\": 2, \"name\": \"Chrome Proxy\", \"permissions\": [ \"proxy\", \"tabs\", \"unlimitedStorage\", \"storage\", \"<all_urls>\", \"webRequest\", \"webRequestBlocking\" ], \"background\": { \"scripts\": [\"script.js\"]},    \"minimum_chrome_version\":\"22.0.0\"}");
            File.WriteAllText(scriptFile,
                              $"var config = {{mode: \"fixed_servers\", rules: {{ singleProxy: {{ scheme: \"http\", host: \"{host}\", port: parseInt({port}) }}, bypassList: []}}}};chrome.proxy.settings.set({{ value: config, scope: \"regular\" }}, function() {{ }});function callbackFn(details){{return {{authCredentials:{{username: \"{userName}\",password: \"{password}\"}}}};}}chrome.webRequest.onAuthRequired.addListener(callbackFn,{{ urls:[\"<all_urls>\"] }}, ['blocking']);");

            using (var destination = ZipFile.Open(crx, ZipArchiveMode.Create))
            {
                destination.CreateEntryFromFile(manifestFile, "manifest.json");
                destination.CreateEntryFromFile(scriptFile, "script.js");
            }

            File.Delete(manifestFile);
            File.Delete(scriptFile);
            options.AddExtension(crx);

        }

        public static void AddFingerPrintDefenderExt(this ChromeOptions options, string accName)
        {
            double hash1 = 0;
            double hash2 = 0;
            double hash3 = 0;
            double hash4 = 0;
            for (var i = 0; i < accName.Length; i++)
            {
                if (i % 4 == 0)
                    hash1 += accName[i];
                if (i % 4 == 1)
                    hash2 += accName[i];
                if (i % 4 == 2)
                    hash3 += accName[i];
                if (i % 4 == 3)
                    hash4 += accName[i];
            }

            while (hash1 > 255)
                hash1 /= 10;

            while (hash2 > 255)
                hash2 /= 10;

            while (hash3 > 255)
                hash3 /= 10;

            while (hash4 > 10)
                hash4 /= 10;

            if (!Directory.Exists($"{g.UserDataPath}/{accName}"))
                Directory.CreateDirectory($"{g.UserDataPath}/{accName}");
            if (!Directory.Exists($"{g.UserDataPath}/{accName}/FPD"))
                Directory.CreateDirectory($"{g.UserDataPath}/{accName}/FPD");
            {
                if (!Directory.Exists($"{g.UserDataPath}/{accName}/FPD/Canvas"))
                    Directory.CreateDirectory($"{g.UserDataPath}/{accName}/FPD/Canvas");
                if (File.Exists($"{g.UserDataPath}/{accName}/FPD/Canvas/ext.crx"))
                    File.Delete($"{g.UserDataPath}/{accName}/FPD/Canvas/ext.crx");
                var manifestFile = $"{g.UserDataPath}/{accName}/FPD/Canvas/manifest.json";
                var scriptFile   = $"{g.UserDataPath}/{accName}/FPD/Canvas/script.js";
                var crx          = $"{g.UserDataPath}/{accName}/FPD/Canvas/ext.crx";

                File.WriteAllText(manifestFile,
                                  "{\"content_scripts\":[{\"all_frames\":true,\"js\":[\"script.js\"],\"match_about_blank\":true,\"matches\":[\"*://*/*\"],\"run_at\":\"document_start\"}],\"manifest_version\":2,\"name\":\"Canvas Fingerprint Defender\",\"permissions\":[\"storage\"],\"version\":\"1.0.0\"}");
                File.WriteAllText(scriptFile,
                                  $"var background=(function(){{var tmp={{}};chrome.runtime.onMessage.addListener(function(request,sender,sendResponse){{for(var id in tmp){{if(tmp[id]&&(typeof tmp[id]===\"function\")){{if(request.path===\"background-to-page\"){{if(request.method===id)tmp[id](request.data)}}}}}}}});return{{\"receive\":function(id,callback){{tmp[id]=callback}},\"send\":function(id,data){{chrome.runtime.sendMessage({{\"path\":\"page-to-background\",\"method\":id,\"data\":data}})}}}}}})();var inject=function(){{const toBlob=HTMLCanvasElement.prototype.toBlob;const toDataURL=HTMLCanvasElement.prototype.toDataURL;const getImageData=CanvasRenderingContext2D.prototype.getImageData;var noisify=function(canvas,context){{if(context){{const shift={{'r':{hash1.ToString().Replace(",", ".")},'g':{hash2.ToString().Replace(",", ".")},'b':{hash3.ToString().Replace(",", ".")},'a':{hash4.ToString().Replace(",", ".")}}};const width=canvas.width;const height=canvas.height;if(width&&height){{const imageData=getImageData.apply(context,[0,0,width,height]);for(let i=0;i<height;i++){{for(let j=0;j<width;j++){{const n=((i*(width*4))+(j*4));if(imageData.data[n+0]+shift.r<256)imageData.data[n+0]=imageData.data[n+0]+shift.r;else imageData.data[n+0]=imageData.data[n+0]-shift.r;if(imageData.data[n+1]+shift.r<256)imageData.data[n+1]=imageData.data[n+1]+shift.g;else imageData.data[n+1]=imageData.data[n+1]-shift.g;if(imageData.data[n+2]+shift.r<256)imageData.data[n+2]=imageData.data[n+2]+shift.b;else imageData.data[n+2]=imageData.data[n+2]-shift.b;if(imageData.data[n+3]+shift.r<256)imageData.data[n+3]=imageData.data[n+3]+shift.a;else imageData.data[n+3]=imageData.data[n+3]-shift.a}}}}window.top.postMessage(\"canvas-fingerprint-defender-alert\",'*');context.putImageData(imageData,0,0)}}}}}};Object.defineProperty(HTMLCanvasElement.prototype,\"toBlob\",{{\"value\":function(){{noisify(this,this.getContext(\"2d\"));return toBlob.apply(this,arguments)}}}});Object.defineProperty(HTMLCanvasElement.prototype,\"toDataURL\",{{\"value\":function(){{noisify(this,this.getContext(\"2d\"));return toDataURL.apply(this,arguments)}}}});Object.defineProperty(CanvasRenderingContext2D.prototype,\"getImageData\",{{\"value\":function(){{noisify(this.canvas,this);return getImageData.apply(this,arguments)}}}});document.documentElement.dataset.cbscriptallow=true}};var script_1=document.createElement(\"script\");script_1.textContent=\"(\"+inject+\")()\";document.documentElement.appendChild(script_1);script_1.remove();if(document.documentElement.dataset.cbscriptallow!==\"true\"){{var script_2=document.createElement(\"script\");script_2.textContent=`{{const iframes=[...window.top.document.querySelectorAll(\"iframe[sandbox]\")];for(var i=0;i<iframes.length;i++){{if(iframes[i].contentWindow){{if(iframes[i].contentWindow.CanvasRenderingContext2D){{iframes[i].contentWindow.CanvasRenderingContext2D.prototype.getImageData=CanvasRenderingContext2D.prototype.getImageData}}if(iframes[i].contentWindow.HTMLCanvasElement){{iframes[i].contentWindow.HTMLCanvasElement.prototype.toBlob=HTMLCanvasElement.prototype.toBlob;iframes[i].contentWindow.HTMLCanvasElement.prototype.toDataURL=HTMLCanvasElement.prototype.toDataURL}}}}}}}}`;window.top.document.documentElement.appendChild(script_2);script_2.remove()}}window.addEventListener(\"message\",function(e){{if(e.data&&e.data===\"canvas-fingerprint-defender-alert\"){{background.send(\"fingerprint\",{{\"host\":document.location.host}})}}}},false);");

                using (var destination = ZipFile.Open(crx, ZipArchiveMode.Create))
                {
                    destination.CreateEntryFromFile(manifestFile, "manifest.json");
                    destination.CreateEntryFromFile(scriptFile, "script.js");
                }

                File.Delete(manifestFile);
                File.Delete(scriptFile);
                options.AddExtension(crx);
            }
            {
                if (!Directory.Exists($"{g.UserDataPath}/{accName}/FPD/Audio"))
                    Directory.CreateDirectory($"{g.UserDataPath}/{accName}/FPD/Audio");
                if (File.Exists($"{g.UserDataPath}/{accName}/FPD/Audio/ext.crx"))
                    File.Delete($"{g.UserDataPath}/{accName}/FPD/Audio/ext.crx");
                var manifestFile = $"{g.UserDataPath}/{accName}/FPD/Audio/manifest.json";
                var scriptFile = $"{g.UserDataPath}/{accName}/FPD/Audio/script.js";
                var crx = $"{g.UserDataPath}/{accName}/FPD/Audio/ext.crx";

                File.WriteAllText(manifestFile,
                                  "{\"content_scripts\":[{\"all_frames\":true,\"js\":[\"script.js\"],\"match_about_blank\":true,\"matches\":[\"*://*/*\"],\"run_at\":\"document_start\"}],\"manifest_version\":2,\"name\":\"AudioContext Fingerprint Defender\",\"permissions\":[\"storage\"],\"version\":\"1.0.0\"}");
                File.WriteAllText(scriptFile,
                                  $"var background=function(){{var t={{}};return chrome.runtime.onMessage.addListener((function(e,n,o){{for(var i in t)t[i]&&\"function\"==typeof t[i]&&\"background-to-page\"===e.path&&e.method===i&&t[i](e.data)}})),{{receive:function(e,n){{t[e]=n}},send:function(t,e){{chrome.runtime.sendMessage({{path:\"page-to-background\",method:t,data:e}})}}}}}}(),inject=function(){{const t={{BUFFER:null,getChannelData:function(e){{const n=e.prototype.getChannelData;Object.defineProperty(e.prototype,\"getChannelData\",{{value:function(){{const e=n.apply(this,arguments);if(t.BUFFER!==e){{t.BUFFER=e,window.top.postMessage(\"audiocontext-fingerprint-defender-alert\",\"*\");for(var o=0;o<e.length;o+=100){{let t=Math.floor({hash1.ToString().Replace(",", ".")}*o);e[t]=e[t]+1e-7*{hash2.ToString().Replace(",", ".")}}}}}return e}}}})}},createAnalyser:function(t){{const e=t.prototype.__proto__.createAnalyser;Object.defineProperty(t.prototype.__proto__,\"createAnalyser\",{{value:function(){{const t=e.apply(this,arguments),n=t.__proto__.getFloatFrequencyData;return Object.defineProperty(t.__proto__,\"getFloatFrequencyData\",{{value:function(){{window.top.postMessage(\"audiocontext-fingerprint-defender-alert\",\"*\");const t=n.apply(this,arguments);for(var e=0;e<arguments[0].length;e+=100){{let t=Math.floor({hash3.ToString().Replace(",", ".")}*e);arguments[0][t]=arguments[0][t]+.1*{hash4.ToString().Replace(",", ".")}}}return t}}}}),t}}}})}}}};t.getChannelData(AudioBuffer),t.createAnalyser(AudioContext),t.getChannelData(OfflineAudioContext),t.createAnalyser(OfflineAudioContext),document.documentElement.dataset.acxscriptallow=!0}},script_1=document.createElement(\"script\");if(script_1.textContent=\"(\"+inject+\")()\",document.documentElement.appendChild(script_1),script_1.remove(),\"true\"!==document.documentElement.dataset.acxscriptallow){{var script_2=document.createElement(\"script\");script_2.textContent='{{\\n    const iframes = [...window.top.document.querySelectorAll(\"iframe[sandbox]\")];\\n    for (var i = 0; i < iframes.length; i++) {{\\n      if (iframes[i].contentWindow) {{\\n        if (iframes[i].contentWindow.AudioBuffer) {{\\n          if (iframes[i].contentWindow.AudioBuffer.prototype) {{\\n            if (iframes[i].contentWindow.AudioBuffer.prototype.getChannelData) {{\\n              iframes[i].contentWindow.AudioBuffer.prototype.getChannelData = AudioBuffer.prototype.getChannelData;\\n            }}\\n          }}\\n        }}\\n\\n        if (iframes[i].contentWindow.AudioContext) {{\\n          if (iframes[i].contentWindow.AudioContext.prototype) {{\\n            if (iframes[i].contentWindow.AudioContext.prototype.__proto__) {{\\n              if (iframes[i].contentWindow.AudioContext.prototype.__proto__.createAnalyser) {{\\n                iframes[i].contentWindow.AudioContext.prototype.__proto__.createAnalyser = AudioContext.prototype.__proto__.createAnalyser;\\n              }}\\n            }}\\n          }}\\n        }}\\n\\n        if (iframes[i].contentWindow.OfflineAudioContext) {{\\n          if (iframes[i].contentWindow.OfflineAudioContext.prototype) {{\\n            if (iframes[i].contentWindow.OfflineAudioContext.prototype.__proto__) {{\\n              if (iframes[i].contentWindow.OfflineAudioContext.prototype.__proto__.createAnalyser) {{\\n                iframes[i].contentWindow.OfflineAudioContext.prototype.__proto__.createAnalyser = OfflineAudioContext.prototype.__proto__.createAnalyser;\\n              }}\\n            }}\\n          }}\\n        }}\\n\\n        if (iframes[i].contentWindow.OfflineAudioContext) {{\\n          if (iframes[i].contentWindow.OfflineAudioContext.prototype) {{\\n            if (iframes[i].contentWindow.OfflineAudioContext.prototype.__proto__) {{\\n              if (iframes[i].contentWindow.OfflineAudioContext.prototype.__proto__.getChannelData) {{\\n                iframes[i].contentWindow.OfflineAudioContext.prototype.__proto__.getChannelData = OfflineAudioContext.prototype.__proto__.getChannelData;\\n              }}\\n            }}\\n          }}\\n        }}\\n      }}\\n    }}\\n  }}',window.top.document.documentElement.appendChild(script_2),script_2.remove()}}window.addEventListener(\"message\",(function(t){{t.data&&\"audiocontext-fingerprint-defender-alert\"===t.data&&background.send(\"fingerprint\",{{host:document.location.host}})}}),!1);");

                using (var destination = ZipFile.Open(crx, ZipArchiveMode.Create))
                {
                    destination.CreateEntryFromFile(manifestFile, "manifest.json");
                    destination.CreateEntryFromFile(scriptFile, "script.js");
                }

                File.Delete(manifestFile);
                File.Delete(scriptFile);
                options.AddExtension(crx);
            }
            //{
            //    if (!Directory.Exists($"{g.Settings.UserDataPath}/{accName}/FPD/WebGL"))
            //        Directory.CreateDirectory($"{g.Settings.UserDataPath}/{accName}/FPD/WebGL");
            //    if (File.Exists($"{g.Settings.UserDataPath}/{accName}/FPD/WebGL/ext.crx"))
            //        File.Delete($"{g.Settings.UserDataPath}/{accName}/FPD/WebGL/ext.crx");
            //    var manifestFile = $"{g.Settings.UserDataPath}/{accName}/FPD/WebGL/manifest.json";
            //    var scriptFile = $"{g.Settings.UserDataPath}/{accName}/FPD/WebGL/script.js";
            //    var crx = $"{g.Settings.UserDataPath}/{accName}/FPD/WebGL/ext.crx";

            //    File.WriteAllText(manifestFile,
            //                      "{\"content_scripts\":[{\"all_frames\":true,\"js\":[\"script.js\"],\"match_about_blank\":true,\"matches\":[\"*://*/*\"],\"run_at\":\"document_start\"}],\"manifest_version\":2,\"name\":\"WebGL Fingerprint Defender\",\"permissions\":[\"storage\"],\"version\":\"1.0.0\"}");
            //    File.WriteAllText(scriptFile,
            //                      $"var background=function(){{var e={{}};return chrome.runtime.onMessage.addListener((function(t,n,r){{for(var o in e)e[o]&&\"function\"==typeof e[o]&&\"background-to-page\"===t.path&&t.method===o&&e[o](t.data)}})),{{receive:function(t,n){{e[t]=n}},send:function(e,t){{chrome.runtime.sendMessage({{path:\"page-to-background\",method:e,data:t}})}}}}}}(),inject=function(){{var e={{random:{{value:function(){{return {hash}}},item:function(t){{var n=t.length*e.random.value();return t[Math.floor(n)]}},number:function(t){{for(var n=[],r=0;r<t.length;r++)n.push(Math.pow(2,t[r]));return e.random.item(n)}},int:function(t){{for(var n=[],r=0;r<t.length;r++){{var o=Math.pow(2,t[r]);n.push(new Int32Array([o,o]))}}return e.random.item(n)}},float:function(t){{for(var n=[],r=0;r<t.length;r++){{var o=Math.pow(2,t[r]);n.push(new Float32Array([1,o]))}}return e.random.item(n)}}}},spoof:{{webgl:{{buffer:function(t){{var n=t.prototype?t.prototype:t.__proto__;const r=n.bufferData;Object.defineProperty(n,\"bufferData\",{{value:function(){{var t=Math.floor(e.random.value()*arguments[1].length),n=void 0!==arguments[1][t]?.1*e.random.value()*arguments[1][t]:0;return arguments[1][t]=arguments[1][t]+n,window.top.postMessage(\"webgl-fingerprint-defender-alert\",\"*\"),r.apply(this,arguments)}}}})}},parameter:function(t){{var n=t.prototype?t.prototype:t.__proto__;const r=n.getParameter;Object.defineProperty(n,\"getParameter\",{{value:function(){{return window.top.postMessage(\"webgl-fingerprint-defender-alert\",\"*\"),3415===arguments[0]?0:3414===arguments[0]?24:36348===arguments[0]?30:7936===arguments[0]?\"WebKit\":37445===arguments[0]?\"Google Inc.\":7937===arguments[0]?\"WebKit WebGL\":3379===arguments[0]?e.random.number([14,15]):36347===arguments[0]?e.random.number([12,13]):34076===arguments[0]||34024===arguments[0]?e.random.number([14,15]):3386===arguments[0]?e.random.int([13,14,15]):3413===arguments[0]||3412===arguments[0]||3411===arguments[0]||3410===arguments[0]||34047===arguments[0]||34930===arguments[0]||34921===arguments[0]||35660===arguments[0]?e.random.number([1,2,3,4]):35661===arguments[0]?e.random.number([4,5,6,7,8]):36349===arguments[0]?e.random.number([10,11,12,13]):33902===arguments[0]||33901===arguments[0]?e.random.float([0,10,11,12,13]):37446===arguments[0]?e.random.item([\"Graphics\",\"HD Graphics\",\"Intel(R) HD Graphics\"]):7938===arguments[0]?e.random.item([\"WebGL 1.0\",\"WebGL 1.0 (OpenGL)\",\"WebGL 1.0 (OpenGL Chromium)\"]):35724===arguments[0]?e.random.item([\"WebGL\",\"WebGL GLSL\",\"WebGL GLSL ES\",\"WebGL GLSL ES (OpenGL Chromium\"]):r.apply(this,arguments)}}}})}}}}}}}};e.spoof.webgl.buffer(WebGLRenderingContext),e.spoof.webgl.buffer(WebGL2RenderingContext),e.spoof.webgl.parameter(WebGLRenderingContext),e.spoof.webgl.parameter(WebGL2RenderingContext),document.documentElement.dataset.wgscriptallow=!0}},script_1=document.createElement(\"script\");if(script_1.textContent=\"(\"+inject+\")()\",document.documentElement.appendChild(script_1),script_1.remove(),\"true\"!==document.documentElement.dataset.wgscriptallow){{var script_2=document.createElement(\"script\");script_2.textContent='{{\\n    const iframes = [...window.top.document.querySelectorAll(\"iframe[sandbox]\")];\\n    for (var i = 0; i < iframes.length; i++) {{\\n      if (iframes[i].contentWindow) {{\\n        if (iframes[i].contentWindow.WebGLRenderingContext) {{\\n          iframes[i].contentWindow.WebGLRenderingContext.prototype.bufferData = WebGLRenderingContext.prototype.bufferData;\\n          iframes[i].contentWindow.WebGLRenderingContext.prototype.getParameter = WebGLRenderingContext.prototype.getParameter;\\n        }}\\n        if (iframes[i].contentWindow.WebGL2RenderingContext) {{\\n          iframes[i].contentWindow.WebGL2RenderingContext.prototype.bufferData = WebGL2RenderingContext.prototype.bufferData;\\n          iframes[i].contentWindow.WebGL2RenderingContext.prototype.getParameter = WebGL2RenderingContext.prototype.getParameter;\\n        }}\\n      }}\\n    }}\\n  }}',window.top.document.documentElement.appendChild(script_2),script_2.remove()}}window.addEventListener(\"message\",(function(e){{e.data&&\"webgl-fingerprint-defender-alert\"===e.data&&background.send(\"fingerprint\",{{host:document.location.host}})}}),!1);");

            //    using (var destination = ZipFile.Open(crx, ZipArchiveMode.Create))
            //    {
            //        destination.CreateEntryFromFile(manifestFile, "manifest.json");
            //        destination.CreateEntryFromFile(scriptFile, "script.js");
            //    }

            //    File.Delete(manifestFile);
            //    File.Delete(scriptFile);
            //    options.AddExtension(crx);
            //}
        }
    }

}
