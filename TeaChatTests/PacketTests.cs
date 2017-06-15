using Microsoft.VisualStudio.TestTools.UnitTesting;
using TeaChat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TeaChat.Packet;
using System.IO;

namespace TeaChat.Tests
{
    [TestClass()]
    public class PacketTests
    {
        Packet packet = new Packet();

        [TestMethod()]
        public void PacketReportNameTest()
        {
            string username = "Lisa";
            packet.makePacketReportName(username);

            Commands command = packet.getCommand();
            string result = packet.getReportNameData();

            Assert.AreEqual(command, Commands.ReportName);
            Assert.AreEqual(username, result);
        }

        [TestMethod()]
        public void PacketUpdateUserListTest()
        {
            List<string> onlineUsers = new List<string>();
            onlineUsers.Add("Lisa");
            onlineUsers.Add("Simon");
            onlineUsers.Add("John");
            packet.makePacketUpdateUserList(onlineUsers);

            Commands command = packet.getCommand();
            List<string> result = packet.getUpdateUserListData();

            Assert.AreEqual(command, Commands.UpdateUserList);
            CollectionAssert.AreEqual(onlineUsers, result);
        }

        [TestMethod()]
        public void PacketChatRequestTest()
        {
            List<string> chatFriends = new List<string>();
            chatFriends.Add("Simon");
            chatFriends.Add("John");
            packet.makePacketChatRequest(chatFriends);

            Commands command = packet.getCommand();
            List<string> result = packet.getChatRequestData();

            Assert.AreEqual(command, Commands.ChatRequest);
            CollectionAssert.AreEqual(chatFriends, result);
        }

        [TestMethod()]
        public void PacketLeaveChatroomTest()
        {
            int chatroomNumber = 3;
            packet.makePacketLeaveChatroom(chatroomNumber);

            Commands command = packet.getCommand();
            int result = packet.getChatroomNumber();

            Assert.AreEqual(command, Commands.LeaveChatroom);
            Assert.AreEqual(chatroomNumber, result);
        }

        [TestMethod()]
        public void PacketFriendLeavingTest()
        {
            int chatroomNumber = 3;
            string leavingFriend = "Lisa";
            packet.makePacketFriendLeaving(chatroomNumber, leavingFriend);

            Commands command = packet.getCommand();
            int result = packet.getChatroomNumber();
            string result1 = packet.getFriendLeavingData();

            Assert.AreEqual(command, Commands.FriendLeaving);
            Assert.AreEqual(chatroomNumber, result);
            Assert.AreEqual(leavingFriend, result1);
        }

        [TestMethod()]
        public void PacketLogOutTest()
        {
            packet.makePacketLogOut();

            Commands command = packet.getCommand();

            Assert.AreEqual(command, Commands.LogOut);
        }

        [TestMethod()]
        public void PacketAddStrokeTest()
        {
            int chatroomNumber = 3;
            string drawingAttributesText = File.ReadAllText("../../../da.txt");
            string stylusPointsText = File.ReadAllText("../../../sp.txt");
            packet.makePacketAddStroke(chatroomNumber, drawingAttributesText, stylusPointsText);

            Commands command = packet.getCommand();
            int result = packet.getChatroomNumber();
            string[] strokeString = packet.getAddStrokeData();
            string drawingAttributesText1 = strokeString[0];
            string stylusPointsText1 = strokeString[1];

            Assert.AreEqual(command, Commands.AddStroke);
            Assert.AreEqual(chatroomNumber, result);
            Assert.AreEqual(drawingAttributesText, drawingAttributesText1);
            Assert.AreEqual(stylusPointsText, stylusPointsText1);
        }

        [TestMethod()]
        public void PacketEraseAllTest()
        {
            int chatroomNumber = 3;
            packet.makePacketEraseAll(chatroomNumber);

            Commands command = packet.getCommand();
            int result = packet.getChatroomNumber();

            Assert.AreEqual(command, Commands.EraseAll);
            Assert.AreEqual(chatroomNumber, result);
        }

        [TestMethod()]
        public void PacketAddTextBoxTest()
        {
            int chatroomNumber = 3;
            string text = "important!!";
            string X = "100.23";
            string Y = "58.2345";
            packet.makePacketAddTextBox(chatroomNumber, text, X, Y);

            Commands command = packet.getCommand();
            int result = packet.getChatroomNumber();
            string[] textBoxString = packet.getAddTextBoxData();
            string text1 = textBoxString[0];
            string X1 = textBoxString[1];
            string Y1 = textBoxString[2];

            Assert.AreEqual(command, Commands.AddTextBox);
            Assert.AreEqual(chatroomNumber, result);
            Assert.AreEqual(text, text1);
            Assert.AreEqual(X, X1);
            Assert.AreEqual(Y, Y1);
        }

        [TestMethod()]
        public void PacketTextMessageTest()
        {
            int chatroomNumber = 3;
            string fromWho = "Lisa";
            string text = "hello~";
            packet.makePacketTextMessage(chatroomNumber, fromWho, text);

            Commands command = packet.getCommand();
            int result = packet.getChatroomNumber();
            string[] textMessageString = packet.getTextMessageData();
            string fromWho1 = textMessageString[0];
            string text1 = textMessageString[1];

            Assert.AreEqual(command, Commands.TextMessage);
            Assert.AreEqual(chatroomNumber, result);
            Assert.AreEqual(fromWho, fromWho1);
            Assert.AreEqual(text, text1);
        }

        [TestMethod()]
        public void PacketBackgroundImageTest()
        {
            int chatroomNumber = 3;
            string filename = "test.png";
            byte[] data = File.ReadAllBytes("../../test.png");
            packet.makePacketBackgroundImage(chatroomNumber, filename, data);

            Commands command = packet.getCommand();
            int result = packet.getChatroomNumber();
            string filename1 = packet.getFilename();
            byte[] data1 = packet.getFileData();

            Assert.AreEqual(command, Commands.BackgroundImage);
            Assert.AreEqual(chatroomNumber, result);
            Assert.AreEqual(filename, filename1);
            CollectionAssert.AreEqual(data, data1);

            File.WriteAllBytes("../../test_result.png", data1);
        }

        [TestMethod()]
        public void PacketFileTest()
        {
            int chatroomNumber = 3;
            string filename = "test.png";
            byte[] data = File.ReadAllBytes("../../test.png");
            packet.makePacketFile(chatroomNumber, filename, data);

            Commands command = packet.getCommand();
            int result = packet.getChatroomNumber();
            string filename1 = packet.getFilename();
            byte[] data1 = packet.getFileData();

            Assert.AreEqual(command, Commands.File);
            Assert.AreEqual(chatroomNumber, result);
            Assert.AreEqual(filename, filename1);
            CollectionAssert.AreEqual(data, data1);

            File.WriteAllBytes("../../test_result.png", data1);
        }
    }
}