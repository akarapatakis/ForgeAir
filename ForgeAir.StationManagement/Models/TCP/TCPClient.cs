using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.StationManagement.Models.TCP
{
    public class TCPClient
    {

        public string ipAddress { get; set; }
        public int port { get; set; }

        private IPEndPoint endPoint { get; set; }
        private System.Net.Sockets.TcpClient client { get; set; }
        private System.Net.Sockets.TcpListener listener { get; set; }
        private NetworkStream stream { get; set; }

        public TCPClient(string ip, int inPort)
        {
            if (ipAddress == null)
            {
                throw new ArgumentNullException("IP Address cannot be null.");
            }
            else if (port == null)
            {
                throw new ArgumentNullException("Port cannot be null.");
            }


            this.ipAddress = ip;
            this.port = inPort;
            client = new System.Net.Sockets.TcpClient(this.ipAddress, this.port);
            
            client.Connect(ipAddress, port);
            stream = client.GetStream();


        }
    }

}
