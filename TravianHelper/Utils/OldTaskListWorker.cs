using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using TravianHelper.StaticData;
using TravianHelper.TravianEntities;

namespace TravianHelper.Utils
{
    public class OldTaskListWorker : NotificationObject
    {
        private string _showButtonText;

        public string ShowButtonText
        {
            get => _showButtonText;
            set
            {
                _showButtonText = value;
                RaisePropertyChanged(() => ShowButtonText);
            }
        }
        private bool _showTaskList;

        public bool ShowTaskList
        {
            get => _showTaskList;
            set
            {
                _showTaskList  = value;
                ShowButtonText = _showTaskList ? "<<" : ">>";
                RaisePropertyChanged(() => ShowTaskList);
            }
        }

        private bool _working;

        public bool Working
        {
            get => _working;
            set
            {
                if (_working == value)
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

        private ObservableCollection<Task> _taskList = new ObservableCollection<Task>();

        public ObservableCollection<Task> TaskList
        {
            get => _taskList;
            set
            {
                _taskList = value;
                RaisePropertyChanged(() => TaskList);
            }
        }

        private Task _selectedTask;

        public Task SelectedTask
        {
            get => _selectedTask;
            set
            {
                _selectedTask = value;
                RaisePropertyChanged(() => SelectedTask);
                if (InstallCurrentTask)
                {
                    InstallCurrentTask = false;
                    Account.CurrentTaskId = TaskList.IndexOf(_selectedTask);
                    Account.Save();
                    Init();
                }
            }
        }

        private bool _installCurrentTask;
        public bool InstallCurrentTask
        {
            get => _installCurrentTask;
            set
            {
                _installCurrentTask = value;
                RaisePropertyChanged(() => InstallCurrentTask);
            }
        }

        private Thread _workThread;

        public DelegateCommand ShowTaskListCmd { get; }

        public OldTaskListWorker(Account acc)
        {
            Account         = acc;
            ShowTaskListCmd = new DelegateCommand(OnShowTaskList);
            NotBlockWait    = true;
        }

        public void Init()
        {
            
            Application.Current.Dispatcher.Invoke(() => { NotBlockWait = false; TaskList.Clear(); });
            if (!File.Exists($"{g.UserDataPath}\\TaskList.txt"))
            {
                File.WriteAllText($"{g.UserDataPath}\\TaskList.txt", "BuildingUpgrade:24:10/Построить склад\r\nFinishNow:0:1/\r\nBuildingUpgrade:25:11/\r\nFinishNow:0:1/\r\nBuildingUpgrade:26:17/\r\nFinishNow:0:1/\r\nBuildingUpgrade:28:18/\r\nFinishNow:0:1/\r\nBuildingUpgrade:23:8/\r\nFinishNow:0:5/\r\nBuildingUpgrade:20:10/\r\nFinishNow:0:5/\r\nBuildingUpgrade:35:11/\r\nFinishNow:0:5/\r\nBuildingUpgrade:38:13/\r\nFinishNow:0:5/\r\nBuildingUpgrade:39:17/\r\nFinishNow:0:5/\r\nBuildingUpgrade:31:22/\r\nFinishNow:0:5/\r\nBuildingUpgrade:24:10/\r\nFinishNow:0:1/\r\nBuildingUpgrade:24:10/\r\nFinishNow:0:1/\r\nBuildingUpgrade:25:11/\r\nFinishNow:0:1/\r\nBuildingUpgrade:25:11/\r\nFinishNow:0:1/\r\nBuildingUpgrade:25:11/\r\nWait:10/\r\nFinishNow:0:1/\r\nCollectReward:/\r\nBuildingUpgrade:29:19/\r\nFinishNow:0:1/\r\nBuildingUpgrade:29:19/\r\nFinishNow:0:1/\r\nCollectReward:/\r\nBuildingUpgrade:34:18/\r\nFinishNow:0:5/\r\nRecruitUnits:29:19:11:5/\r\nCollectReward:/\r\nRecruitUnits:29:19:11:20/\r\nCollectReward:/\r\nBuildingUpgrade:21:23/\r\nFinishNow:0:1/\r\nCollectReward:/\r\nBuildingUpgrade:8:4/\r\nFinishNow:0:2/\r\nBuildingUpgrade:9:4/\r\nFinishNow:0:2/\r\nBuildingUpgrade:12:4/\r\nFinishNow:0:2/\r\nBuildingUpgrade:13:4/\r\nFinishNow:0:2/\r\nBuildingUpgrade:15:4/\r\nFinishNow:0:2/\r\nBuildingUpgrade:2:4/\r\nFinishNow:0:2/\r\nBuildingUpgrade:8:4/\r\nFinishNow:0:2/\r\nBuildingUpgrade:9:4/\r\nFinishNow:0:2/\r\nBuildingUpgrade:12:4/\r\nFinishNow:0:2/\r\nBuildingUpgrade:13:4/\r\nFinishNow:0:2/\r\nBuildingUpgrade:15:4/\r\nFinishNow:0:2/\r\nCollectReward:/\r\nBuildingUpgrade:2:4/\r\nFinishNow:0:2/\r\nBuildingUpgrade:8:4/\r\nFinishNow:0:2/\r\nBuildingUpgrade:9:4/\r\nFinishNow:0:2/\r\nBuildingUpgrade:12:4/\r\nFinishNow:0:2/\r\nBuildingUpgrade:13:4/\r\nFinishNow:0:2/\r\nBuildingUpgrade:15:4/\r\nFinishNow:0:2/\r\nCollectReward:/\r\nBuildingUpgrade:27:15/\r\nFinishNow:0:1/\r\nBuildingUpgrade:27:15/\r\nWait:700/\r\nTrade:/\r\nHeroAttribute:0:0:1/\r\nHeroAttribute:0:0:0/\r\nCollectReward:/\r\nBuildingUpgrade:24:10/\r\nFinishNow:0:1/\r\nBuildingUpgrade:24:10/\r\nFinishNow:1:1/\r\nBuildingUpgrade:25:11/\r\nFinishNow:1:1/\r\nCollectReward:/\r\nBuildingUpgrade:24:10/\r\nFinishNow:1:1/\r\nBuildingUpgrade:34:25/\r\nWait:390/\r\nCollectReward:/\r\nBuildingUpgrade:34:25/\r\nWait:480/\r\nCollectReward:/\r\nNpcT:0/\r\nBuildingUpgrade:24:10/\r\nFinishNow:1:1/\r\nCollectReward:/\r\nBuildingUpgrade:34:25/\r\nWait:580/\r\nBuildingUpgrade:34:25/\r\nWait:760/\r\nBuildingUpgrade:34:25/\r\nWait:1270/\r\nCollectReward:/\r\nNpcT:1/\r\nRecruitUnits:34:25:20:1/\r\nNpcT:1/\r\nRecruitUnits:34:25:20:1/\r\nNpcT:1/\r\nRecruitUnits:34:25:20:1/\r\nCollectReward:/\r\nNpcT:2/\r\nBuildingUpgrade:27:15/\r\nFinishNow:1:1/\r\nBuildingUpgrade:27:15/\r\nFinishNow:1:1/\r\nBuildingUpgrade:27:15/\r\nFinishNow:1:1/\r\nBuildingUpgrade:27:15/\r\nFinishNow:1:1/\r\nBuildingUpgrade:27:15/\r\nFinishNow:1:1/\r\nBuildingDestroy:34/\r\nBuildingUpgrade:38:22/\r\nFinishNow:0:1/\r\nBuildingUpgrade:38:22/\r\nFinishNow:0:1/\r\nBuildingUpgrade:38:22/\r\nFinishNow:0:1/\r\nBuildingUpgrade:38:22/\r\nFinishNow:0:1/\r\nBuildingUpgrade:38:22/\r\nFinishNow:1:1/\r\nBuildingUpgrade:38:22/\r\nWait:5400/Снос резы\r\nBuildingUpgrade:34:25/\r\nWait:720/\r\nBuildingUpgrade:38:22/\r\nFinishNow:1:1/\r\nBuildingUpgrade:38:22/\r\nFinishNow:1:1/\r\nBuildingUpgrade:38:22/\r\nFinishNow:1:1/\r\nCollectReward:/\r\nBuildingUpgrade:38:22/\r\nFinishNow:1:1/\r\nBuildingUpgrade:37:21/\r\nFinishNow:0:1/\r\nCollectReward:/\r\nBuildingDestroy:21/\r\nWait:1/\r\nBuildingUpgrade:21:23/\r\nWait:1/\r\nBuildingDestroy:26/\r\nFinishNow:0:5/\r\nBuildingUpgrade:26:17/\r\nFinishNow:0:5/\r\nBuildingDestroy:28/\r\nFinishNow:0:5/\r\nBuildingUpgrade:28:18/\r\nFinishNow:0:5/\r\nBuildingDestroy:29/\r\nFinishNow:0:5/\r\nBuildingUpgrade:29:19/\r\nFinishNow:0:5/\r\nBuildingDestroy:37/\r\nFinishNow:0:5/\r\nBuildingUpgrade:37:21/\r\nFinishNow:0:5/\r\nBuildingUpgrade:40:24/\r\nFinishNow:0:1/\r\nBuildingDestroy:38/\r\nFinishNow:1:5/\r\nBuildingUpgrade:38:22/\r\nFinishNow:0:5/\r\nBuildingDestroy:33/\r\nFinishNow:0:5/\r\nBuildingUpgrade:33:32/\r\nFinishNow:0:5/\r\nCollectReward:/\r\nBuildingUpgrade:25:11/\r\nFinishNow:1:1/\r\nBuildingUpgrade:25:11/\r\nFinishNow:1:1/\r\nBuildingDestroy:27/\r\nFinishNow:1:5/\r\nBuildingUpgrade:27:15/\r\nFinishNow:0:5/\r\nNpcT:3/");
            }
            using (var fs = new FileStream($"{g.UserDataPath}\\TaskList.txt", FileMode.Open, FileAccess.Read))
            using (var sr = new StreamReader(fs))
            {
                while (!sr.EndOfStream)
                {
                    var str = sr.ReadLine().Split('/');
                    Application.Current.Dispatcher.Invoke(() => { TaskList.Add(new Task(this, Account, str[0], str[1])); });
                }
            }

            if (Account.CurrentTaskId >= 0)
            {
                if (Account.CurrentTaskId < TaskList.Count)
                {
                    SelectedTask = TaskList[Account.CurrentTaskId];
                    Application.Current.Dispatcher.Invoke(() => { SelectedTask.State = TaskState.Queue; });
                }

                for (var i = 0; i < TaskList.Count && i < Account.CurrentTaskId; i++)
                {
                    var x = TaskList[i];
                    Application.Current.Dispatcher.Invoke(() => { x.State = TaskState.Finished; });
                }

                Application.Current.Dispatcher.Invoke(() => { NotBlockWait = true; });
            }

            ShowTaskList      = ShowTaskList ? true : Account.CurrentTaskId > 0 && Account.CurrentTaskId < TaskList.Count;
        }

        private void OnShowTaskList()
        {
            ShowTaskList = !ShowTaskList;
        }

        private void Run()
        {
            _workThread = new Thread(ThreadFunc);
            _workThread.Start();
            //Account.UseFastBuilding = false;
            //Account.UserAutoAdv = true;
            //Account.UseRobber = true;
        }

        private void Stop()
        {
            if (_workThread == null) return;
            _workThread.Abort();
            _workThread = null;
        }

        private void ThreadFunc()
        {
            while (Working)
            {
                if (Account.CurrentTaskId >= TaskList.Count) return;
                SelectedTask = TaskList[Account.CurrentTaskId];
                var res = SelectedTask.Exec();
                if (res != "")
                {
                    Working = false;
                }
                else
                {
                    if (Working)
                    {
                        Application.Current.Dispatcher.Invoke(() => { Account.CurrentTaskId++; });
                        Account.Save();
                    }
                }
            }

            Application.Current.Dispatcher.Invoke(() =>
                                                  {
                                                      NotBlockWait = true;
                                                  });
        }
    }

    public class Task : NotificationObject
    {
        public  OldTaskListWorker Worker;
        private string            _action;
        private TaskState         _state;

        public string Action
        {
            get => _action;
            set
            {
                _action = value;
                RaisePropertyChanged(() => Action);
            }
        }

        public TaskState State
        {
            get => _state;
            set
            {
                _state = value;
                RaisePropertyChanged(() => State);
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

        public string Comment { get; set; }
        public Task(OldTaskListWorker worker, Account acc, string a, string c, TaskState t = TaskState.Queue)
        {
            Worker  = worker;
            Account = acc;
            Action  = a;
            State   = t;
            Comment = c;
        }

        public override string ToString()
        {
            return Action;
        }

        public string Exec()
        {
            var a = Action.Split(':');
            if (a[0] == "BuildingUpgrade")
            {
                Application.Current.Dispatcher.Invoke(() => {
                                                          State = TaskState.InProgress;
                                                      });
                if (!BuildingUpgrade(Convert.ToInt32(a[1]), Convert.ToInt32(a[2]), a.Length == 4 && Convert.ToBoolean(a[3])))
                    if (!BuildingUpgrade(Convert.ToInt32(a[1]), Convert.ToInt32(a[2]), true))
                        if (!BuildingUpgrade(Convert.ToInt32(a[1]), Convert.ToInt32(a[2]), true))
                        {
                            Application.Current.Dispatcher.Invoke(() => {
                                                                      State = TaskState.Error;
                                                                  });
                            return "err";
                        }

                Application.Current.Dispatcher.Invoke(() => {
                                                          State = TaskState.Finished;
                                                      });
                return "";
            }

            if (a[0] == "CollectReward")
            {
                Application.Current.Dispatcher.Invoke(() => {
                                                          State = TaskState.InProgress;
                                                      });
                Account.Player.UpdateQuestList();
                foreach (var x in Account.Player.QuestList.ToList().Where(x => x.IsCompleted))
                    Account.Driver.CollectReward(Account.Player.VillageList.First().Id, x.Id);
                Account.Player.UpdateQuestList();
                foreach (var x in Account.Player.QuestList.ToList().Where(x => x.IsCompleted))
                    Account.Driver.CollectReward(Account.Player.VillageList.First().Id, x.Id);
                Account.Player.UpdateQuestList();
                foreach (var x in Account.Player.QuestList.ToList().Where(x => x.IsCompleted))
                    Account.Driver.CollectReward(Account.Player.VillageList.First().Id, x.Id);
                Application.Current.Dispatcher.Invoke(() => {
                                                          State = TaskState.Finished;
                                                      });
                return "";
            }

            if (a[0] == "RecruitUnits")
            {
                Application.Current.Dispatcher.Invoke(() => {
                                                          State = TaskState.InProgress;
                                                      });
                if (!RecruitUnits(Convert.ToInt32(a[1]), Convert.ToInt32(a[2]), a[3], Convert.ToInt32(a[4])))
                    if (!RecruitUnits(Convert.ToInt32(a[1]), Convert.ToInt32(a[2]), a[3], Convert.ToInt32(a[4])))
                        if (!RecruitUnits(Convert.ToInt32(a[1]), Convert.ToInt32(a[2]), a[3], Convert.ToInt32(a[4])))
                        {
                            Application.Current.Dispatcher.Invoke(() => {
                                                                      State = TaskState.Error;
                                                                  });
                            return "err";
                        }
                Application.Current.Dispatcher.Invoke(() => {
                                                          State = TaskState.Finished;
                                                      });
                return "";
            }

            if (a[0] == "NpcT")
            {
                Application.Current.Dispatcher.Invoke(() => {
                                                          State = TaskState.InProgress;
                                                      });
                if (!NpcT(Convert.ToInt32(a[1])))
                    if (!NpcT(Convert.ToInt32(a[1])))
                        if (!NpcT(Convert.ToInt32(a[1])))
                        {
                            Application.Current.Dispatcher.Invoke(() => {
                                                                      State = TaskState.Error;
                                                                  });
                            return "err";
                        }
                Application.Current.Dispatcher.Invoke(() => {
                                                          State = TaskState.Finished;
                                                      });
                return "";
            }

            if (a[0] == "BuildingDestroy")
            {
                Application.Current.Dispatcher.Invoke(() => {
                                                          State = TaskState.InProgress;
                                                      });
                if (!BuildingDestroy(Convert.ToInt32(a[1])))
                    if (!BuildingDestroy(Convert.ToInt32(a[1])))
                        if (!BuildingDestroy(Convert.ToInt32(a[1])))
                        {
                            Application.Current.Dispatcher.Invoke(() => {
                                                                      State = TaskState.Error;
                                                                  });
                            return "err";
                        }
                Application.Current.Dispatcher.Invoke(() => {
                                                          State = TaskState.Finished;
                                                      });
                return "";
            }

            if (a[0] == "UseItem")
            {
                Application.Current.Dispatcher.Invoke(() => {
                                                          State = TaskState.InProgress;
                                                      });
                if (!UseItem(Convert.ToInt32(a[1]), Convert.ToInt32(a[2])))
                    if (!UseItem(Convert.ToInt32(a[1]), Convert.ToInt32(a[2])))
                        if (!UseItem(Convert.ToInt32(a[1]), Convert.ToInt32(a[2])))
                        {
                            Application.Current.Dispatcher.Invoke(() => {
                                                                      State = TaskState.Error;
                                                                  });
                            return "err";
                        }
                Application.Current.Dispatcher.Invoke(() => {
                                                          State = TaskState.Finished;
                                                      });
                return "";
            }

            if (a[0] == "FinishNow")
            {
                Application.Current.Dispatcher.Invoke(() => {
                                                          State = TaskState.InProgress;
                                                      });
                var p = a[1];
                if (p != "0")
                    p = Account.Player.Hero.HasBuildItem ? "-1" : "1";
                if (!FinishNow(Convert.ToInt32(p), Convert.ToInt32(a[2]), false))
                    if (!FinishNow(Convert.ToInt32(p), Convert.ToInt32(a[2]), true))
                        if (!FinishNow(Convert.ToInt32(p), Convert.ToInt32(a[2]), true))
                            if (!FinishNow(Convert.ToInt32(p), Convert.ToInt32(a[2]), true))
                                if (!FinishNow(Convert.ToInt32(p), Convert.ToInt32(a[2]), true))
                                {
                                    Application.Current.Dispatcher.Invoke(() => {
                                                                              State = TaskState.Error;
                                                                          });
                                    return "err";
                                }

                Application.Current.Dispatcher.Invoke(() => {
                                                          State = TaskState.Finished;
                                                      });
                return "";
            }

            if (a[0] == "HeroAttribute")
            {
                Application.Current.Dispatcher.Invoke(() => {
                                                          State = TaskState.InProgress;
                                                      });
                if (!HeroAttribute(Convert.ToInt32(a[1]), Convert.ToInt32(a[2]), Convert.ToInt32(a[3])))
                    if (!HeroAttribute(Convert.ToInt32(a[1]), Convert.ToInt32(a[2]), Convert.ToInt32(a[3])))
                        if (!HeroAttribute(Convert.ToInt32(a[1]), Convert.ToInt32(a[2]), Convert.ToInt32(a[3])))
                        {
                            Application.Current.Dispatcher.Invoke(() => {
                                                                      State = TaskState.Error;
                                                                  });
                            return "err";
                        }
                Application.Current.Dispatcher.Invoke(() => {
                                                          State = TaskState.Finished;
                                                      });
                return "";
            }

            if (a[0] == "Trade")
            {
                Application.Current.Dispatcher.Invoke(() => {
                                                          State = TaskState.InProgress;
                                                      });
                if (!Trade())
                    if (!Trade())
                        if (!Trade())
                        {
                            Application.Current.Dispatcher.Invoke(() => {
                                                                      State = TaskState.Error;
                                                                  });
                            return "err";
                        }
                Application.Current.Dispatcher.Invoke(() => {
                                                          State = TaskState.Finished;
                                                      });
                return "";
            }

            if (a[0] == "Wait")
            {
                Application.Current.Dispatcher.Invoke(() => {
                                                          State = TaskState.InProgress;
                                                      });
                for (var i = 0; i < Convert.ToInt32(a[1]) * 2; i++)
                {
                    if (!Worker.Working)
                    {
                        Application.Current.Dispatcher.Invoke(() => {
                                                                  State = TaskState.Queue;
                                                              });
                        return "";
                    }
                    Thread.Sleep(500);
                }

                if (!Worker.Working) return "";
                    Account.Player.UpdateQuestList();
                foreach (var x in Account.Player.QuestList.ToList().Where(x => x.IsCompleted))
                {
                    Account.Driver.CollectReward(Account.Player.VillageList.First().Id, x.Id);
                }
                Application.Current.Dispatcher.Invoke(() => {
                                                          State = TaskState.Finished;
                                                      });
            }

            return "";
        }

        private bool BuildingUpgrade(int locationId, int buildingType, bool check = false)
        {
            var vil = Account.Player.VillageList.First();
            if (check)
            {
                vil.Update();
                vil.UpdateBuildingList();
                if (vil.BuildingList.First(x => x.Location == locationId).BuildingType != 0)
                {
                    while (!vil.Storage.IsGreaterOrEq(vil.BuildingList.First(x => x.Location == locationId).UpgradeCost))
                    {
                        Thread.Sleep(10000);
                        if (!Worker.Working)
                        {
                            return false;
                        }
                        Account.Player.UpdateQuestList();
                        foreach (var x in Account.Player.QuestList.ToList().Where(x => x.IsCompleted))
                        {
                            Account.Driver.CollectReward(Account.Player.VillageList.First().Id, x.Id);
                        }
                        vil.Update();
                    }
                }
                else
                {
                    while (!vil.Storage.IsGreaterOrEq(BuildingsData.GetById(buildingType).BuildRes))
                    {
                        Thread.Sleep(10000);
                        if (!Worker.Working)
                        {
                            return false;
                        }
                        Account.Player.UpdateQuestList();
                        foreach (var x in Account.Player.QuestList.ToList().Where(x => x.IsCompleted))
                        {
                            Account.Driver.CollectReward(Account.Player.VillageList.First().Id, x.Id);
                        }
                        vil.Update();
                    }
                }
            }

            return Account.Driver.BuildingUpgrade(vil.Id, locationId, buildingType);
        }

        private bool CollectReward(int questId)
        {
            return Account.Driver.CollectReward(Account.Player.VillageList.First().Id, questId);
        }
        private bool BuildingDestroy(int locationId)
        {
            return Account.Driver.BuildingDestroy(Account.Player.VillageList.First().Id, locationId);
        }

        private bool NpcT(int id)
        {
            var vil = Account.Player.VillageList.First();
            var r1 = 0;
            var r2 = 0;
            var r3 = 0;
            var r4 = 0;
            vil.Update();
            if (id == 0)
            {
                r1 = 1250;
                r2 = 1000;
                r3 = 1000;
                r4 = 1000;
            }
            if (id == 1)
            {
                while (vil.Storage.MultiRes < 10700)
                {
                    for (var i = 0; i < 20; i++)
                    {
                        Thread.Sleep(500);
                        if (!Worker.Working)
                        {
                            return false;
                        }
                    }
                    vil.Update();
                }
                r1 = 4000;
                r2 = 3500;
                r3 = 3200;
                r4 = 0;
            }
            if (id == 2)
            {
                r1 = 5815 + (vil.Storage.MultiRes - 14570) / 4;
                r2 = 3790 + (vil.Storage.MultiRes - 14570) / 4;
                r3 = 3620 + (vil.Storage.MultiRes - 14570) / 4;
                r4 = 1345 + (vil.Storage.MultiRes - 14570) / 4;
            }
            if (id == 3)
            {
                while (vil.Storage.MultiRes < 20330)
                {
                    for (var i = 0; i < 20; i++)
                    {
                        Thread.Sleep(500);
                        if (!Worker.Working)
                        {
                            return false;
                        }
                    }
                    vil.Update();
                }
                r1 = 3800;
                r2 = 4000;
                r3 = 3030;
                r4 = 9500;
            }

            return Account.Driver.NpcTrade(vil.Id, new Resource(r1, r2, r3, r4, -1));
        }

        private bool UseItem(int id, int amount)
        {
            return Account.Driver.UseHeroItem(amount, id, Account.Player.VillageList.First().Id);
        }

        private bool RecruitUnits(int locationId, int buildingType, string uid, int amount)
        {
            return Account.Driver.RecruitUnits(Account.Player.VillageList.FirstOrDefault().Id, locationId, buildingType, uid, amount);
        }

        private bool FinishNow(int price, int queueType, bool updateItems)
        {
            if (updateItems)
            {
                Account.Player.Hero.UpdateItems();
                Account.Player.VillageList.First().UpdateBuildingQueue();
            }

            var p = 0;
            var q = Account.Player.VillageList.First().Queue;
            if (q.QueueList.Count(x => x.QueueId == 1) != 0)
            {
                var qq = q.QueueList.First(x => x.QueueId == 1);
                if (price == 0)
                {
                    if (qq.FinishTime - q.UpdateTimeStamp < 295)
                    {
                        Account.Player.Hero.UpdateItems();
                        var aa = Account.Driver.FinishNow(Account.Player.VillageList.FirstOrDefault().Id, 1, Account.Player.Hero.HasBuildItem ? -1 : 1);
                        if (!aa)
                        {
                            var qwe = 123;
                        }
                        return aa;
                    }
                }
                else
                {
                    var aa = Account.Driver.FinishNow(Account.Player.VillageList.FirstOrDefault().Id, 1, Account.Player.Hero.HasBuildItem ? -1 : 1);
                    if (!aa)
                    {
                        var qwe = 123;
                    }
                    return aa;
                }
            }
            if (q.QueueList.Count(x => x.QueueId == 2) != 0)
            {
                var qq = q.QueueList.First(x => x.QueueId == 2);
                if (price == 0)
                {
                    if (qq.FinishTime - q.UpdateTimeStamp < 295)
                    {
                        Account.Player.Hero.UpdateItems();
                        var aa = Account.Driver.FinishNow(Account.Player.VillageList.FirstOrDefault().Id, 2, Account.Player.Hero.HasBuildItem ? -1 : 1);
                        if (!aa)
                        {
                            var qwe = 123;
                        }
                        return aa;
                    }
                }
                else
                {
                    var aa = Account.Driver.FinishNow(Account.Player.VillageList.FirstOrDefault().Id, 2, Account.Player.Hero.HasBuildItem ? -1 : 1);
                    if (!aa)
                    {
                        var qwe = 123;
                    }
                    return aa;
                }
            }
            if (q.QueueList.Count(x => x.QueueId == 5) != 0)
            {
                var qq = q.QueueList.First(x => x.QueueId == 5);
                if (price == 0)
                {
                    if (qq.FinishTime - q.UpdateTimeStamp < 295)
                    {
                        Account.Player.Hero.UpdateItems();
                        var aa = Account.Driver.FinishNow(Account.Player.VillageList.FirstOrDefault().Id, 5, Account.Player.Hero.HasBuildItem ? -1 : 1);
                        if (!aa)
                        {
                            var qwe = 123;
                        }
                        return aa;
                    }
                }
                else
                {
                    var aa = Account.Driver.FinishNow(Account.Player.VillageList.FirstOrDefault().Id, 5, Account.Player.Hero.HasBuildItem ? -1 : 1);
                    if (!aa)
                    {
                        var qwe = 123;
                    }

                    return aa;
                }
            }
            Thread.Sleep(3000);
            return false;
        }

        private bool HeroAttribute(int fightStrengthPoints, int resBonusPoints, int resBonusType)
        {
            var data = Account.Driver.PostJo(RPG.HeroAttribute(Account.Driver.GetSession(), fightStrengthPoints, 0, 0, resBonusPoints, resBonusType));
            Account.Update(data, (long)data.time);
            return true;
        }

        private bool Trade()
        {
            var data = Account.Driver.PostJo(RPG.Trade(Account.Driver.GetSession(), Account.Player.VillageList.FirstOrDefault().Id, 2, 1, 1, 1, false));
            Account.Update(data, (long)data.time);
            return true;
        }
    }
}
