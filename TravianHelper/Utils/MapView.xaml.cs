using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Shapes;
using Newtonsoft.Json;

namespace TravianHelper.Utils
{
    /// <summary>
    /// Interaction logic for MapView.xaml
    /// </summary>
    public partial class MapView : Window
    {
        public MapView()
        {
            InitializeComponent();
            var flist = JsonConvert.DeserializeObject<List<Field>>(File.ReadAllText("testmap.txt"));
            var newflist = flist.Where(x => x.ResType != "0" || x.HasVillage || x.IsOasis).ToList();
            //var newflist = flist.Where(x => x.IsOasis && (x.UnitList.Count(c => c.Id == 7) != 0 || x.UnitList.Count(c => c.Id == 8) != 0 || x.UnitList.Count(c => c.Id == 9) != 0 || x.UnitList.Count(c => c.Id == 10) != 0)).ToList();
            //var newflist = flist.Where(x => x.IsOasis).ToList();
            //newflist = newflist.Where(x => x.ResType == "11115" || x.ResType == "3339" || x.IsOasis || x.HasVillage).ToList();

            foreach (var x in newflist)
            {
                Canvas.Children.Add(CreateFieldGrid(x));
            }
            b = new Border();
            b.HorizontalAlignment = HorizontalAlignment.Left;
            b.VerticalAlignment = VerticalAlignment.Top;
            b.Margin = new Thickness(0, 0, 0, 0);
            b.Width = 700;
            b.Height = 700;
            b.Visibility = Visibility.Collapsed;
            b.BorderThickness = new Thickness(5);
            b.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
            Canvas.Children.Add(b);
            //Canvas.Children.Add(CreateFieldGrid(newflist.First()));
            //Canvas.Children.Add(CreateFieldGrid(newflist.Last()));
        }

        Point m_start;
        private bool cap = false;
        private void TreeCanvas_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            m_start = e.GetPosition(MainGrid);
            cap = true;
        }

