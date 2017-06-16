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
        LogInWindow home_window;

        private int packet_size = 0;
        private byte[] packet_buff = new byte[Packet.PACKET_MAX_SIZE];
        private Packet request_packet = new Packet();

        public AudioPacketHandler()
        {

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
            
            // create new packet
            this.packet_size = Packet.CreateAudioPacket(this.packet_buff, -1, data, data_size);
            this.request_packet.SetPacket(this.packet_buff);

            // socket send
            if (home_window != null)
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
