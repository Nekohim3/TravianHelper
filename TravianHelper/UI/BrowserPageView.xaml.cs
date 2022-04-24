using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

namespace TravianHelper.UI
{
    /// <summary>
    /// Interaction logic for BrowserPageView.xaml
    /// </summary>
    public partial class BrowserPageView : Page
    {
        public BrowserPageView()
        {
            InitializeComponent();
            
        }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //if (e.AddedItems.Count != 0)
            //    ((ListView)sender).ScrollIntoView(e.AddedItems[0]);
            //VirtualizingStackPanel vsp =
            //    (VirtualizingStackPanel)typeof(ItemsControl).InvokeMember("_itemsHost",
            //                                                              BindingFlags.Instance | BindingFlags.GetField | BindingFlags.NonPublic, null,
            //                                                              ListViewTask, null);

            //double scrollHeight = vsp.ScrollOwner.ScrollableHeight;

            //// itemIndex_ is index of the item which we want to show in the middle of the view
            //double offset = scrollHeight * ListViewTask. / ListViewTask.Items.Count;

            //vsp.SetVerticalOffset(offset);
        }
    }
}
