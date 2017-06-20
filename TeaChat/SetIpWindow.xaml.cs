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
using System.Windows.Shapes;

namespace TeaChat
{
    /// <summary>
    /// Interaction logic for SetIpWindow.xaml
    /// </summary>
    public partial class SetIpWindow : Window
    {
        public SetIpWindow()
        {
            InitializeComponent();
        }

        private void buttonSetIp_Click(object sender, RoutedEventArgs e)
        {
            (new LogInWindow(textBoxSetIp.Text)).Show();
            Close();
        }
    }
}
