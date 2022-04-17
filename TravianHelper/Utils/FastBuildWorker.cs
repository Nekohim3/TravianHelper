using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Practices.Prism.ViewModel;
using OpenQA.Selenium;
using TravianHelper.TravianEntities;

namespace TravianHelper.Utils
{
    public class FastBuildWorker : NotificationObject
    {
        private bool _working;

        public bool Working
        {
            get => _working;
            set
            {
                if(_working == value) 
                    return;
                _working = value;
                if (_working)
                    Run();
                else
                    NotBlockWait = false;
                RaisePropertyChanged(() => Working);
            }
        }

        private bool _notBlockWait;

        public bool NotBlockWait
        {
            get => _notBlockWait;
            set
            {
                _notBlockWait = value;
                RaisePropertyChanged(() => NotBlockWait);
            }
        }

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

        private Thread _workThread; 
        
        public FastBuildWorker(Account acc)
        {
            Account      = acc;
            NotBlockWait = true;
        }

        private void Run()
        {
            _workThread = new Thread(ThreadFunc);
            _workThread.Start();
        }

        private void ThreadFunc()
        {
            while (Working)
            {
                if (Account.Running == true && Account.Loaded && Account.RegComplete)
                {
                    if (Account.UseSingleBuild)
                    {
                        try
                        {
                            var vid = Account.Driver.Chrome.Url.Split('/').FirstOrDefault(x => x.ToLower().Contains("villid"))?.Split(':').LastOrDefault();
                            var finish = Account.Driver.Chrome.FindElementsByClassName("constructionContainer").FirstOrDefault()?.FindElements(By.ClassName("progressbar")).FirstOrDefault()?.GetAttribute("finish-time");
                            var currentTime = Account.Driver.Chrome.FindElementsById("servertime").FirstOrDefault()?.FindElements(By.ClassName("clickable")).FirstOrDefault(x => x.TagName == "span")?.GetAttribute("i18ndt");
                            var queueType = Account.Driver.Chrome.FindElementsByClassName("buildingSlotImage").FirstOrDefault()?.GetAttribute("className").Split(' ')
                                                   .FirstOrDefault(x => x.Contains("queueType")).Last();
                            var qTypeNumber = 0;
                            if (queueType != null)
                                qTypeNumber = queueType == '1' ? 1 : 2;
                            if (!string.IsNullOrEmpty(finish) && !string.IsNullOrEmpty(currentTime) && qTypeNumber != 0 && !string.IsNullOrEmpty(vid))
                            {
                                var time = Convert.ToInt32(Convert.ToDouble(finish.Replace(".", ","))) - Convert.ToInt32(Convert.ToDouble(currentTime.Replace(".", ",")));
                                if (time < 298)
                                {
                                    try
                                    {
                                        Account.Driver.FinishNow(Convert.ToInt32(vid), qTypeNumber, 0);
                                        Thread.Sleep(500);
                                    }
                                    catch (Exception e)
                                    {

                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            
                        }
                        
                    }
                    //Account.Driver.GetCache(Account.Player.VillageList.Select(x => $"BuildingQueue:{x.Id}").ToList());
                    //foreach (var x in Account.Player.VillageList.ToList())
                    //{
                    //    var q1 = x.Queue.Queue.FirstOrDefault(c => c.idq == 1);
                    //    var q2 = x.Queue.Queue.FirstOrDefault(c => c.idq == 2);
                    //    var q5 = x.Queue.Queue.FirstOrDefault(c => c.idq == 5);
                    //    if (q1 != default((int, int, int, int)))
                    //        if (q1.finishTime - x.Queue.UpdateTimeStamp < 295)
                    //            Account.Driver.FinishNow(x.Id, 1, 0);
                    //    if (q2 != default((int, int, int, int)))
                    //        if (q2.finishTime - x.Queue.UpdateTimeStamp < 295)
                    //            Account.Driver.FinishNow(x.Id, 2, 0);
                    //    if (q5 != default((int, int, int, int)))
                    //        if (q5.finishTime - x.Queue.UpdateTimeStamp < 295)
                    //            Account.Driver.FinishNow(x.Id, 5, 0);
                    //}
                }

                Thread.Sleep(500);
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                NotBlockWait = true;
            });
        }
    }
}
