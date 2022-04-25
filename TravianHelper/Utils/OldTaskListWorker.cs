using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using TravianHelper.StaticData;
using TravianHelper.TravianEntities;
using TravianHelper.UI;

namespace TravianHelper.Utils
{
    public class OldTaskListWorker : NotificationObject
    {
        private TaskState _state;

        public TaskState State
        {
            get => _state;
            set
            {
                _state = value;
                RaisePropertyChanged(() => State);
            }
        }

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
                //Working = false;
                //NotBlockWait = true;
                //State = TaskState.Queue;
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
            State           = TaskState.Queue;
        }

        public void Init()
        {
            
            Application.Current.Dispatcher.Invoke(() => { NotBlockWait = false; TaskList.Clear(); });
            if (!File.Exists($"{g.UserDataPath}\\TaskList.txt"))
            {
                File.WriteAllText($"{g.UserDataPath}\\TaskList.txt", "BuildingUpgrade:24:10/Склад >1\r\nFinishNow:0:1/Закончить\r\nBuildingUpgrade:25:11/Амбар >1\r\nFinishNow:0:1/Закончить\r\nBuildingUpgrade:26:17/Рынок >1\r\nFinishNow:0:1/Закончить\r\nBuildingUpgrade:28:18/Посольство >1\r\nFinishNow:0:1/Закончить\r\nBuildingUpgrade:23:8/Собрать руины\r\nFinishNow:0:5/Закончить\r\nBuildingUpgrade:20:10/Собрать руины\r\nFinishNow:0:5/Закончить\r\nBuildingUpgrade:35:11/Собрать руины\r\nFinishNow:0:5/Закончить\r\nBuildingUpgrade:38:13/Собрать руины\r\nFinishNow:0:5/Закончить\r\nBuildingUpgrade:39:17/Собрать руины\r\nFinishNow:0:5/Закончить\r\nBuildingUpgrade:31:22/Собрать руины\r\nFinishNow:0:5/Закончить\r\nBuildingUpgrade:24:10/Склад >2\r\nFinishNow:0:1/Закончить\r\nBuildingUpgrade:24:10/Склад >3\r\nFinishNow:0:1/Закончить\r\nBuildingUpgrade:25:11/Амбар >2\r\nFinishNow:0:1/Закончить\r\nBuildingUpgrade:25:11/Амбар >3\r\nFinishNow:0:1/Закончить\r\nBuildingUpgrade:25:11/Амбар >4\r\nWait:10/Ждать 10 сек\r\nFinishNow:0:1/Закончить\r\nCollectReward:/Собрать награды\r\nBuildingUpgrade:29:19/Казарма >2\r\nFinishNow:0:1/Закончить\r\nBuildingUpgrade:29:19/Казарма >2\r\nFinishNow:0:1/Закончить\r\nCollectReward:/Собрать награды\r\nBuildingUpgrade:34:18/Собрать руины\r\nFinishNow:0:5/Закончить\r\nRecruitUnits:29:19:11:5/5 Дубин\r\nCollectReward:/Собрать награды\r\nRecruitUnits:29:19:11:20/20 Дубин\r\nCollectReward:/Собрать награды\r\nBuildingUpgrade:21:23/Тайник >1\r\nFinishNow:0:1/Закончить\r\nCollectReward:/Собрать награды\r\nBuildingUpgrade:8:4/Ферма >1\r\nFinishNow:0:2/Закончить\r\nBuildingUpgrade:9:4/Ферма >1\r\nFinishNow:0:2/Закончить\r\nBuildingUpgrade:12:4/Ферма >1\r\nFinishNow:0:2/Закончить\r\nBuildingUpgrade:13:4/Ферма >1\r\nFinishNow:0:2/Закончить\r\nBuildingUpgrade:15:4/Ферма >1\r\nFinishNow:0:2/Закончить\r\nBuildingUpgrade:2:4/Ферма >2\r\nFinishNow:0:2/Закончить\r\nBuildingUpgrade:8:4/Ферма >2\r\nFinishNow:0:2/Закончить\r\nBuildingUpgrade:9:4/Ферма >2\r\nFinishNow:0:2/Закончить\r\nBuildingUpgrade:12:4/Ферма >2\r\nFinishNow:0:2/Закончить\r\nBuildingUpgrade:13:4/Ферма >2\r\nFinishNow:0:2/Закончить\r\nBuildingUpgrade:15:4/Ферма >2\r\nFinishNow:0:2/Закончить\r\nCollectReward:/Собрать награды\r\nBuildingUpgrade:2:4/Ферма >3\r\nFinishNow:0:2/Закончить\r\nBuildingUpgrade:8:4/Ферма >3\r\nFinishNow:0:2/Закончить\r\nBuildingUpgrade:9:4/Ферма >3\r\nFinishNow:0:2/Закончить\r\nBuildingUpgrade:12:4/Ферма >3\r\nFinishNow:0:2/Закончить\r\nBuildingUpgrade:13:4/Ферма >3\r\nFinishNow:0:2/Закончить\r\nBuildingUpgrade:15:4/Ферма >3\r\nFinishNow:0:2/Закончить\r\nCollectReward:/Собрать награды\r\nBuildingUpgrade:27:15/ГЗ >4\r\nFinishNow:0:1/Закончить\r\nBuildingUpgrade:27:15/ГЗ >5\r\nWait:700/Ждать 700 сек\r\nTrade:/Торговля\r\nHeroAttribute:0:0:1/Героя на дерево\r\nHeroAttribute:0:0:0/Героя на все\r\nCollectReward:/Собрать награды\r\nBuildingUpgrade:24:10/Склад >4\r\nFinishNow:0:1/Закончить\r\nBuildingUpgrade:24:10/Склад >5\r\nFinishNow:1:1/Закончить\r\nBuildingUpgrade:25:11/Амбар >5\r\nFinishNow:1:1/Закончить\r\nCollectReward:/Собрать награды\r\nBuildingUpgrade:24:10/Склад >6\r\nFinishNow:1:1/Закончить\r\nBuildingUpgrade:34:25/Реза >1\r\nWait:390/Ждать 390 сек\r\nCollectReward:/Собрать награды\r\nBuildingUpgrade:34:25/Реза >2\r\nWait:480/Ждать 480 сек\r\nCollectReward:/Собрать награды\r\nBuildingUpgrade:34:25/Реза >3\r\nWait:580/Ждать 580 сек\r\nBuildingUpgrade:34:25/Реза >4\r\nWait:760/Ждать 760 сек\r\nBuildingUpgrade:34:25/Реза >5\r\nWait:1270/Ждать 1270 сек\r\nCollectReward:/Собрать награды\r\nNpcT:1/Нпс на поселка 1\r\nRecruitUnits:34:25:20:1/Нанять поселка 1\r\nNpcT:1/Нпс на поселка 2\r\nRecruitUnits:34:25:20:1/Нанять поселка 2\r\nNpcT:1/Нпс на поселка 3\r\nRecruitUnits:34:25:20:1/Нанять поселка 3\r\nCollectReward:/Собрать награды\r\nNpcT:2/Нпс торговец\r\nBuildingUpgrade:27:15/ГЗ >6\r\nFinishNow:1:1/Закончить\r\nBuildingUpgrade:27:15/ГЗ >7\r\nFinishNow:1:1/Закончить\r\nBuildingUpgrade:27:15/ГЗ >8\r\nFinishNow:1:1/Закончить\r\nBuildingUpgrade:27:15/ГЗ >9\r\nFinishNow:1:1/Закончить\r\nBuildingUpgrade:27:15/ГЗ >10\r\nFinishNow:1:1/Закончить\r\nBuildingDestroy:34/Снос резы\r\nBuildingUpgrade:38:22/Академия >1\r\nFinishNow:0:1/Закончить\r\nBuildingUpgrade:38:22/Академия >2\r\nFinishNow:0:1/Закончить\r\nBuildingUpgrade:38:22/Академия >3\r\nFinishNow:0:1/Закончить\r\nBuildingUpgrade:38:22/Академия >4\r\nFinishNow:0:1/Закончить\r\nBuildingUpgrade:38:22/Академия >5\r\nFinishNow:1:1/Закончить\r\nBuildingUpgrade:38:22/Академия >6\r\nWait:5400/Ждать 5400 сек\r\nBuildingUpgrade:34:25/Собрать руины\r\nWait:720/Ждать 720 сек\r\nBuildingUpgrade:38:22/Академия >7\r\nFinishNow:1:1/Закончить\r\nBuildingUpgrade:38:22/Академия >8\r\nFinishNow:1:1/Закончить\r\nBuildingUpgrade:38:22/Академия >9\r\nFinishNow:1:1/Закончить\r\nCollectReward:/Собрать награды\r\nBuildingUpgrade:38:22/Академия >10\r\nFinishNow:1:1/Закончить\r\nBuildingUpgrade:37:21/Мастерская >1\r\nFinishNow:0:1/Закончить\r\nCollectReward:/Собрать награды\r\nBuildingDestroy:21/Снести тайник\r\nWait:1/Ждать 1 сек\r\nBuildingUpgrade:21:23/Собрать руины\r\nWait:1/Ждать 1 сек\r\nBuildingDestroy:26/Снести рынок\r\nFinishNow:0:5/Закончить\r\nBuildingUpgrade:26:17/Собать руины\r\nFinishNow:0:5/Закончить\r\nBuildingDestroy:28/Снести посольство\r\nFinishNow:0:5/Закончить\r\nBuildingUpgrade:28:18/Собать руины\r\nFinishNow:0:5/Закончить\r\nBuildingDestroy:29/Снести казарму\r\nFinishNow:0:5/Закончить\r\nBuildingUpgrade:29:19/Собать руины\r\nFinishNow:0:5/Закончить\r\nBuildingDestroy:37/Снести мастерскую\r\nFinishNow:0:5/Закончить\r\nBuildingUpgrade:37:21/Собать руины\r\nFinishNow:0:5/Закончить\r\nBuildingUpgrade:40:24/Ратуша >1\r\nFinishNow:0:1/Закончить\r\nBuildingDestroy:38/Снести академию\r\nFinishNow:1:5/Закончить\r\nBuildingUpgrade:38:22/Собать руины\r\nFinishNow:0:5/Закончить\r\nBuildingDestroy:33/Снести стену\r\nFinishNow:0:5/Закончить\r\nBuildingUpgrade:33:32/Собать руины\r\nFinishNow:0:5/Закончить\r\nCollectReward:/Собрать награды\r\nBuildingUpgrade:25:11/Амбар >6\r\nFinishNow:1:1/Закончить\r\nBuildingUpgrade:25:11/Амбар >7\r\nFinishNow:1:1/Закончить\r\nBuildingDestroy:27/Снести ГЗ\r\nFinishNow:1:5/Закончить\r\nBuildingUpgrade:27:15/Собать руины\r\nFinishNow:0:5/Закончить\r\nNpcT:3/Нпс на праздник");
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
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        SelectedTask                                             = TaskList.FirstOrDefault();
                        SelectedTask                                             = TaskList[Account.CurrentTaskId];
                                                              SelectedTask.State = TaskState.Queue;
                                                          });
                }

                for (var i = 0; i < TaskList.Count && i < Account.CurrentTaskId; i++)
                {
                    var x = TaskList[i];
                    Application.Current.Dispatcher.Invoke(() => { x.State = TaskState.Finished; });
                }

                Application.Current.Dispatcher.Invoke(() => { NotBlockWait = true; });
            }

            ShowTaskList = ShowTaskList ? true : Account.CurrentTaskId > 0 && Account.CurrentTaskId < TaskList.Count;
            Application.Current.Dispatcher.Invoke(() =>
                                                  {
                                                      SelectedTask = TaskList.FirstOrDefault();
                                                      if(Account.CurrentTaskId < TaskList.Count)
                                                        SelectedTask = TaskList[Account.CurrentTaskId];
                                                  });
        }

        private void OnShowTaskList()
        {
            ShowTaskList = !ShowTaskList;
        }

        private void Run()
        {
            _workThread = new Thread(ThreadFunc);
            _workThread.Start();
            State = TaskState.InProgress;
        }

        private void ThreadFunc()
        {
            while (Working)
            {
                if (Account.CurrentTaskId >= TaskList.Count)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                                                          {
                                                              State        = TaskState.Queue;
                                                              Working      = false;
                                                              NotBlockWait = true;
                                                          });
                    return;
                }
                Application.Current.Dispatcher.Invoke(() =>
                {
                    SelectedTask = TaskList[Account.CurrentTaskId];
                });
                var res = SelectedTask.Exec();
                if (res != "")
                {
                    Logger.Error($"{Account.NameWithNote}: {SelectedTask.Comment}");
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        State = TaskState.Error;
                    });
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
                                                      if(State != TaskState.Error)
                                                        State        = TaskState.Queue;
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
            return Comment;
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
                Application.Current.Dispatcher.Invoke(() => { State = TaskState.InProgress; });
                if (Account.Player.UpdateQuestList())
                    foreach (var x in Account.Player.QuestList.ToList().Where(x => x.IsCompleted))
                        Account.Driver.CollectReward(Account.Player.VillageList.First().Id, x.Id);
                else if (Account.Player.UpdateQuestList())
                    foreach (var x in Account.Player.QuestList.ToList().Where(x => x.IsCompleted))
                        Account.Driver.CollectReward(Account.Player.VillageList.First().Id, x.Id);
                else if (Account.Player.UpdateQuestList())
                    foreach (var x in Account.Player.QuestList.ToList().Where(x => x.IsCompleted))
                        Account.Driver.CollectReward(Account.Player.VillageList.First().Id, x.Id);
                Application.Current.Dispatcher.Invoke(() => { State = TaskState.Finished; });
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
                                                          State = Worker.Working ? TaskState.Finished : TaskState.Queue;
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
                        Thread.Sleep(5000);
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
                        Thread.Sleep(5000);
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
                            return true;
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
                            return true;
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
                    if (qq.FinishTime - q.UpdateTimeStamp < 298)
                    {
                        var aa = Account.Driver.FinishNow(Account.Player.VillageList.FirstOrDefault().Id, 1, Account.Player.Hero.HasBuildItem ? -1 : 1);
                        if (!aa)
                        {
                            var qwe = 123;
                            Logger.Error($"{Account.NameWithNote}: FinishNow 0 Error 1");
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
                        Logger.Error($"{Account.NameWithNote}: FinishNow 1 Error 1");
                    }
                    return aa;
                }
            }
            if (q.QueueList.Count(x => x.QueueId == 2) != 0)
            {
                var qq = q.QueueList.First(x => x.QueueId == 2);
                if (price == 0)
                {
                    if (qq.FinishTime - q.UpdateTimeStamp < 298)
                    {
                        var aa = Account.Driver.FinishNow(Account.Player.VillageList.FirstOrDefault().Id, 2, Account.Player.Hero.HasBuildItem ? -1 : 1);
                        if (!aa)
                        {
                            var qwe = 123;
                            Logger.Error($"{Account.NameWithNote}: FinishNow 0 Error 2");
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
                        Logger.Error($"{Account.NameWithNote}: FinishNow 1 Error 2");
                    }
                    return aa;
                }
            }
            if (q.QueueList.Count(x => x.QueueId == 5) != 0)
            {
                var qq = q.QueueList.First(x => x.QueueId == 5);
                if (price == 0)
                {
                    if (qq.FinishTime - q.UpdateTimeStamp < 298)
                    {
                        var aa = Account.Driver.FinishNow(Account.Player.VillageList.FirstOrDefault().Id, 5, Account.Player.Hero.HasBuildItem ? -1 : 1);
                        if (!aa)
                        {
                            var qwe = 123;
                            Logger.Error($"{Account.NameWithNote}: FinishNow 0 Error 5");
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
                        Logger.Error($"{Account.NameWithNote}: FinishNow 1 Error 5");
                    }

                    return aa;
                }
            }
            Thread.Sleep(1000);
            Logger.Error($"{Account.NameWithNote}: FinishNow Error");
            return false;
        }

        private bool HeroAttribute(int fightStrengthPoints, int resBonusPoints, int resBonusType)
        {
            var data = Account.Driver.Post(RPG.HeroAttribute(Account.Driver.GetSession(), fightStrengthPoints, 0, 0, resBonusPoints, resBonusType), out var error);
            if (!string.IsNullOrEmpty(error) || data.time == null) return false;
            Account.Update(data, (long)data.time);
            return true;
        }

        private bool Trade()
        {
            var data = Account.Driver.Post(RPG.Trade(Account.Driver.GetSession(), Account.Player.VillageList.FirstOrDefault().Id, 2, 1, 1, 1, false), out var error);
            if (!string.IsNullOrEmpty(error) || data.time == null) return false;
            Account.Update(data, (long)data.time);
            return true;
        }
    }
}
