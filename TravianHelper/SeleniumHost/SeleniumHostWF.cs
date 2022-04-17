using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenQA.Selenium;

namespace TravianHelper.SeleniumHost
{
    public partial class SeleniumHostWF : UserControl
    {
        private static class UnsafeNativeMethods
        {
            [DllImport("user32")]
            public static extern IntPtr SetParent(IntPtr hWnd, IntPtr hWndParent);

            [DllImport("user32.dll", SetLastError = true)]
            public static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);
        }

        public SeleniumHostWF()
        {
            InitializeComponent();
        }

        public DriverService DriverService
        {
            get => _driverService;
            set
            {

                if (_driverService != null)
                    DetachDriverService();

                _driverService = value;

                if (_driverService != null)
                    AttachDriverService(_driverService);
            }
        }

        private DriverService _driverService;
        private IntPtr? _browserHandle;

        private void DetachDriverService()
        {
            if (_browserHandle.HasValue)
            {
                UnsafeNativeMethods.SetParent(_browserHandle.Value, IntPtr.Zero);
            }
        }

        public void AttachDriverService(DriverService service)
        {
            var driverProcess = Process.GetProcessById(service.ProcessId);

            var browserProcess = driverProcess
                                 .GetChildren()
                                 .First(p => p.ProcessName != "conhost");

            _browserHandle = browserProcess.MainWindowHandle;

            Resize += (sender, e) => { UnsafeNativeMethods.MoveWindow(_browserHandle.Value, -8, -43, Width + 16, Height + 8 + 43, true); };

            UnsafeNativeMethods.SetParent(_browserHandle.Value, Handle);
            UnsafeNativeMethods.MoveWindow(_browserHandle.Value, -8, 0, Width + 16, Height + 8, true);
        }
    }

    public static class ProcessExtensions
    {
        public static IEnumerable<Process> GetChildren(this Process parent)
        {
            var query = new ManagementObjectSearcher($@"
                SELECT *
                FROM Win32_Process
                WHERE ParentProcessId={parent.Id}");

            return from item in query.Get().OfType<ManagementBaseObject>()
                   let childProcessId = (int)(uint)item["ProcessId"]
                   select Process.GetProcessById(childProcessId);
        }

    }
}
