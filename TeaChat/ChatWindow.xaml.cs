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
using System.Net.Sockets;
using System.Threading;
using System.Windows.Ink;
using Microsoft.Win32;
using Ookii.Dialogs.Wpf;
using TeaChat.Audio;


namespace TeaChat
{
    /// <summary>
    /// Interaction logic for ChatWindow.xaml
    /// </summary>
    public partial class ChatWindow : Window
    {
        Window1 echoWindow = new Window1();
        LogInWindow homeWindow;

        TextBox textBoxInCanvas = new TextBox();
        bool creatingTextBox = false;
        bool editingText = false;
        
        private List<string> chatFriends;

        private ConferenceCallWindow conf_call_window = null;// = new ConferenceCallWindow();

        public ChatWindow(List<string> chatFriends, LogInWindow homeWindow)
        {
            InitializeComponent();
            this.Hide();
            textBoxInCanvas.Width = 100;
            textBoxInCanvas.Height = 50;
            textBoxInCanvas.Background = Brushes.Transparent;
            textBoxInCanvas.BorderBrush = Brushes.Transparent;
            textBoxInCanvas.AcceptsReturn = true;

            this.homeWindow = homeWindow;
            this.chatFriends = chatFriends;
            foreach (string friendName in chatFriends)
            {
                this.Title += " " + friendName;
                labelFriends.Header += " " + friendName;
            }

            //echoWindow.Show();
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            if (echoWindow != null) echoWindow.Close();

            // 告訴 server 離開聊天
            Packet packet = new Packet();
            packet.makePacketLogOut();
            homeWindow.sendToServer(this, packet);
            //
        }

        #region 畫圖及清除
        private void InkCanvas_StrokeCollected(object sender, InkCanvasStrokeCollectedEventArgs e)
        {
            int pointsCount = e.Stroke.StylusPoints.Count;
            int sentPointsCount = 0;
            while (pointsCount - sentPointsCount > 0)
            {
                StylusPointCollection stylusPointsOnePacket = new StylusPointCollection();
                for (int i = sentPointsCount; i < sentPointsCount + 80 && i < pointsCount; i++)
                {
                    stylusPointsOnePacket.Add(e.Stroke.StylusPoints[i]);
                }
                string drawingAttributesText = JsonConvert.SerializeObject(e.Stroke.DrawingAttributes);
                string stylusPointsText = JsonConvert.SerializeObject(stylusPointsOnePacket);
                File.WriteAllText(sentPointsCount + ".txt", stylusPointsText);

                // 傳送 drawingAttributesText, stylusPointsText 兩段 string 給 server
                Packet packet = new Packet();
                packet.makePacketAddStroke(0, drawingAttributesText, stylusPointsText);
                homeWindow.sendToServer(this, packet);
                echoWindow.receiveStroke(drawingAttributesText, stylusPointsText);
                //

                sentPointsCount += 80;
            }
        }

        private void menuItemEraseAll_Click(object sender, RoutedEventArgs e)
        {
            inkCanvas.Strokes.Clear();
            for (int i = inkCanvas.Children.Count - 1; i >= 0; i--)
            {
                if (inkCanvas.Children[i] is Label)
                    inkCanvas.Children.RemoveAt(i);
            }

            // 傳送 Erase 命令給 server
            Packet packet = new Packet();
            packet.makePacketEraseAll(0);
            echoWindow.receiveErase();
            //
        }
        #endregion

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

        #region 文字方塊
        private void menuItemAddText_Checked(object sender, RoutedEventArgs e)
        {
            creatingTextBox = true;
            inkCanvas.EditingMode = InkCanvasEditingMode.None;
        }

        private void menuItemAddText_Unchecked(object sender, RoutedEventArgs e)
        {
            creatingTextBox = false;
            inkCanvas.EditingMode = InkCanvasEditingMode.Ink;
        }

        private void inkCanvas_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (creatingTextBox)
            {
                e.Handled = true;
                inkCanvas.Children.Add(textBoxInCanvas);
                InkCanvas.SetLeft(textBoxInCanvas, e.GetPosition((IInputElement)sender).X);
                InkCanvas.SetTop(textBoxInCanvas, e.GetPosition((IInputElement)sender).Y);
                textBoxInCanvas.Focus();
                editingText = true;
                creatingTextBox = false;
            }
            else if (editingText && !textBoxInCanvas.IsMouseOver)
            {
                e.Handled = true;

                string text = textBoxInCanvas.Text;
                double X = InkCanvas.GetLeft(textBoxInCanvas);
                double Y = InkCanvas.GetTop(textBoxInCanvas);
                textBoxInCanvas.Text = "";
                inkCanvas.Children.Remove(textBoxInCanvas);

                Label label = new Label();
                label.Content = text;
                inkCanvas.Children.Add(label);
                InkCanvas.SetLeft(label, X);
                InkCanvas.SetTop(label, Y);

                menuItemAddText.IsChecked = false;
                editingText = false;

                // 傳送文字方塊 string text 和座標 double X, Y給 server
                Packet packet = new Packet();
                packet.makePacketAddTextBox(0, text, X.ToString(), Y.ToString());
                homeWindow.sendToServer(this, packet);
                echoWindow.receiveTextBox(text, X, Y);
                //
            }
        }
        #endregion

        #region 文字聊天
        private void buttonSendText_Click(object sender, RoutedEventArgs e)
        {
            string text = textBox.Text;
            textBox.Text = "";
            textBlock.Text += "我： " + text + "\n";

            // 傳送文字給 server
            Packet packet = new Packet();
            packet.makePacketTextMessage(0, homeWindow.myName, text);
            homeWindow.sendToServer(this, packet);
            echoWindow.receiveTextMessage(text);
            //
        }

