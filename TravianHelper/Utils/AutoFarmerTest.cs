using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Practices.Prism.ViewModel;
using OpenQA.Selenium;
using OpenQA.Selenium.Internal;
using TravianHelper.TravianEntities;

namespace TravianHelper.Utils
{
    public class AutoFarmerTest : NotificationObject
    {
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

        private ObservableCollection<FarmEntity> _farmList;

        public ObservableCollection<FarmEntity> FarmList
        {
            get => _farmList;
            set
            {
                _farmList = value;
                RaisePropertyChanged(() => FarmList);
            }
        }

        private Thread _workThread;

        public AutoFarmerTest(Account acc)
        {
            Account = acc;
        }

        public void Init()
        {
            if (!File.Exists($"{g.UserDataPath}\\FarmList.txt"))
            {
                MessageBox.Show("Не найден фарм лист");
                return;
            }

            FarmList = new ObservableCollection<FarmEntity>();

            using (var fs = new FileStream($"{g.UserDataPath}\\farmlist.txt", FileMode.Open, FileAccess.Read))
            using (var sr = new StreamReader(fs))
                while (!sr.EndOfStream)
                {
                    var farm = new FarmEntity();
                    var strs = sr.ReadLine().Split(';');
                    if (strs.Length >= 2)
                        farm.Note = strs[1];
                    var prms = strs[0].Split(':');
                    farm.SId = Convert.ToInt32(prms[0]);
                    farm.DId = Convert.ToInt32(prms[1]);
                    farm.C1  = Convert.ToInt32(prms[2]);
                    farm.C2  = Convert.ToInt32(prms[3]);
                    farm.C3  = Convert.ToInt32(prms[4]);
                    farm.C4  = Convert.ToInt32(prms[5]);
                    farm.C5  = Convert.ToInt32(prms[6]);
                    farm.C6  = Convert.ToInt32(prms[7]);

                    FarmList.Add(farm);
                }

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
                try
                {
                    var vids = FarmList.Select(x => x.SId).Distinct().ToList();
                    foreach (var x in vids)
                    {
                        Account.Player.VillageList.FirstOrDefault(c => c.Id == x)?.UpdateStationaryTroops();
                    }
                }
                catch (Exception e)
                {
                    Logger.Error(e, "Error farmer");
                }

                Thread.Sleep(500);
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                NotBlockWait = true;
            });
        }
    }

    public class FarmEntity
    {
        public int      SId   { get; set; }
        public int      DId   { get; set; }
        public string   Note     { get; set; }
        public int      C1       { get; set; }
        public int      C2       { get; set; }
        public int      C3       { get; set; }
        public int      C4       { get; set; }
        public int      C5       { get; set; }
        public int      C6       { get; set; }
        public DateTime LastSend { get; set; } = DateTime.MinValue;
    }
}
