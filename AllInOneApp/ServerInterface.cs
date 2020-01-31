using System;
using System.Threading.Tasks;
using System.Net.Sockets;
using Windows.Storage;
using System.Diagnostics;

namespace AllInOneApp
{
    class ServerInterface
    {
        public class ServerPacket
        {
            public byte[] Flags { get; set; }
            public byte[] Message { get; set; }
            public ServerPacket(byte[] flags, byte[] message)
            {
                Flags = flags;
                Message = message;
            }
        }

        public static ServerPacket GeneralRequest(ServerPacket sp, String host = "lukasaldersley.de", int port = 1337)
        {
            Debug.WriteLine("Starting new General Request to {0} on port {1} with flags {2} {3} {4} {5} and msg {6}", host, port, sp.Flags[0], sp.Flags[1], sp.Flags[2], sp.Flags[3], sp.Message.ToUTF8String());
            Socket client = new Socket(SocketType.Stream, ProtocolType.Tcp);
            Debug.WriteLine("created socket");
            try
            {
                client.Connect(host, port);
            }
            catch(Exception e)
            {
                e.PrintStackTrace();
                Debug.WriteLine("ConnectionError");
                return new ServerPacket(new byte[] { NAK, NAK, NAK, NAK }, new byte[0]);
            }
            Debug.WriteLine("connected");
            client.Send(sp.Flags);
            Debug.WriteLine("sent flags");
            client.Send(sp.Message.Length.GetByteQuartetFromInt());
            Debug.WriteLine("sent len");
            client.Send(sp.Message);
            Debug.WriteLine("sent msg");

            byte[] RxFlags=new byte[4];
            byte[] RxLen = new byte[4];

            client.Receive(RxFlags);
            Debug.Write("rx flags:");
            Debug.Write(RxFlags[0].ToString("x"));
            Debug.Write(RxFlags[1].ToString("x"));
            Debug.Write(RxFlags[2].ToString("x"));
            Debug.WriteLine(RxFlags[3].ToString("x"));
            client.Receive(RxLen);
            Debug.WriteLine("rx len");

            int len = RxLen.GetIntFromByteQuartet();
            byte[] RxMsg = new byte[len];

            client.Receive(RxMsg);
            Debug.WriteLine("rx msg:"+RxMsg.ToUTF8String());
            client.Send(new byte[] { ACK });
            client.Dispose();
            Debug.WriteLine("disposedConn");

            return new ServerPacket(RxFlags, RxMsg);
        }

        public static byte GENERAL = 0x31;
        public static byte FOLLOW_UP = 0x32;
        public static byte NUL = 0x00;
        public static readonly byte NAK = 0x15;
        public static readonly byte ACK = 0x06;
        public static readonly byte APPEND = 0x61;
        public static readonly byte DELETE = 0x64;
        public static readonly byte LIST_DIRECTORY = 0x6c;
        public static readonly byte READ = 0x72;
        public static readonly byte WRITE = 0x77;
        //TODO implement folder Actions
        public static readonly byte DELETE_F = 0x66;
        public static readonly byte CREATE_F = 0x6d;

        public static String[] ListDirectory(String path, String host = "lukasaldersley.de", int port = 1337)
        {
            return GeneralRequest(new ServerPacket(new byte[] { NUL, GENERAL, LIST_DIRECTORY, NUL }, path.ToByteArray()), host, port).Message.ToUTF8String().Split("|");
        }

        public async static Task<bool> UploadFile(StorageFile source, String remotePath, String host = "lukasaldersley.de", int port = 1337)
        {
            ServerPacket SP1 = GeneralRequest(new ServerPacket(new byte[] { NUL, GENERAL, WRITE, NUL }, remotePath.ToByteArray()), host, port);
            if (SP1.Flags[1] != ACK)
            {
                return false;
            }
            ServerPacket SP2 = GeneralRequest(new ServerPacket(new byte[] { NUL, FOLLOW_UP, SP1.Flags[2], NUL }, await source.GetBytes()), host, port);
            return SP2.Flags[1] == ACK ? true : false;
        }

        public static bool WriteFile(String remotePath, String text,String host="lukasaldersley.de",int port=1337)
        {
            ServerPacket SP1 = GeneralRequest(new ServerPacket(new byte[] { NUL, GENERAL, WRITE, NUL }, remotePath.ToByteArray()),host,port);
            if (SP1.Flags[1] != ACK)
            {
                return false;
            }
            ServerPacket SP2 = GeneralRequest(new ServerPacket(new byte[] { NUL, FOLLOW_UP, SP1.Flags[2], NUL }, text.ToUTF8ByteArray()), host, port);
            return SP2.Flags[1] == ACK ? true : false;
        }

        public static ServerPacket ReadFile(String remotePath, String host = "lukasaldersley.de", int port = 1337)
        {
            return GeneralRequest(new ServerPacket(new byte[] { NUL, GENERAL, READ, NUL }, remotePath.ToByteArray()), host, port);
        }
    }
}