        private void textBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string text = textBox.Text;
                textBox.Text = "";
                textBlock.Text += "我： " + text + "\n";

                // 傳送文字給 server
                Packet packet = new Packet();
                packet.makePacketTextMessage(0, homeWindow.myName, text);
                homeWindow.sendToServer(this, packet);
                echoWindow.receiveTextMessage(text);
                //
            }
        }
        #endregion

        #region 上傳背景or檔案
        private void menuItemBackgroundImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.bmp;*.jpg;*.jpeg;*.png";
            if (openFileDialog.ShowDialog() == true)
            {
                BitmapImage imageSource = new BitmapImage(new Uri(openFileDialog.FileName));
                Image image = new Image();
                image.Source = imageSource;
                if (imageSource.Width > inkCanvas.ActualWidth || imageSource.Height > inkCanvas.ActualHeight)
                {
                    image.Stretch = Stretch.Uniform;
                    image.MaxWidth = inkCanvas.ActualWidth;
                    image.MaxHeight = inkCanvas.ActualHeight;
                }
                inkCanvas.Children.Add(image);

                uploadFile(true, openFileDialog.FileName);
            }
        }

        private void menuItemUploadFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                uploadFile(false, openFileDialog.FileName);
            }
        }

        private void uploadFile(bool isBackgroundImage, string filePath)
        {
            byte[] data = File.ReadAllBytes(filePath);
            // TODO: 傳送檔案給 server

            //
        }
        #endregion

        #region 從server收到的命令 DONE
        public void receiveStroke(string[] strokeString)
        {
            string drawingAttributesText = strokeString[0];
            string stylusPointsText = strokeString[1];
            DrawingAttributes drawingAttributes = JsonConvert.DeserializeObject<DrawingAttributes>(drawingAttributesText);
            StylusPointCollection strokeCollection = JsonConvert.DeserializeObject<StylusPointCollection>(stylusPointsText);
            inkCanvas.Strokes.Add(new Stroke(strokeCollection, drawingAttributes));
        }

        public void receiveErase()
        {
            inkCanvas.Strokes.Clear();
            for (int i = inkCanvas.Children.Count - 1; i >= 0; i--)
            {
                if (inkCanvas.Children[i] is Label)
                    inkCanvas.Children.RemoveAt(i);
            }
        }

        public void receiveTextBox(string[] textBoxString)
        {
            string text = textBoxString[0];
            double X = Convert.ToDouble(textBoxString[1]);
            double Y = Convert.ToDouble(textBoxString[2]);
            Label label = new Label();
            label.Content = text;
            inkCanvas.Children.Add(label);
            InkCanvas.SetLeft(label, X);
            InkCanvas.SetTop(label, Y);
        }

        public void receiveTextMessage(string[] textMessageString)
        {
            string fromWho = textMessageString[0];
            string text = textMessageString[1];
            textBlock.Text += fromWho + ": " + text + "\n";
        }

        public void receiveBackgroundImage(string filename, byte[] data)
        {
            File.WriteAllBytes("Background Images\\" + filename, data);

            BitmapImage imageSource = new BitmapImage(new Uri("Background Images\\" + filename));
            Image image = new Image();
            image.Source = imageSource;
            if (imageSource.Width > inkCanvas.ActualWidth || imageSource.Height > inkCanvas.ActualHeight)
            {
                image.Stretch = Stretch.Uniform;
                image.MaxWidth = inkCanvas.ActualWidth;
                image.MaxHeight = inkCanvas.ActualHeight;
            }
            inkCanvas.Children.Add(image);
        }

        public void receiveFile(string filename, byte[] data)
        {
            if (MessageBox.Show("是否要儲存其他人上傳的檔案: " + filename, "確認訊息", MessageBoxButton.YesNo)
                == MessageBoxResult.Yes)
            {
                VistaFolderBrowserDialog folderBrowserDialog = new VistaFolderBrowserDialog();
                if (folderBrowserDialog.ShowDialog() == true)
                {
                    string filepath = folderBrowserDialog.SelectedPath + "\\";
                    File.WriteAllBytes(filepath + filename, data);
                }
            }
        }

        public void receiveLeavingFriend(string leavingFriend)
        {
            textBlock.Text += "*** " + leavingFriend + "離開聊天室\n";
        }
        #endregion

        #region conference call
        public void SetupConferenceCallWindow()
        {
            if (this.conf_call_window == null)
            {
                this.conf_call_window = new ConferenceCallWindow();
                this.conf_call_window.SetSocketandChatRoom(this.homeWindow, this);
                this.conf_call_window.Show();
            }
            else
            {
                if (!this.conf_call_window.IsLoaded)
                {
                    this.conf_call_window = new ConferenceCallWindow();
                    this.conf_call_window.SetSocketandChatRoom(this.homeWindow, this);
                    this.conf_call_window.Show();
                }
            }
        }

        public void ConferenceCallOn()
        {
            if (this.conf_call_window == null) return;
            if (!this.conf_call_window.IsLoaded) return;

            this.conf_call_window.SetConferenceCallStateOnChat();
        }

        public void PlayAudioData(byte[] data, int data_size)
        {
            if(this.conf_call_window == null) return;
            if (!this.conf_call_window.IsLoaded) return;

            this.conf_call_window.PlayAudioData(data, data_size);
        }

        private void conferenceCallButton_Click(object sender, RoutedEventArgs e)
        {
            this.SetupConferenceCallWindow();

            Packet packet = new Packet();
            packet.MakeOpenConfCallPakcet(0);
            this.homeWindow.sendToServer(this, packet);
        }
        #endregion
    }
}
