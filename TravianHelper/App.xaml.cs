using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using TravianHelper.UI;
using TravianHelper.Utils;

namespace TravianHelper
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            g.Init();
            base.OnStartup(e);
            Logger.Init();
            AppDomain.CurrentDomain.UnhandledException       += CurrentDomain_UnhandledException;
            Application.Current.DispatcherUnhandledException += CurrentOnDispatcherUnhandledException;
            var f  = new MainWindow();
            var vm = new MainWindowViewModel();
            f.DataContext         = vm;
            g.MainViewModel = vm;
            f.ShowDialog();
        }
        protected override void OnExit(ExitEventArgs e)
        {
            //g.Shutdown();
            //foreach (var x in g.TabManager.TabList)
            //{
            //    x.Account.Stop();
            //}
            base.OnExit(e);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.ErrorQ(e.ExceptionObject as Exception);
        }

        private void CurrentOnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Logger.ErrorQ(e.Exception);
        }
    }
}
