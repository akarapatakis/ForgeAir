using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.TCP
{
    public class Client
    {
        public TcpClient client;
        string _ipaddress;
        int _port;
        public event EventHandler<string> OnDataReceived;
        public string receivedData;
        public bool keepAlive = true;

        public Client(string ip, int port) { 
            _ipaddress = ip;
            _port = port;

            client = new TcpClient(ip, port);

            OnDataReceived += Client_OnDataReceived;
        }

        private protected void raiseonDatareceived(string response)
        {
            OnDataReceived.Invoke(null, response);
        }
        private void Client_OnDataReceived(object? sender, string e)
        {
           receivedData = e;
        }

        public async Task ConenctAndListenForever()
        {
            try
            {
                await client.ConnectAsync(_ipaddress, _port);
                NetworkStream stream = client.GetStream();

                while (keepAlive)
                {
                    byte[] data = new byte[256];
                    int bytes = stream.Read(data, 0, data.Length);

                    if (bytes == 0) break; 

                    string response = Encoding.UTF8.GetString(data, 0, bytes); // Using UTF-8 to decode correctly greek characters (πουτάνεςς)
                    raiseonDatareceived(response);
                }

                client.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
        }
    }
}
