using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TeaChat
{
    public class Packet
    {
        byte[] packet;

        public enum Commands
        {
            ReportName,     // string username
            UpdateUserList, // List<string> onlineUsers
            ChatRequest,    // List<string> chatFriends
            LeaveChatroom,  // int chatroomNumber
            FriendLeaving,  // int chatroomNumber, string leavingFriend
            LogOut,

            AddStroke,      // int chatroomNumber, string drawingAttributesText, string stylusPointsText
            EraseAll,       // int chatroomNumber
            AddTextBox,     // int chatroomNumber, string text, string X, string Y
            TextMessage,    // int chatroomNumber, string fromWho, string text
            BackgroundImage,// int chatroomNumber, string filename, byte[] data
            File,           // int chatroomNumber, string filename, byte[] data
        }

        public Packet()
        {
            packet = new byte[8192];
        }

        public Packet(byte[] packet)
        {
            this.packet = packet;
        }

        public byte[] getPacket()
        {
            return packet;
        }

        public Commands getCommand()
        {
            return (Commands)packet[0];
        }

        public int getChatroomNumber()
        {
            return packet[1];
        }

        public void changeChatroomNumber(int chatroomNumber)
        {
            packet[1] = (byte)chatroomNumber;
        }

        public int getDataSize()
        {
            return BitConverter.ToInt32(packet, 2);
        }

        #region getPacketData
        public string getReportNameData()
        {
            int dataSize = getDataSize();
            byte[] data = new byte[dataSize];
            Array.Copy(packet, 6, data, 0, dataSize);
            return Encoding.UTF8.GetString(data);
        }

        public List<string> getUpdateUserListData()
        {
            int dataSize = getDataSize();
            byte[] data = new byte[dataSize];
            Array.Copy(packet, 6, data, 0, dataSize);
            string json = Encoding.UTF8.GetString(data);
            return JsonConvert.DeserializeObject<List<string>>(json);
        }

        public List<string> getChatRequestData()
        {
            int dataSize = getDataSize();
            byte[] data = new byte[dataSize];
            Array.Copy(packet, 6, data, 0, dataSize);
            string json = Encoding.UTF8.GetString(data);
            return JsonConvert.DeserializeObject<List<string>>(json);
        }

        public string getFriendLeavingData()
        {
            int dataSize = getDataSize();
            byte[] data = new byte[dataSize];
            Array.Copy(packet, 6, data, 0, dataSize);
            return Encoding.UTF8.GetString(data);
        }

        public string[] getAddStrokeData()
        {
            int dataSize = getDataSize();
            byte[] data = new byte[dataSize];
            Array.Copy(packet, 6, data, 0, dataSize);
            string json = Encoding.UTF8.GetString(data);
            return JsonConvert.DeserializeObject<string[]>(json);
        }

        public string[] getAddTextBoxData()
        {
            int dataSize = getDataSize();
            byte[] data = new byte[dataSize];
            Array.Copy(packet, 6, data, 0, dataSize);
            string json = Encoding.UTF8.GetString(data);
            return JsonConvert.DeserializeObject<string[]>(json);
        }

        public string[] getTextMessageData()
        {
            int dataSize = getDataSize();
            byte[] data = new byte[dataSize];
            Array.Copy(packet, 6, data, 0, dataSize);
            string json = Encoding.UTF8.GetString(data);
            return JsonConvert.DeserializeObject<string[]>(json);
        }

        public string getFilename()
        {
            byte[] filenameByte = new byte[64];
            Array.Copy(packet, 6, filenameByte, 0, 64);
            string filename = Encoding.UTF8.GetString(filenameByte);
            return filename.TrimEnd((char)0);
        }

        public byte[] getFileData()
        {
            int dataSize = Math.Min(8118, getDataSize());
            byte[] data = new byte[dataSize];
            Array.Copy(packet, 74, data, 0, dataSize);
            return data;
        }
        #endregion

        #region makePacket
        public void makePacketReportName(string username)
        {
            packet.Initialize();
            packet[0] = (byte)Commands.ReportName;
            packet[1] = byte.MaxValue;
            byte[] data = Encoding.UTF8.GetBytes(username);
            byte[] dataSize = BitConverter.GetBytes(data.Length);
            Array.Copy(dataSize, 0, packet, 2, 4);
            Array.Copy(data, 0, packet, 6, data.Length);
        }

        public void makePacketUpdateUserList(List<string> onlineUsers)
        {
            packet.Initialize();
            packet[0] = (byte)Commands.UpdateUserList;
            packet[1] = byte.MaxValue;
            string json = JsonConvert.SerializeObject(onlineUsers);
            byte[] data = Encoding.UTF8.GetBytes(json);
            byte[] dataSize = BitConverter.GetBytes(data.Length);
            Array.Copy(dataSize, 0, packet, 2, 4);
            Array.Copy(data, 0, packet, 6, data.Length);
        }

        public void makePacketChatRequest(List<string> chatFriends)
        {
            packet.Initialize();
            packet[0] = (byte)Commands.ChatRequest;
            packet[1] = byte.MaxValue;
            string json = JsonConvert.SerializeObject(chatFriends);
            byte[] data = Encoding.UTF8.GetBytes(json);
            byte[] dataSize = BitConverter.GetBytes(data.Length);
            Array.Copy(dataSize, 0, packet, 2, 4);
            Array.Copy(data, 0, packet, 6, data.Length);
        }

        public void makePacketLeaveChatroom(int chatroomNumber)
        {
            packet.Initialize();
            packet[0] = (byte)Commands.LeaveChatroom;
            packet[1] = (byte)chatroomNumber;
            byte[] dataSize = BitConverter.GetBytes(0);
            Array.Copy(dataSize, 0, packet, 2, 4);
        }

        public void makePacketFriendLeaving(int chatroomNumber, string leavingFriend)
        {
            packet.Initialize();
            packet[0] = (byte)Commands.FriendLeaving;
            packet[1] = (byte)chatroomNumber;
            byte[] data = Encoding.UTF8.GetBytes(leavingFriend);
            byte[] dataSize = BitConverter.GetBytes(data.Length);
            Array.Copy(dataSize, 0, packet, 2, 4);
            Array.Copy(data, 0, packet, 6, data.Length);
        }

        public void makePacketLogOut()
        {
            packet.Initialize();
            packet[0] = (byte)Commands.LogOut;
            packet[1] = byte.MaxValue;
            byte[] dataSize = BitConverter.GetBytes(0);
            Array.Copy(dataSize, 0, packet, 2, 4);
        }

        public void makePacketAddStroke(int chatroomNumber, string drawingAttributesText, string stylusPointsText)
        {
            packet.Initialize();
            packet[0] = (byte)Commands.AddStroke;
            packet[1] = (byte)chatroomNumber;
            string[] stringArray = new string[2] { drawingAttributesText, stylusPointsText };
            string json = JsonConvert.SerializeObject(stringArray);
            byte[] data = Encoding.UTF8.GetBytes(json);
            byte[] dataSize = BitConverter.GetBytes(data.Length);
            Array.Copy(dataSize, 0, packet, 2, 4);
            Array.Copy(data, 0, packet, 6, Math.Min(8186, data.Length)); // TODO: 分割封包
        }

        public void makePacketEraseAll(int chatroomNumber)
        {
            packet.Initialize();
            packet[0] = (byte)Commands.EraseAll;
            packet[1] = (byte)chatroomNumber;
            byte[] dataSize = BitConverter.GetBytes(0);
            Array.Copy(dataSize, 0, packet, 2, 4);
        }

        public void makePacketAddTextBox(int chatroomNumber, string text, string X, string Y)
        {
            packet.Initialize();
            packet[0] = (byte)Commands.AddTextBox;
            packet[1] = (byte)chatroomNumber;
            string[] stringArray = new string[3] { text, X, Y };
            string json = JsonConvert.SerializeObject(stringArray);
            byte[] data = Encoding.UTF8.GetBytes(json);
            byte[] dataSize = BitConverter.GetBytes(data.Length);
            Array.Copy(dataSize, 0, packet, 2, 4);
            Array.Copy(data, 0, packet, 6, data.Length);
        }

        public void makePacketTextMessage(int chatroomNumber, string fromWho, string text)
        {
            packet.Initialize();
            packet[0] = (byte)Commands.TextMessage;
            packet[1] = (byte)chatroomNumber;
            string[] stringArray = new string[2] { fromWho, text };
            string json = JsonConvert.SerializeObject(stringArray);
            byte[] data = Encoding.UTF8.GetBytes(json);
            byte[] dataSize = BitConverter.GetBytes(data.Length);
            Array.Copy(dataSize, 0, packet, 2, 4);
            Array.Copy(data, 0, packet, 6, data.Length);
        }

        public void makePacketBackgroundImage(int chatroomNumber, string filename, byte[] data)
        {
            packet.Initialize();
            packet[0] = (byte)Commands.BackgroundImage;
            packet[1] = (byte)chatroomNumber;
            byte[] dataSize = BitConverter.GetBytes(data.Length);
            Array.Copy(dataSize, 0, packet, 2, 4);
            byte[] filenameByte = Encoding.UTF8.GetBytes(filename);
            Array.Copy(filenameByte, 0, packet, 6, filenameByte.Length);
            Array.Copy(data, 0, packet, 74, Math.Min(8118, data.Length)); // TODO: 分割封包
        }

        public void makePacketFile(int chatroomNumber, string filename, byte[] data)
        {
            packet.Initialize();
            packet[0] = (byte)Commands.File;
            packet[1] = (byte)chatroomNumber;
            byte[] dataSize = BitConverter.GetBytes(data.Length);
            Array.Copy(dataSize, 0, packet, 2, 4);
            byte[] filenameByte = Encoding.UTF8.GetBytes(filename);
            Array.Copy(filenameByte, 0, packet, 6, filenameByte.Length);
            Array.Copy(data, 0, packet, 74, Math.Min(8118, data.Length)); // TODO: 分割封包
        }
        #endregion

    }
}
