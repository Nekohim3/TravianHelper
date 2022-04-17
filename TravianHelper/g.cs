using System;
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
        public static TabManager          TabManager    { get; set; }
        
        public static void Init()
        {
            UserDataPath = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            if (!Directory.Exists($"{UserDataPath}\\UserData"))
                Directory.CreateDirectory($"{UserDataPath}\\UserData");
            UserDataPath   = $"{UserDataPath}\\UserData";
            Db             = new DataStore($"{UserDataPath}\\Database", reloadBeforeGetCollection: true);
            TabManager     = new TabManager();
            TabManager.OpenSettingsTab();
        }
    }
}
