using System;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace AllInOneApp
{
    class ClintServerMethods
    {
        public async static Task<String> SendDataAndRecieveAnswer(String hostname, int port, String dataToSend)
        {
            ASCIIEncoding asciiEncoder = new ASCIIEncoding();
            return asciiEncoder.GetString(await SendDataAndRecieveAnwer(hostname, port, asciiEncoder.GetBytes(dataToSend)));

        }

        public async static Task<byte[]> SendDataAndRecieveAnwer(String hostname, int port, byte[] dataToSend)
        {
            if (dataToSend.Length > Math.Pow(2, 24))
            {
                throw new ArgumentOutOfRangeException();
            }

            TcpClient client = new TcpClient();
            //client.Connect(hostname, port);
            await client.ConnectAsync(hostname, port);
            Stream transmissionStream = client.GetStream();

            ///Before I send the actual data I want to send one byte reserved if I ever want to have a byte as signal-flag or something
            ///and then three bytes containing the lenght of the message, so the reciever knows how much data to expect (no buffer overruns).
            ///24 bit length field is probably WAY overkill, but 6 bit length field would only gibe me a 65kB maximum message length
            byte reservedForFutureUse = 0x00;
            transmissionStream.WriteByte(reservedForFutureUse);

            int len = dataToSend.Length;
            int tmp = len;
            tmp = tmp >> 16;
            //Console.WriteLine("---");
            //Console.WriteLine(len.ToString("x"));//print as hex
            //Console.WriteLine(tmp.ToString("x"));
            //Console.WriteLine("-");
            transmissionStream.WriteByte((byte)tmp);
            len = len - (tmp << 16);
            tmp = len;
            tmp = tmp >> 8;
            //Console.WriteLine(len.ToString("x"));//print as hex
            //Console.WriteLine(tmp.ToString("x"));
            //Console.WriteLine("-");
            transmissionStream.WriteByte((byte)tmp);
            len = len - (tmp << 8);
            tmp = len;
            //tmp = tmp >> 0;
            //Console.WriteLine(len.ToString("x"));//print as hex
            //Console.WriteLine(tmp.ToString("x"));
            //Console.WriteLine("---");
            transmissionStream.WriteByte((byte)tmp);
            ///Here is the end of my homebrew header of 4 bytes (1 Byte reserved + 3 Byte length)

            transmissionStream.Write(dataToSend, 0, dataToSend.Length);

            byte reservedByte = (byte)transmissionStream.ReadByte();
            int rxLen = transmissionStream.ReadByte();
            rxLen = rxLen << 8;
            rxLen += transmissionStream.ReadByte();
            rxLen = rxLen << 8;
            rxLen += transmissionStream.ReadByte();
            //Console.WriteLine("Excpecting answer of length " + rxLen.ToString() + " (0x" + rxLen.ToString("x") + ")");
            byte[] recievedData = new byte[rxLen];
            int bytesReallyRead = transmissionStream.Read(recievedData, 0, rxLen);

            //client.Close();
            client.Dispose();
            return recievedData;
        }
    }
}
