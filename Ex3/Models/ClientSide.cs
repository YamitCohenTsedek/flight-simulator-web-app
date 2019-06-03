using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Net.Sockets;
using System.Net;


namespace Ex3.Models
{
    // a struct that represents the info that we want to get from the simulator:
    // the lon, lat, throttle and rudder values
    public struct SimulatorInfo
    {
        public double Lon;
        public double Lat;
        public double Throttle;
        public double Rudder;

        public SimulatorInfo(double lonValue, double latValue, double throttleValue, double rudderValue)
        {
            Lon = lonValue;
            Lat = latValue;
            Throttle = throttleValue;
            Rudder = rudderValue;
        }
    }

    public class ClientSide
    {
        private IPEndPoint endPoint;
        private string ip;
        private int port;
        private int time;
        private StreamReader reader;

        private NetworkStream stream;
        private Socket webClient;
        private string lonGetCommand = "get /position/longitude-deg";
        private string latGetCommand = "get /position/latitude-deg";
        private string throttleGetCommand = "get /controls/engines/current-engine/throttle";
        private string rudderGetCommand = "get /controls/flight/rudder";

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
            webClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            while (webClient.Connected == false)
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
        }

        public void Disconnect()
        {
            if(IsConnectedToSimulator)
            {
                webClient.Close();
            }
            IsConnectedToSimulator = false;
        }

        public SimulatorInfo SendCommandsToSimulator()
        {
            Byte[] lonBuff = Encoding.ASCII.GetBytes(lonGetCommand + "\r\n");
            Byte[] latBuff = Encoding.ASCII.GetBytes(latGetCommand + "\r\n");
            Byte[] throttleBuff = Encoding.ASCII.GetBytes(throttleGetCommand + "\r\n");
            Byte[] rudderBuff = Encoding.ASCII.GetBytes(rudderGetCommand + "\r\n");
            double lon;
            double lat;
            double throttle;
            double rudder;
            stream = new NetworkStream(webClient);
            reader = new StreamReader(stream);
            stream.Write(lonBuff, 0, lonBuff.Length);
            lon = Double.Parse(reader.ReadLine().Split('=')[1].Split('\'')[1]);
            stream.Write(latBuff, 0, latBuff.Length);
            lat = Double.Parse(reader.ReadLine().Split('=')[1].Split('\'')[1]);
            stream.Write(throttleBuff, 0, throttleBuff.Length);
            throttle = Double.Parse(reader.ReadLine().Split('=')[1].Split('\'')[1]);
            stream.Write(rudderBuff, 0, rudderBuff.Length);
            rudder = Double.Parse(reader.ReadLine().Split('=')[1].Split('\'')[1]);
            SimulatorInfo info = new SimulatorInfo(lon, lat,
                throttle, rudder);
            return info;
        }
    }
}