        private void TreeCanvas_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            cap = false;
        }

        private void MainGrid_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var canvasMatrix = CanvasMatrix.Matrix;

            var canvasPosition = e.GetPosition(Canvas);
            var sc = e.Delta >= 0 ? 1.1 : (1.0 / 1.1);


            canvasMatrix.ScaleAtPrepend(sc, sc, canvasPosition.X, canvasPosition.Y);


            CanvasMatrix.Matrix = canvasMatrix;
        }

        private void TreeCanvas_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (cap)
            {
                var offset = Point.Subtract(e.GetPosition(MainGrid), m_start);
                var canvasMatrix = CanvasMatrix.Matrix;

                canvasMatrix.Translate(offset.X, offset.Y);


                CanvasMatrix.Matrix = canvasMatrix;

                m_start = e.GetPosition(MainGrid);
            }
        }

        private Border b = new Border();
        private Grid CreateFieldGrid(Field field)
        {
            var g = new Grid();
            g.HorizontalAlignment = HorizontalAlignment.Left;
            g.VerticalAlignment = VerticalAlignment.Top;
            g.Margin = new Thickness(field.X * 101, -field.Y * 101, 0, 0);
            g.Width = 100;
            g.Height = 100;
            if (field.IsOasis && field.OasisBonus.Crop != 0)
            {
                g.Background = new SolidColorBrush(Color.FromArgb(255, 75, 75, 75));
                var tb = new TextBlock();
                tb.TextAlignment = TextAlignment.Center;
                tb.HorizontalAlignment = HorizontalAlignment.Center;
                tb.VerticalAlignment = VerticalAlignment.Center;

                //#region Animals

                //var exp = 0;
                //exp += field.UnitList.FirstOrDefault(x => x.Id == 1)?.Count ?? 0;
                //exp += field.UnitList.FirstOrDefault(x => x.Id == 2)?.Count ?? 0;
                //exp += field.UnitList.FirstOrDefault(x => x.Id == 3)?.Count ?? 0;
                //exp += field.UnitList.FirstOrDefault(x => x.Id == 4)?.Count ?? 0;
                //exp += field.UnitList.FirstOrDefault(x => x.Id == 5)?.Count * 2 ?? 0;
                //exp += field.UnitList.FirstOrDefault(x => x.Id == 6)?.Count * 2 ?? 0;
                //exp += field.UnitList.FirstOrDefault(x => x.Id == 7)?.Count * 3 ?? 0;
                //exp += field.UnitList.FirstOrDefault(x => x.Id == 8)?.Count * 3 ?? 0;
                //exp += field.UnitList.FirstOrDefault(x => x.Id == 9)?.Count * 3 ?? 0;
                //exp += field.UnitList.FirstOrDefault(x => x.Id == 10)?.Count * 5 ?? 0;

                ////var m   = field.UnitList.FirstOrDefault(x => x.Id == 7);
                ////var k   = field.UnitList.FirstOrDefault(x => x.Id == 8);
                ////var t   = field.UnitList.FirstOrDefault(x => x.Id == 9);
                ////var s   = field.UnitList.FirstOrDefault(x => x.Id == 10);

                ////if (m != null)
                ////{
                ////    var ggg = new Grid();
                ////    ggg.HorizontalAlignment = HorizontalAlignment.Left;
                ////    ggg.VerticalAlignment = VerticalAlignment.Top;
                ////    ggg.Width = 50;
                ////    ggg.Height = 50;
                ////    ggg.Margin = new Thickness(0, 0, 0, 0);
                ////    ggg.Background = new SolidColorBrush(Color.FromArgb(255, 150, 0, 0));
                ////    var ttb = new TextBlock();
                ////    ttb.TextAlignment = TextAlignment.Center;
                ////    ttb.HorizontalAlignment = HorizontalAlignment.Center;
                ////    ttb.VerticalAlignment = VerticalAlignment.Center;
                ////    ttb.FontSize = 15;
                ////    ttb.Text = m.Count.ToString();
                ////    ggg.Children.Add(ttb);
                ////    g.Children.Add(ggg);
                ////}
                ////if (k != null)
                ////{
                ////    var ggg = new Grid();
                ////    ggg.HorizontalAlignment = HorizontalAlignment.Left;
                ////    ggg.VerticalAlignment = VerticalAlignment.Top;
                ////    ggg.Width = 50;
                ////    ggg.Height = 50;
                ////    ggg.Margin = new Thickness(50, 0, 0, 0);
                ////    ggg.Background = new SolidColorBrush(Color.FromArgb(255, 150, 0, 0));
                ////    var ttb = new TextBlock();
                ////    ttb.TextAlignment = TextAlignment.Center;
                ////    ttb.HorizontalAlignment = HorizontalAlignment.Center;
                ////    ttb.VerticalAlignment = VerticalAlignment.Center;
                ////    ttb.FontSize = 15;
                ////    ttb.Text = k.Count.ToString();
                ////    ggg.Children.Add(ttb);
                ////    g.Children.Add(ggg);
                ////}
                ////if (t != null)
                ////{
                ////    var ggg = new Grid();
                ////    ggg.HorizontalAlignment = HorizontalAlignment.Left;
                ////    ggg.VerticalAlignment = VerticalAlignment.Top;
                ////    ggg.Width = 50;
                ////    ggg.Height = 50;
                ////    ggg.Margin = new Thickness(0, 50, 0, 0);
                ////    ggg.Background = new SolidColorBrush(Color.FromArgb(255, 150, 0, 0));
                ////    var ttb = new TextBlock();
                ////    ttb.TextAlignment = TextAlignment.Center;
                ////    ttb.HorizontalAlignment = HorizontalAlignment.Center;
                ////    ttb.VerticalAlignment = VerticalAlignment.Center;
                ////    ttb.FontSize = 15;
                ////    ttb.Text = t.Count.ToString();
                ////    ggg.Children.Add(ttb);
                ////    g.Children.Add(ggg);
                ////}
                ////if (s != null)
                ////{
                ////    var ggg = new Grid();
                ////    ggg.HorizontalAlignment = HorizontalAlignment.Left;
                ////    ggg.VerticalAlignment = VerticalAlignment.Top;
                ////    ggg.Width = 50;
                ////    ggg.Height = 50;
                ////    ggg.Margin = new Thickness(50, 50, 0, 0);
                ////    ggg.Background = new SolidColorBrush(Color.FromArgb(255, 150, 0, 0));
                ////    var ttb = new TextBlock();
                ////    ttb.TextAlignment = TextAlignment.Center;
                ////    ttb.HorizontalAlignment = HorizontalAlignment.Center;
                ////    ttb.VerticalAlignment = VerticalAlignment.Center;
                ////    ttb.FontSize = 15;
                ////    ttb.Text = s.Count.ToString();
                ////    ggg.Children.Add(ttb);
                ////    g.Children.Add(ggg);
                ////}
                //#endregion

                //tb.Text = $"({field.X}|{field.Y})";
                //tb.Text += $"\n{exp}";
                //tb.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));

                var str = "";
                if (field.OasisBonus.Wood == 25)
                    str += "Д";
                if (field.OasisBonus.Clay == 25)
                    str += "Г";
                if (field.OasisBonus.Iron == 25)
                    str += "Ж";
                if (field.OasisBonus.Crop == 25)
                    str += "З";
                if (field.OasisBonus.Crop == 50)
                    str += "ЗЗ";
                tb.Text += $"\n{str}";

                tb.FontSize = 20;
                //if (exp >= 500)
                //if (field.OasisBonus.Crop == 25)
                   // g.Background = new SolidColorBrush(Color.FromArgb(150, 150, 150, 0));
                //if (exp >= 1000)
                if (field.OasisBonus.Crop == 25)
                    g.Background = new SolidColorBrush(Color.FromArgb(255, 255, 150, 0));
                //if (exp >= 1500)
                //if (field.OasisBonus.Crop == 25)
                    //g.Background = new SolidColorBrush(Color.FromArgb(255, 250, 250, 250));
                if (field.OasisBonus.Crop == 50)
                    g.Background = new SolidColorBrush(Color.FromArgb(255, 0, 200, 0));
                g.Children.Add(tb);
            }
            else if (field.ResType == "11115")
            {
                g.Background = !field.HasVillage ? new SolidColorBrush(Color.FromArgb(255, 255, 0, 0)) : new SolidColorBrush(Color.FromArgb(50, 255, 0, 0));
                var tb = new TextBlock();
                tb.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                tb.TextAlignment = TextAlignment.Center;
                tb.HorizontalAlignment = HorizontalAlignment.Center;
                tb.VerticalAlignment = VerticalAlignment.Center;

                tb.Text = $"({field.X}|{field.Y})";
                tb.FontSize = 20;
                g.Children.Add(tb);
                g.MouseEnter += (sender, args) =>
                                                    {
                                                        b.Margin = new Thickness(g.Margin.Left - 300, g.Margin.Top - 300, 0, 0);
                                                        b.Visibility = Visibility.Visible;
                                                    };
                g.MouseLeave += (sender, args) =>
                                {
                                    b.Visibility = Visibility.Collapsed;
                                };
            }
            else if (field.ResType == "3339")
            {
                g.Background = !field.HasVillage ? new SolidColorBrush(Color.FromArgb(255, 255, 0, 255)) : new SolidColorBrush(Color.FromArgb(50, 255, 0, 255));
                var tb = new TextBlock();
                tb.TextAlignment = TextAlignment.Center;
                tb.HorizontalAlignment = HorizontalAlignment.Center;
                tb.VerticalAlignment = VerticalAlignment.Center;

                tb.Text = $"({field.X}|{field.Y})";
                tb.Foreground = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                tb.FontSize = 20;
                g.Children.Add(tb);
                g.MouseEnter += (sender, args) =>
                                {
                                    b.Margin = new Thickness(g.Margin.Left - 300, g.Margin.Top - 300, 0, 0);
                                    b.Visibility = Visibility.Visible;
                                };
                g.MouseLeave += (sender, args) =>
                                {
                                    b.Visibility = Visibility.Collapsed;
                                };
            }
            else
            {
                //g.Background = new SolidColorBrush(Color.FromArgb(99, 255, 255, 255));
            }





            //if (field.IsOasis)
            //{
            //    g.Background = new SolidColorBrush(Color.FromArgb(255, 200, 100, 0));
            //    var tb = new TextBlock();
            //    tb.TextAlignment = TextAlignment.Center;
            //    tb.HorizontalAlignment = HorizontalAlignment.Center;
            //    tb.HorizontalAlignment = HorizontalAlignment.Center;
            //    var str = "";
            //    if (field.OasisBonus.Wood == 25)
            //        str += "Д";
            //    if (field.OasisBonus.Clay == 25)
            //        str += "Г";
            //    if (field.OasisBonus.Iron == 25)
            //        str += "Ж";
            //    if (field.OasisBonus.Crop == 25)
            //        str += "З";
            //    if (field.OasisBonus.Crop == 50)
            //        str += "ЗЗ";
            //    tb.Text = str;
            //    tb.FontSize = 20;
            //    g.Children.Add(tb);
            //}

            return g;
        }
    }
}
