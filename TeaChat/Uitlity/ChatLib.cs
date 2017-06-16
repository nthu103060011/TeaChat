using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace TeaChat.Uitlity
{
    public class ChatSetting
    {
        public static String serverIp = "140.114.86.61";
        public static int port = 9877;
    }

    public delegate String StrHandler(String str);

    public class ChatSocket
    {
        public Socket socket;
        public NetworkStream stream;
        public StreamReader reader;
        public StreamWriter writer;
        public StrHandler inHandler;
        public EndPoint remoteEndPoint;
        public bool isDead = false;

        public ChatSocket(Socket s)
        {
            socket = s;
            stream = new NetworkStream(s);
            reader = new StreamReader(stream);
            writer = new StreamWriter(stream);
            remoteEndPoint = socket.RemoteEndPoint;
        }

        public String receive()
        {
            return reader.ReadLine();
        }

        public ChatSocket send(String line)
        {
            writer.WriteLine(line);
            writer.Flush();
            return this;
        }

        public static ChatSocket connect(String ip)
        {
            IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(ip), ChatSetting.port);

            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                socket.Connect(ipep);
            }
            catch
            {
                return null;
            }
            return new ChatSocket(socket);
        }

        public Thread newListener(StrHandler pHandler)
        {
            inHandler = pHandler;

            Thread listenThread = new Thread(new ThreadStart(listen));
            listenThread.Start();
            return listenThread;
        }

        public void listen()
        {
            try
            {
                while (true)
                {
                    String line = receive();
                    inHandler(line);
                }
            }
            catch (Exception ex)
            {
                isDead = true;
                Console.WriteLine(ex.Message);
            }
        }
    }
}
