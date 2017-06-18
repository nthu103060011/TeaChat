using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeaChat;

namespace TeaChat.Audio
{
    class AudioPacketHandler
    {
        private ChatWindow chat_room;

        // socket
        private LogInWindow home_window;

        private int packet_size = 0;
        private byte[] packet_buff = new byte[Packet.PACKET_MAX_SIZE];
        private Packet request_packet = new Packet();

        public AudioPacketHandler()
        {
            //this.packet_buff = new byte[Packet.PACKET_MAX_SIZE];
            //this.request_packet = new Packet();
        }

        public AudioPacketHandler(LogInWindow home_window, ChatWindow chat_room)
        {
            if (home_window == null || chat_room == null)
                throw new ArgumentNullException();

            this.SetChatRoom(chat_room);
            this.SetSocket(home_window);
        }

        public void SendAudioPacket(byte[] data, int data_size)
        {
            if (this.request_packet == null)
                this.request_packet = new Packet();

            // create new packet
            this.packet_size = Packet.CreateAudioPacket(this.packet_buff, 0, data, data_size);
            this.request_packet.SetPacket(this.packet_buff);

            // socket send
            if (this.home_window != null)
                this.home_window.sendToServer(this.chat_room, this.request_packet);
        }

        public void SendPartConferenceCallPacket()
        {
            if (this.request_packet == null)
                this.request_packet = new Packet();

            this.request_packet.MakePartConfCallPacket(0);

            if (this.home_window != null)
                this.home_window.sendToServer(this.chat_room, this.request_packet);
        }

        public void SetChatRoom(ChatWindow chat_room)
        {
            this.chat_room = chat_room;
        }

        public void SetSocket(LogInWindow home_window)
        {
            this.home_window = home_window;
        }
    }
}
