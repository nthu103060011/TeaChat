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
            // TODO: 告訴 server 我要登出，並關閉連線

            //

            foreach (ChatWindow chatWindow in chatWindows)
                chatWindow.Close();
        }

        #region 登入 登出 建立聊天
        private void buttonLogIn_Click(object sender, RoutedEventArgs e)
        {
            if (textBoxUsername.Text == "")
                MessageBox.Show("請輸入帳號");
            else if (logIn(textBoxUsername.Text) == false)
                MessageBox.Show("登入失敗\nServer沒開或帳號錯誤");
        }
        private bool logIn(string username)
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

        private void buttonLogOut_Click(object sender, RoutedEventArgs e)
        {
            // TODO: 告訴 server 我要登出，並關閉連線

            //

            foreach (ChatWindow chatWindow in chatWindows)
                chatWindow.Close();

            listBoxOnlineUsers.UnselectAll();
            userList.Clear();

            this.Title = "TeaChat - 登入";
            gridHome.Visibility = Visibility.Collapsed;
            stackPanelLogIn.Visibility = Visibility.Visible;
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
                ChatWindow newChatWindow = new ChatWindow(chatFriends, this);
                chatWindows.Add(newChatWindow);
                newChatWindow.Show();

                return true;
            }
            return false;
        }
        #endregion

        private void receiveFromServer()
        {
            // TODO: 接收訊息 更新上線者名單 or 聊天請求 or 聊天訊息
            string command = "updateUserList";
            int chatroomIndex = 1;
            byte[] data = new byte[8190];
            //
            switch (command)
            {
                case "updateUserList":
                    // TODO: 從 data 解析出 userListString
                    string userListString = "[\"friend1\", \"friend2\", \"friend3\"]";
                    //
                    userList = JsonConvert.DeserializeObject<List<string>>(userListString);
                    listBoxOnlineUsers.ItemsSource = userList;
                    break;
                case "chatRequest":
                    // TODO: 從 data 解析出 chatFriendsString
                    string chatFriendsString = "[\"friend1\", \"friend3\"]";
                    //
                    List<string> chatFriends = JsonConvert.DeserializeObject<List<string>>(chatFriendsString);
                    ChatWindow newChatWindow = new ChatWindow(chatFriends, this);
                    MessageBox.Show("有新的聊天請求");
                    chatWindows.Add(newChatWindow);
                    newChatWindow.Show();
                    break;
                case "friendLeaving":
                    // TODO: 從 data 解析出 leavingFriend
                    string leavingFriend = "takeashower";
                    //
                    chatWindows[chatroomIndex].receiveLeavingFriend(leavingFriend);
                    break;
                case "addStroke":
                    // TODO: 從 data 解析出 drawingAttributesText, stylusPointsText
                    string[] strokeString = new string[2];
                    strokeString[0] = File.ReadAllText("../../../da.txt");
                    strokeString[1] = File.ReadAllText("../../../sp.txt");
                    //
                    chatWindows[chatroomIndex].receiveStroke(strokeString);
                    break;
                case "eraseAll":
                    chatWindows[chatroomIndex].receiveErase();
                    break;
                case "addTextBox":
                    // TODO: 從 data 解析出 string text, string X, string Y
                    string[] textBoxString = { "important!!", "100.23", "58.2345" };
                    //
                    chatWindows[chatroomIndex].receiveTextBox(textBoxString);
                    break;
                case "textMessage":
                    // TODO: 從 data 解析出 string fromWho, string text
                    string[] textMessageString = { "friend2", "hello~" };
                    //
                    chatWindows[chatroomIndex].receiveTextMessage(textMessageString);
                    break;
                case "backgroundImage":
                    // TODO: 從 data 解析出 string filename, byte[] filedata
                    string filename = "image.png";
                    byte[] filedata = new byte[12345];
                    //
                    chatWindows[chatroomIndex].receiveBackgroundImage(filename, filedata);
                    break;
                case "file":
                    // TODO: 從 data 解析出 string filename, byte[] filedata
                    filename = "file.txt";
                    filedata = new byte[12345];
                    //
                    chatWindows[chatroomIndex].receiveFile(filename, filedata);
                    break;
                default:
                    MessageBox.Show("Server傳了未知指令");
                    break;
            }
        }

        public void sendToServer(ChatWindow fromChatroom, Packet packet)
        {
            int chatroomIndex = chatWindows.IndexOf(fromChatroom);
            packet.changeChatroomNumber(chatroomIndex);
            byte[] dataSand = packet.getPacket();

            // TODO: 傳送 dataSand 給 server

            //
        }
    }
}
