using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Net.Sockets;


namespace Ex3.Models
{
    public class ClientSide
    {
        TcpClient client;

        private static ClientSide instance = null;
        public static ClientSide Instance
        {
            get
            {
                if (instance == null)
                    instance = new ClientSide();
                return instance;
            }
        }

        /*
        public void Start()
        {
            int simulatorPort = Instance.FlightCommandPort;
            string simulatorIp = Instance.FlightServerIP;
            client = new TcpClient(simulatorIp, simulatorPort);
        }
       */

        public void Send(string command)
        {
            if (client != null)
            {
                StreamWriter writer = new StreamWriter(client.GetStream());
                writer.WriteLine(command);
                writer.Flush();
            }

        }

        public void Dispose()
        {
            client.Close();
        }
    }
}
