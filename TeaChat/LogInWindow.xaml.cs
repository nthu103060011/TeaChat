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

        //StrHandler msgHandler;

        private Packet req_packet;

        List<ChatWindow> chatWindows = new List<ChatWindow>();

        public List<string> userList;

        public string myName;

        public bool LoginIn = false;

        public LogInWindow()
        {
            InitializeComponent();
            gridHome.Visibility = Visibility.Collapsed;
            stackPanelLogIn.Visibility = Visibility.Visible;

            this.req_packet = new Packet();

            this.client = ChatSocket.connect(ChatSetting.serverIp);

            client.newListener(receiveFromServer);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            // 如果還沒登出，要登出
            if (LoginIn)
            {
                Packet packet = new Packet();
                packet.makePacketLogOut();
                sendToServer(null, packet);
                LoginIn = false;
            }
            
            try
            {
                client.close();
            }
            catch (Exception err)
            {
                Console.WriteLine(err.ToString());
            }

            foreach (ChatWindow chatWindow in chatWindows)
                chatWindow.Close();
        }

        private void buttonRegister_Click(object sender, RoutedEventArgs e)
        {
            if (this.textBoxUsername.Text.Length <= 0)
            {
                MessageBox.Show("請輸入帳號");
                return;
            }
            if (this.textBoxPassword.Password.Length <= 0)
            {
                MessageBox.Show("請輸入密碼");
                return;
            }
            if (this.client == null)
            {
                this.client = ChatSocket.connect(ChatSetting.serverIp);
                this.client.newListener(receiveFromServer);
            }

            if (this.req_packet == null)
                this.req_packet = new Packet();

            this.req_packet.MakePacketRequestUserRegister(this.textBoxUsername.Text, this.textBoxPassword.Password);
            this.sendToServer(null, this.req_packet);
        }

        #region 登入 登出 建立聊天

        private void stackPanelLogIn_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (textBoxUsername.Text == "")
                    MessageBox.Show("請輸入帳號");
                else if (logIn(textBoxUsername.Text) == false)
                    MessageBox.Show("Server沒開");
            }
        }

        private void buttonLogIn_Click(object sender, RoutedEventArgs e)
        {
            if (this.textBoxUsername.Text.Length <= 0)
            {
                MessageBox.Show("請輸入帳號");
                return;
            }
            if (this.textBoxPassword.Password.Length <= 0)
            {
                MessageBox.Show("請輸入密碼");
                return;
            }

            if (logIn(textBoxUsername.Text) == false)
                MessageBox.Show("Server沒開");
        }
      
        private bool logIn(string username)
        {
            myName = username;

            if (this.client == null)
            {
                this.client = ChatSocket.connect(ChatSetting.serverIp);
                this.client.newListener(receiveFromServer);
            }

            bool connectSuccess;
            //client = ChatSocket.connect(ChatSetting.serverIp);
            if (client == null) connectSuccess = false;
            else connectSuccess = true;

            if (connectSuccess)
            {
                LoginIn = true;
                Packet packet = new Packet();
                packet.makePacketReportName(username, this.textBoxPassword.Password);
                sendToServer(null, packet);
            }
            return connectSuccess;
        }

        private void buttonLogOut_Click(object sender, RoutedEventArgs e)
        {
            Packet packet = new Packet();
            packet.makePacketLogOut();
            sendToServer(null, packet);
            // 關閉連線
            LoginIn = false;
            client.close();
            client = null;
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
            else
                startChat(selectedUsers);
        }
        private void startChat(List<string> chatFriends)
        {
            ChatWindow newChatWindow = new ChatWindow(chatFriends, this);
            chatWindows.Add(newChatWindow);

            Packet packet = new Packet();
            packet.makePacketChatRequest(chatFriends);
            sendToServer(newChatWindow, packet);

            newChatWindow.Show();
        }
        #endregion

        private void receiveFromServer(byte[] dataReceive)
        {
            Packet packet = new Packet(dataReceive);
            Packet.Commands command = packet.getCommand();
            int chatroomIndex = packet.getChatroomIndex();

            // debug
            Console.Write("Recv command: "); Console.WriteLine(command);
            Console.Write("Recv Chat room num: ");Console.WriteLine(chatroomIndex);

            switch (command)
            {
                case Packet.Commands.UserRegisterAccept:
                    MessageBox.Show("註冊成功");
                    Dispatcher.BeginInvoke(new Action(delegate ()
                    {
                        buttonLogIn.Focus();
                    }));
                    break;
                case Packet.Commands.UserRegisterDeny:
                    MessageBox.Show("註冊失敗");
                    break;
                case Packet.Commands.UpdateUserList:
                    userList = new List<string>(packet.getUpdateUserListData());
                    Dispatcher.BeginInvoke(new Action(delegate ()
                    {
                        listBoxOnlineUsers.ItemsSource = userList;
                    }));
                    break;
                case Packet.Commands.AccountAuthorized:
                    Dispatcher.BeginInvoke(new Action(delegate ()
                    {
                        this.Title = "TeaChat - " + myName;
                        stackPanelLogIn.Visibility = Visibility.Collapsed;
                        gridHome.Visibility = Visibility.Visible;
                    }));
                    break;
                case Packet.Commands.AccountInvalid:
                    MessageBox.Show("帳號或密碼錯誤");
                    break;
                case Packet.Commands.ChatRequest:
                    List<string> chatFriends = packet.getChatRequestData();
                    ChatWindow newChatWindow;
                    Dispatcher.BeginInvoke(new Action(delegate ()
                    {
                        newChatWindow = new ChatWindow(chatFriends, this);
                        chatWindows.Add(newChatWindow);
                        Packet registerChatwindowPacket = new Packet();
                        registerChatwindowPacket.makePacketRegisterChatroom(0, chatroomIndex);
                        sendToServer(newChatWindow, registerChatwindowPacket);
                        newChatWindow.Show();
                    }));
                    break;
                case Packet.Commands.LeaveChatroom:
                    string leavingFriend = packet.getLeavingFriendData();
                    Dispatcher.BeginInvoke(new Action(delegate ()
                    {
                        chatWindows[chatroomIndex].receiveLeavingFriend(leavingFriend);
                    }));
                    break;
                case Packet.Commands.AddStroke:
                    string[] strokeString = packet.getAddStrokeData();
                    Dispatcher.BeginInvoke(new Action(delegate ()
                    {
                        chatWindows[chatroomIndex].receiveStroke(strokeString);
                    }));
                    break;
                case Packet.Commands.EraseAll:
                    Dispatcher.BeginInvoke(new Action(delegate ()
                    {
                        chatWindows[chatroomIndex].receiveErase();
                    }));
                    break;
                case Packet.Commands.AddTextBox:
                    string[] textBoxString = packet.getAddTextBoxData();
                    Dispatcher.BeginInvoke(new Action(delegate ()
                    {
                        chatWindows[chatroomIndex].receiveTextBox(textBoxString);
                    }));
                    break;
                case Packet.Commands.TextMessage:
                    string[] textMessageString = packet.getTextMessageData();
                    Dispatcher.BeginInvoke(new Action(delegate ()
                    {
                        chatWindows[chatroomIndex].receiveTextMessage(textMessageString);
                    }));
                    break;
                case Packet.Commands.BackgroundImage:
                    string imageFilename = packet.getFilename();
                    byte[] imageFiledata = packet.getFileData();
                    int imageSerialNumber = packet.getFileSerialNumber();
                    Console.WriteLine(imageSerialNumber);
                    Dispatcher.BeginInvoke(new Action(delegate ()
                    {
                        chatWindows[chatroomIndex].receiveBackgroundImage(imageFilename, imageSerialNumber, imageFiledata);
                    }));
                    break;
                case Packet.Commands.File:
                    string filename = packet.getFilename();
                    byte[] filedata = packet.getFileData();
                    int serialNumber = packet.getFileSerialNumber();
                    Console.WriteLine(serialNumber);
                    Dispatcher.BeginInvoke(new Action(delegate ()
                    {
                        chatWindows[chatroomIndex].receiveFile(filename, serialNumber, filedata);
                    }));
                    break;
                case Packet.Commands.OpenConferenceCall: // peer
                    Dispatcher.BeginInvoke(
                        new Action(
                            delegate ()
                            {
                                try
                                {
                                    this.chatWindows[chatroomIndex].SetupConferenceCallWindow();
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.ToString());
                                }
                            }
                        )
                    );
                    Dispatcher.BeginInvoke(
                            new Action(
                                delegate ()
                                {
                                    try
                                    {
                                        this.chatWindows[chatroomIndex].ConferenceCallConfirmation();
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine(e.ToString());
                                    }
                                }
                            )
                        );
                    break;
                case Packet.Commands.ConferenceCallOn:
                    Dispatcher.BeginInvoke(
                        new Action(
                            delegate ()
                            {
                                try
                                {
                                    this.chatWindows[chatroomIndex].ConferenceCallOn();
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.ToString());
                                }
                            }
                        )
                    );
                    //Console.WriteLine("Get conf call on packet");
                    break;
                case Packet.Commands.AudioData:
                    byte[] data = new byte[Audio.AudioHandler.AUDIO_DATA_MAX_SIZE];
                    int data_size = packet.GetPacketBody(data);
                    Dispatcher.BeginInvoke(
                        new Action(
                            delegate ()
                            {
                                try
                                {
                                    this.chatWindows[chatroomIndex].PlayAudioData(data, data_size);
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.ToString());
                                }
                            }
                        )
                    );
                    break;
                default:
                   // MessageBox.Show("Server傳了未知指令");
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

            // debug
            Console.Write("Send command: "); Console.WriteLine(packet.getCommand());
            Console.Write("Send Chat room num: "); Console.WriteLine(chatroomIndex);

            try
            {
                client.send(dataSand);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
