using Microsoft.VisualStudio.TestTools.UnitTesting;
using TeaChat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TeaChat.Packet;

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
            string username = "Lisa";
            packet.makePacketReportName(username);

            string result = packet.getReportNameData();

            Assert.AreEqual(username, result);
        }
    }
}