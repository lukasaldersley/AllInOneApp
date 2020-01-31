using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Web.Http.Filters;

namespace AllInOneApp
{
    class NetworkInterface
    {
        private static HttpBaseProtocolFilter filter;
        private static HttpClient httpClient;
        private static CancellationTokenSource cts;

        internal async static Task<String> DownloadFileFromGithub(String name)
        {
            Debug.WriteLine(DATA.GithubURI + name);
            String text = await Download(DATA.GithubURI + name);
            return text;
        }

        internal async static Task<String> DownloadFileFromWebsite(String name)
        {
            Debug.WriteLine(DATA.WebsiteURI + name);
            String text = await Download(DATA.WebsiteURI + name);
            return text;
        }

        internal async static Task<String> DownloadFileFromArchive(String name)
        {
            Debug.WriteLine(DATA.ArchiveURI + name);
            String text = await Download(DATA.ArchiveURI + name);
            return text;
        }

        internal async static Task<byte[]> DownloadGarfield(DateTimeOffset date)
        {
            Debug.WriteLine("Downloading Garfield: " + DATA.GarfieldURI + date.ToString("yyyy") + "/" + date.ToString("yyyy-MM-dd") + ".gif");
            return await new HttpClient().GetByteArrayAsync(DATA.GarfieldURI + date.ToString("yyyy") + "/" + date.ToString("yyyy-MM-dd") + ".gif");
        }

        internal async static Task<byte[]> DownloadXKCD(String id)
        {
            Debug.WriteLine("Downloading XKCD: https://" + id);
            return await new HttpClient().GetByteArrayAsync("https://" + id);
        }

        internal static async Task<String> Download(String src)
        {
            src = src.Replace("\r\n", "");
            src = src.Replace("\n", "");
            try
            {
                filter = new HttpBaseProtocolFilter();
                httpClient = new HttpClient();
                cts = new CancellationTokenSource();


                if (!Uri.TryCreate(src.Trim(), UriKind.Absolute, out Uri resourceUri))
                {
                    return null;
                }

                filter.CacheControl.ReadBehavior = HttpCacheReadBehavior.Default;
                filter.CacheControl.WriteBehavior = HttpCacheWriteBehavior.Default;

                HttpResponseMessage response = await httpClient.GetAsync(resourceUri);
                String output = await response.Content.ReadAsStringAsync();
                output = output.Replace("\n", "\r\n");
                output = output.Replace("\r\r", "\r");

                //System.Diagnostics.Debug.WriteLine("!" + output + "!");
                return output;
            }
            catch
            {
                return null;
            }
        }

        public static String TX_RX_TCP(String command, String ip, int port)
        {
            try
            {
                Socket s = new Socket(SocketType.Stream, ProtocolType.Tcp);
                s.Connect(ip, port);
                s.ReceiveTimeout = 10000;
                String res = TX_RX_TCP(command, s);
                s.Dispose();
                return res.Trim();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.InnerException);
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
                return "VERBINDUNGSAUFBAU NICHT MÖGLICH! BESTEHT EINE INTERNETVERBINDUNG?";
            }
        }

        public static String TX_RX_TCP(String command, Socket s)
        {
            try
            {
                s.Send(Encoding.ASCII.GetBytes(command + "\r\n"));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.InnerException);
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
                return "FEHLER BEIM SENDEN";
            }
            try
            {
                byte[] buffer = new byte[128];
                char[] chars = new char[s.Receive(buffer)];
                Encoding.UTF8.GetDecoder().GetChars(buffer, 0, chars.Length, chars, 0);
                return new String(chars).Trim();
            }
            catch (SocketException ex)
            {
                Debug.WriteLine(ex.InnerException);
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
                Debug.WriteLine(ex.SocketErrorCode);
                return "TIMEOUT BEIM LESEN DER ANTWORT/KEINE ANTWORT";
            }
        }
    }
}
