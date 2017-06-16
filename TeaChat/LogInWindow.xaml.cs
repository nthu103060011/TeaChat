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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using TeaChat.Uitlity;

namespace TeaChat
{
    /// <summary>
    /// Interaction logic for LogInWindow.xaml
    /// </summary>
    public partial class LogInWindow : Window
    {
        ChatSocket client;

        StrHandler msgHandler;

        List<ChatWindow> chatWindows = new List<ChatWindow>();

        public List<string> userList;

        public string myName;

        public LogInWindow()
        {
            InitializeComponent();
            gridHome.Visibility = Visibility.Collapsed;
            stackPanelLogIn.Visibility = Visibility.Visible;
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            // TODO: 如果還沒登出，要登出

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
            myName = username;

            // TODO: 連線到server，如果沒連上，connectSuccess設為false
            bool connectSuccess;
            //client = ChatSocket.connect(ChatSetting.serverIp);
            if (client == null)
                connectSuccess = false;
            else
                connectSuccess = true;
            //
            connectSuccess = true;
            if (connectSuccess)
            {
                Packet packet = new Packet();
                packet.makePacketReportName(username);
                sendToServer(null, packet);

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

                    userList = new List<string>();
                    userList.Add("friend1");
                    userList.Add("friend2");
                    listBoxOnlineUsers.ItemsSource = userList;
                    //

                    return true;
                }
            }
            return false;
        }

        private void buttonLogOut_Click(object sender, RoutedEventArgs e)
        {
            Packet packet = new Packet();
            packet.makePacketLogOut();
            sendToServer(null, packet);
            // TODO: 關閉連線

            //

            foreach (ChatWindow chatWindow in chatWindows)
                chatWindow.Close();

            listBoxOnlineUsers.UnselectAll();
            if (userList != null) userList.Clear();

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
            Packet packet = new Packet();
            packet.makePacketChatRequest(chatFriends);
            sendToServer(null, packet);

            // TODO: 確認建立聊天是否成功
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
            // TODO: 接收dataReceive
            byte[] dataReceive = new byte[8192];
            //
            Packet packet = new Packet(dataReceive);
            Packet.Commands command = packet.getCommand();
            int chatroomIndex = packet.getChatroomIndex();
            
            switch (command)
            {
                case Packet.Commands.UpdateUserList:
                    userList = packet.getUpdateUserListData();
                    listBoxOnlineUsers.ItemsSource = userList;
                    break;
                case Packet.Commands.ChatRequest:
                    List<string> chatFriends = packet.getChatRequestData();
                    ChatWindow newChatWindow = new ChatWindow(chatFriends, this);
                    MessageBox.Show("有新的聊天請求");
                    chatWindows.Add(newChatWindow);
                    newChatWindow.Show();
                    break;
                case Packet.Commands.LeaveChatroom:
                    string leavingFriend = packet.getFriendLeavingData();
                    chatWindows[chatroomIndex].receiveLeavingFriend(leavingFriend);
                    break;
                case Packet.Commands.AddStroke:
                    string[] strokeString = packet.getAddStrokeData();
                    chatWindows[chatroomIndex].receiveStroke(strokeString);
                    break;
                case Packet.Commands.EraseAll:
                    chatWindows[chatroomIndex].receiveErase();
                    break;
                case Packet.Commands.AddTextBox:
                    string[] textBoxString = packet.getAddTextBoxData();
                    chatWindows[chatroomIndex].receiveTextBox(textBoxString);
                    break;
                case Packet.Commands.TextMessage:
                    string[] textMessageString = packet.getTextMessageData();
                    chatWindows[chatroomIndex].receiveTextMessage(textMessageString);
                    break;
                case Packet.Commands.BackgroundImage:
                    string imageFilename = packet.getFilename();
                    byte[] imageFiledata = packet.getFileData();
                    chatWindows[chatroomIndex].receiveBackgroundImage(imageFilename, imageFiledata);
                    break;
                case Packet.Commands.File:
                    string filename = packet.getFilename();
                    byte[] filedata = packet.getFileData();
                    chatWindows[chatroomIndex].receiveFile(filename, filedata);
                    break;
                case Packet.Commands.OpenConferneceCall:
                    this.chatWindows[chatroomIndex].SetupConferenceCallWindow();
                    Packet rp_packet = new Packet();
                    rp_packet.MakePratConfCallPacket(chatroomIndex);
                    this.sendToServer(this.chatWindows[chatroomIndex], rp_packet);
                    break;
                case Packet.Commands.ConferenceCallOn:
                    this.chatWindows[chatroomIndex].ConferenceCallOn();
                    break;
                default:
                    MessageBox.Show("Server傳了未知指令");
                    break;
            }
        }

        public void sendToServer(ChatWindow fromChatroom, Packet packet)
        {
            int chatroomIndex;
            if (fromChatroom != null)
                chatroomIndex = chatWindows.IndexOf(fromChatroom);
            else
                chatroomIndex = -1;
            packet.changeChatroomIndex(chatroomIndex);
            byte[] dataSand = packet.getPacket();

            // TODO: 傳送 dataSand 給 server
            //client.send(  " : " + dataSand);
            //
        }
    }
}
