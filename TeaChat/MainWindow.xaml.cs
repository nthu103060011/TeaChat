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

namespace TeaChat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //int index = 0;

        Window1 echoWindow;

        public MainWindow()
        {
            InitializeComponent();
            echoWindow = new Window1();
            echoWindow.Show();
        }

        private void InkCanvas_StrokeCollected(object sender, InkCanvasStrokeCollectedEventArgs e)
        {
            //textBlock.Text += "StrokeCollected" + index + '\n';
            //index++;

            ((InkCanvas)((Grid)echoWindow.Content).Children[0]).Strokes.Add(e.Stroke);
            string serializedJson = JsonConvert.SerializeObject(e.Stroke, Formatting.Indented,
                     new JsonSerializerSettings()
                     {
                         TypeNameHandling = TypeNameHandling.Auto,
                         PreserveReferencesHandling = PreserveReferencesHandling.Objects
                     });
            textBlock.Text += serializedJson + '\n';
            //MessageBox.Show(e.Stroke.DrawingAttributes.IgnorePressure.ToString());
        }

        private void menuItemEraseAll_Click(object sender, RoutedEventArgs e)
        {
            inkCanvas.Strokes.Clear();
        }
    }
}
