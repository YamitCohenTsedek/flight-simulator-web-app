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
    // a struct that represents the flight values that we want to get
    // from the flight simulator: the lon, lat, throttle and rudder values
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

    // the client side - the web client that connects to the flight simulator (which is the server)
    public class ClientSide
    {
        private IPEndPoint endPoint;
        private StreamReader reader;
        private NetworkStream stream;
        private Socket webClient;
        
        // get commands to the flight simulator
        private string lonGetCommand = "get /position/longitude-deg";
        private string latGetCommand = "get /position/latitude-deg";
        private string throttleGetCommand = "get /controls/engines/current-engine/throttle";
        private string rudderGetCommand = "get /controls/flight/rudder";

        public string Ip { get; set; }

        public int Port { get; set; }

        public int Time { get; set; }

        // indicates whether the web client is connected to the flight simulator
        public bool IsConnectedToSimulator { get; set; } = false;

        // singleton design pattern
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

        // connect to the flight simulator
        public void Connect()
        {
            // create the end point by the given ip and port
            endPoint = new IPEndPoint(IPAddress.Parse(Ip), Port);
            // create the socket of the web client
            webClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            // try to connect to the flight simulator as long as the web client is not connected 
            while (!(webClient.Connected))
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
            // the web client is connected to the simulator
            IsConnectedToSimulator = true;
        }

        // disconnect from the flight simulator
        public void Disconnect()
        {
            if(IsConnectedToSimulator)
            {
                webClient.Close();
            }
            IsConnectedToSimulator = false;
        }

        // sample flight values from the flight simulator
        public SimulatorInfo SampleFlightValues()
        {
            // the get commands represented as a bytes array
            Byte[] lonBuff = Encoding.ASCII.GetBytes(lonGetCommand + "\r\n");
            Byte[] latBuff = Encoding.ASCII.GetBytes(latGetCommand + "\r\n");
            Byte[] throttleBuff = Encoding.ASCII.GetBytes(throttleGetCommand + "\r\n");
            Byte[] rudderBuff = Encoding.ASCII.GetBytes(rudderGetCommand + "\r\n");
            double lon;
            double lat;
            double throttle;
            double rudder;
            // create a new stream and wrap it with a StreamReader
            stream = new NetworkStream(webClient);
            reader = new StreamReader(stream);
            // write get commands to the flight simulator, read it's response, 
            // and extract the flight values from it
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
