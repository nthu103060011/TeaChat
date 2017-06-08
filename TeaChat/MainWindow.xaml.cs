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
using Newtonsoft.Json;
using System.IO;

namespace TeaChat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Window1 echoWindow;

        public MainWindow()
        {
            InitializeComponent();
            echoWindow = new Window1();
            echoWindow.Show();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            echoWindow.Close();
        }

        private void InkCanvas_StrokeCollected(object sender, InkCanvasStrokeCollectedEventArgs e)
        {
            //MessageBox.Show(e.Stroke.DrawingAttributes.Width.ToString() + "," + e.Stroke.DrawingAttributes.Height.ToString());
            string drawingAttributesText = JsonConvert.SerializeObject(e.Stroke.DrawingAttributes);
            string stylusPointsText = JsonConvert.SerializeObject(e.Stroke.StylusPoints);

            // 傳送 drawingAttributesText, stylusPointsText 兩段 string 給 server
            echoWindow.receiveStroke(drawingAttributesText, stylusPointsText);
            //
        }

        private void menuItemEraseAll_Click(object sender, RoutedEventArgs e)
        {
            inkCanvas.Strokes.Clear();

            // 傳送 Erase 命令給 server
            echoWindow.receiveErase();
            //
        }

        #region 畫筆顏色
        private void menuItemRed_Click(object sender, RoutedEventArgs e)
        {
            inkCanvas.DefaultDrawingAttributes.Color = Colors.Red;
            foreach (MenuItem m in menuItemPenColor.Items)
                if (m != sender) m.IsChecked = false;
        }

        private void menuItemGreen_Click(object sender, RoutedEventArgs e)
        {
            inkCanvas.DefaultDrawingAttributes.Color = Colors.Green;
            foreach (MenuItem m in menuItemPenColor.Items)
                if (m != sender) m.IsChecked = false;
        }

        private void menuItemBlue_Click(object sender, RoutedEventArgs e)
        {
            inkCanvas.DefaultDrawingAttributes.Color = Colors.Blue;
            foreach (MenuItem m in menuItemPenColor.Items)
                if (m != sender) m.IsChecked = false;
        }

        private void menuItemBlack_Click(object sender, RoutedEventArgs e)
        {
            inkCanvas.DefaultDrawingAttributes.Color = Colors.Black;
            foreach (MenuItem m in menuItemPenColor.Items)
                if (m != sender) m.IsChecked = false;
        }
        #endregion

        #region 畫筆粗細
        private void menuItem1_Click(object sender, RoutedEventArgs e)
        {
            inkCanvas.DefaultDrawingAttributes.Height = 1;
            inkCanvas.DefaultDrawingAttributes.Width = 1;
            foreach (MenuItem m in menuItemPenSize.Items)
                if (m != sender) m.IsChecked = false;
        }
        private void menuItem2_Click(object sender, RoutedEventArgs e)
        {
            inkCanvas.DefaultDrawingAttributes.Height = 2;
            inkCanvas.DefaultDrawingAttributes.Width = 2;
            foreach (MenuItem m in menuItemPenSize.Items)
                if (m != sender) m.IsChecked = false;
        }
        private void menuItem3_Click(object sender, RoutedEventArgs e)
        {
            inkCanvas.DefaultDrawingAttributes.Height = 3;
            inkCanvas.DefaultDrawingAttributes.Width = 3;
            foreach (MenuItem m in menuItemPenSize.Items)
                if (m != sender) m.IsChecked = false;
        }
        private void menuItem4_Click(object sender, RoutedEventArgs e)
        {
            inkCanvas.DefaultDrawingAttributes.Height = 4;
            inkCanvas.DefaultDrawingAttributes.Width = 4;
            foreach (MenuItem m in menuItemPenSize.Items)
                if (m != sender) m.IsChecked = false;
        }
        private void menuItem5_Click(object sender, RoutedEventArgs e)
        {
            inkCanvas.DefaultDrawingAttributes.Height = 5;
            inkCanvas.DefaultDrawingAttributes.Width = 5;
            foreach (MenuItem m in menuItemPenSize.Items)
                if (m != sender) m.IsChecked = false;
        }
        #endregion

        private void menuItemAddText_Click(object sender, RoutedEventArgs e)
        {
            TextBox txtBox = new TextBox();
            txtBox.Width = 100;
            txtBox.Height = 50;
            txtBox.Background = Brushes.Transparent;
            txtBox.BorderBrush = Brushes.Transparent;
            txtBox.AcceptsReturn = true;
            inkCanvas.Children.Add(txtBox);
            txtBox.Focus();
        }
    }
}
