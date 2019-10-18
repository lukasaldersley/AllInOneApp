using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using Windows.Storage;

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
            Socket client = new Socket(SocketType.Stream, ProtocolType.Tcp);
            client.Connect(host, port);
            client.Send(sp.Flags);
            client.Send(sp.Message.Length.GetByteQuartetFromInt());
            client.Send(sp.Message);

            byte[] RxFlags=new byte[4];
            byte[] RxLen = new byte[4];

            client.Receive(RxFlags);
            client.Receive(RxLen);

            int len = RxLen.GetIntFromByteQuartet();
            byte[] RxMsg = new byte[len];

            client.Receive(RxMsg);
            client.Dispose();

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

        public static String[] ListDirectory(String path)
        {
            return GeneralRequest(new ServerPacket(new byte[] { NUL, GENERAL, LIST_DIRECTORY, NUL }, path.ToByteArray())).Message.ToAsciiString().Split("|");
        }

        public async static Task<bool> UploadFile(StorageFile source, String remotePath)
        {
            ServerPacket SP1 = GeneralRequest(new ServerPacket(new byte[] { NUL, GENERAL, WRITE, NUL }, remotePath.ToByteArray()));
            if (SP1.Flags[1] != ACK)
            {
                return false;
            }
            ServerPacket SP2 = GeneralRequest(new ServerPacket(new byte[] { NUL, FOLLOW_UP, SP1.Flags[2], NUL }, await source.GetBytes()));
            return SP2.Flags[1]==ACK?true:false;
        }
    }
}
