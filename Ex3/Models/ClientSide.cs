using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Net.Sockets;
using System.Net;


namespace Ex3.Models
{
    // a struct that represents the info that we want to get from the simulator - the lon and the lat
    public struct SimulatorInfo
    {
        public double Lon;
        public double Lat;

        public SimulatorInfo(double lonValue, double latValue)
        {
            Lon = lonValue;
            Lat = latValue;
        }
    }

    public class ClientSide
    {
        private IPEndPoint endPoint;
        private string ip;
        private int port;
        private int time;
        private TcpClient webClient;
        private StreamWriter writer;
        private StreamReader reader;
        private string lonGetCommand = "get /position/longitude-deg\r\n";
        private string latGetCommand = "get /position/latitude-deg\r\n";

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

        public string Ip
        {
            get { return ip; }
            set { ip = value; }
        }

        public int Port
        {
            get { return port; }
            set { port = value; }
        }

        public int Time
        {
            get { return time; }
            set { time = value; }
        }

        public bool IsConnectedToSimulator { get; set; } = false;

        public void Connect()
        {
            endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            webClient = new TcpClient();

            while (!webClient.Connected)
            {
                try
                {
                    webClient.Connect(endPoint);
                }
                catch (Exception)
                {
                    continue;
                }
            }
            IsConnectedToSimulator = true;
            writer = new StreamWriter(webClient.GetStream());
            reader = new StreamReader(webClient.GetStream());
        }

        public SimulatorInfo SendCommandsToSimulator()
        {
            double lon;
            double lat;
            StreamWriter writer = new StreamWriter(webClient.GetStream());
            writer.WriteLine(lonGetCommand);
            writer.Flush();
            lon = Convert.ToDouble(reader.ReadLine());
            writer.WriteLine(latGetCommand);
            writer.Flush();
            lat = Convert.ToDouble(reader.ReadLine());
            SimulatorInfo info = new SimulatorInfo(lon, lat);
            return info;
        }

        // TODO: add toXML method

        public void Dispose()
        {
            webClient.Close();
        }
    }
}
