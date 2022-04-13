﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.ViewModel;
using Newtonsoft.Json;
using TravianHelper.JsonDb;
using TravianHelper.Settings;
using TravianHelper.UI;

namespace TravianHelper
{
    public static class g
    {
        public static DataStore           Db            { get; set; }
        public static string              UserDataPath  { get; set; }
        public static MainWindowViewModel MainViewModel { get; set; }

        public static TabManager TabManager { get; set; }

        public static ObservableCollection<Proxy>        ProxyList        { get; set; } = new ObservableCollection<Proxy>();
        public static ObservableCollection<ServerConfig> ServerList { get; set; } = new ObservableCollection<ServerConfig>();

        static g()
        {
            UserDataPath = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            if (!Directory.Exists($"{UserDataPath}\\UserData"))
                Directory.CreateDirectory($"{UserDataPath}\\UserData");
            UserDataPath = $"{UserDataPath}\\UserData";
            Db           = new DataStore($"{UserDataPath}\\Database", reloadBeforeGetCollection:true);
            LoadAll();
            TabManager = new TabManager();
        }

        public static void LoadAll()
        {
            LoadProxyList();
            LoadServerList();
        }

        public static void LoadProxyList()
        {
            ProxyList.Clear();
            ProxyList.AddRange(Db.GetCollection<Proxy>().AsQueryable());
        }

        public static void LoadServerList()
        {
            ServerList.Clear();
            ServerList.AddRange(Db.GetCollection<ServerConfig>().AsQueryable());
        }
    }
}
