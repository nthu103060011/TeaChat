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
    /// Interaction logic for LogInWindow.xaml
    /// </summary>
    public partial class LogInWindow : Window
    {
        MainWindow parentWindow;
        public LogInWindow(MainWindow mainWindow)
        {
            InitializeComponent();
            this.parentWindow = mainWindow;
        }

        private void buttonLogIn_Click(object sender, RoutedEventArgs e)
        {
            if (textBoxUsername.Text == "") MessageBox.Show("請輸入帳號");
            else
            {
                parentWindow.logIn(textBoxUsername.Text);
            }
        }
    }
}
