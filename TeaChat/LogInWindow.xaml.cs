using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
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
        List<ChatWindow> chatWindows = new List<ChatWindow>();

        public List<string> userList;

        public LogInWindow()
        {
            InitializeComponent();
            gridHome.Visibility = Visibility.Collapsed;
            stackPanelLogIn.Visibility = Visibility.Visible;
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            foreach (ChatWindow chatWindow in chatWindows)
                chatWindow.Close();
        }

        private void buttonLogIn_Click(object sender, RoutedEventArgs e)
        {
            if (textBoxUsername.Text == "")
                MessageBox.Show("請輸入帳號");
            else if (logIn(textBoxUsername.Text) == false)
                MessageBox.Show("登入失敗\nServer沒開或帳號錯誤");
        }

        public bool logIn(string username)
        {
            // TODO: 連線到server，如果沒連上，connectSuccess設為false
            bool connectSuccess = true;
            //

            if (connectSuccess)
            {
                // TODO: 告訴 server 我的 username

                //

                // TODO(可晚點做): 從 server 那邊得到回傳 登入成功或失敗
                bool logInSuccess = true;
                //

                if (logInSuccess)
                {
                    this.Title = "TeaChat - " + username;
                    stackPanelLogIn.Visibility = Visibility.Collapsed;
                    gridHome.Visibility = Visibility.Visible;

                    // TODO: 新增 thread 去跑 receiveFromServer() 接收 server的訊息

                    //

                    //假裝在這時收到server傳來使用者名單
                    receiveFromServer();
                    //

                    return true;
                }
            }
            return false;
        }

        private void receiveFromServer()
        {
            // TODO: 接收訊息 更新上線者名單 or 聊天請求
            string message = "updateUserList [\"friend1\", \"friend2\", \"friend3\"]";
            string command = "updateUserList";
            string userListString = "[\"friend1\", \"friend2\", \"friend3\"]";
            string chatFriendsString = "[\"friend1\", \"friend3\"]";
            //
            switch (command)
            {
                case "updateUserList":
                    userList = JsonConvert.DeserializeObject<List<string>>(userListString);
                    listBoxOnlineUsers.ItemsSource = userList;
                    break;
                case "chatRequest":
                    List<string> chatFriends = JsonConvert.DeserializeObject<List<string>>(chatFriendsString);
                    ChatWindow newChatWindow = new ChatWindow(chatFriends);
                    chatWindows.Add(newChatWindow);
                    newChatWindow.Show();
                    break;
            }
        }

        private void buttonStartChat_Click(object sender, RoutedEventArgs e)
        {
            List<string> selectedUsers = new List<string>();
            foreach (string selectedUser in listBoxOnlineUsers.SelectedItems)
                selectedUsers.Add(selectedUser);

            if (selectedUsers.Count <= 0)
                MessageBox.Show("請選擇聊天對象");
            else if (startChat(selectedUsers) == false)
                MessageBox.Show("建立聊天失敗");
        }

        private bool startChat(List<string> chatFriends)
        {
            // TODO: 告訴 server 我要跟 List<string> chatFriends 建立聊天
            bool startChatSuccess = true;
            //
            if (startChatSuccess)
            {
                ChatWindow newChatWindow = new ChatWindow(chatFriends);
                chatWindows.Add(newChatWindow);
                newChatWindow.Show();

                return true;
            }
            return false;
        }
    }
}
