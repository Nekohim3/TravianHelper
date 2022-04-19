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
                if (Account.UseSingleBuild || Account.UseMultiBuild)
                {
                    if (_working == value)
                        return;
                    _working = value;
                    if (_working)
                        Run();
                    else
                        NotBlockWait = false;
                }
                else
                {
                    if (_working && !value)
                    {
                        _working     = false;
                        NotBlockWait = false;
                    }
                }
                

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

        private DateTime _lastMultiCheck = DateTime.MinValue;

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
                                if (time < 299)
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

                    if (Account.UseMultiBuild)
                    {
                        var check = false;
                        if (!Account.UseRandomDelay)
                        {
                            if ((DateTime.Now - _lastMultiCheck).TotalSeconds > Account.FastBuildDelayMin)
                                check = true;
                        }
                        else
                        {
                            var r     = new Random();
                            var delay = r.Next(Account.FastBuildDelayMin, Account.FastBuildDelayMax);
                            if ((DateTime.Now - _lastMultiCheck).TotalSeconds > delay)
                                check = true;
                        }

                        if (check)
                        {
                            Account.Driver.GetCache(Account.Player.VillageList.Select(x => $"BuildingQueue:{x.Id}").ToList());
                            foreach (var x in Account.Player.VillageList.ToList())
                            {
                                var q1 = x.Queue.QueueList.FirstOrDefault(c => c.QueueId == 1);
                                var q2 = x.Queue.QueueList.FirstOrDefault(c => c.QueueId == 2);
                                var q5 = x.Queue.QueueList.FirstOrDefault(c => c.QueueId == 5);
                                if (q1 != null)
                                    if (q1.FinishTime - x.Queue.UpdateTimeStamp < 299)
                                    {
                                        Account.Driver.FinishNow(x.Id, 1, 0);
                                        Thread.Sleep(500);
                                        _lastMultiCheck = DateTime.Now;
                                    }

                                if (q2 != null)
                                    if (q2.FinishTime - x.Queue.UpdateTimeStamp < 299)
                                    {
                                        Account.Driver.FinishNow(x.Id, 2, 0);
                                        Thread.Sleep(500);
                                        _lastMultiCheck = DateTime.Now;
                                    }

                                if (q5 != null)
                                    if (q5.FinishTime - x.Queue.UpdateTimeStamp < 299)
                                    {
                                        Account.Driver.FinishNow(x.Id, 5, 0);
                                        Thread.Sleep(500);
                                        _lastMultiCheck = DateTime.Now;
                                    }
                            }
                        }
                    }
                    
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
