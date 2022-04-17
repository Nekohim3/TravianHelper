using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OpenQA.Selenium;

namespace TravianHelper.SeleniumHost
{
    /// <summary>
    /// Interaction logic for SeleniumHostWPF.xaml
    /// </summary>
    public partial class SeleniumHostWPF : UserControl
    {
        public SeleniumHostWPF()
        {
            InitializeComponent();
        }

        public DriverService DriverService
        {
            get => (DriverService)GetValue(DriverServiceProperty);
            set => SetValue(DriverServiceProperty, value);
        }

        public static readonly DependencyProperty DriverServiceProperty =
            DependencyProperty.Register(nameof(DriverService), typeof(DriverService), typeof(SeleniumHostWPF), new PropertyMetadata(default(DriverService), DriverServicePropertyChanged));

        private static void DriverServicePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is SeleniumHostWPF that) that.SeleniumHost.DriverService = e.NewValue as DriverService;
        }
    }
}
