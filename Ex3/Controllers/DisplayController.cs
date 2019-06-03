using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ex3.Models;
using System.Text;
using System.Xml;
using System.Net;

namespace Ex3.Controllers
{
    public class DisplayController : Controller
    {
        
        private static List<string> fileLinesList = new List<string>();
        private static int numOfCurrentLine = 0;
        private static int numOfLinesInFile = 0;
        private static bool newSavingInFile = false;

        // GET: Display
        public ActionResult Index()
        {
            return View();
        }

        private void ConnectToSimulator(String ip, int port)
        {
            ViewBag.ip = ip;
            ViewBag.port = port;
            ClientSide.Instance.Ip = ip;
            ClientSide.Instance.Port = port;
            ClientSide.Instance.Connect();
            SimulatorInfo info = ClientSide.Instance.SendCommandsToSimulator();
            ViewBag.Lon = info.Lon;
            ViewBag.Lat = info.Lat;
            ViewBag.Throttle = info.Throttle;
            ViewBag.Rudder = info.Rudder;
        }

        [HttpGet]
        public ActionResult DisplayLocation(String ip, int port)
        {
            if (ClientSide.Instance.IsConnectedToSimulator)
            {
                ClientSide.Instance.Disconnect();
            }
            System.Net.IPAddress IP = null;
            if (!IPAddress.TryParse(ip, out IP))
            {
                return RedirectToAction("ReadAndAnimate", new { file = ip, time = port });
            }
            ConnectToSimulator(ip, port);
            return View();
        }

        [HttpGet]
        public ActionResult DisplayLine(String ip, int port, int time)
        {
            if (ClientSide.Instance.IsConnectedToSimulator)
            {
                ClientSide.Instance.Disconnect();
            }
            ConnectToSimulator(ip, port);
            Session["time"] = time;

            return View();
        }

        [HttpPost]
        public string GetValuesFromSimulatorAndDisplayLine()
        {
            SimulatorInfo info = ClientSide.Instance.SendCommandsToSimulator();
            double lon = info.Lon;
            double lat = info.Lat;
            double throttle = info.Throttle;
            double rudder = info.Rudder;
            return ToXml(lat, lon, throttle, rudder, 0);
        }

        [HttpGet]
        public ActionResult Save(String ip, int port, int time, int seconds, string file)
        {
            newSavingInFile = true;
            if (ClientSide.Instance.IsConnectedToSimulator)
            {
                ClientSide.Instance.Disconnect();
            }
            ConnectToSimulator(ip, port);
            Session["time"] = time;
            Session["seconds"] = seconds;
            Session["filename"] = file;
            return View();
        }

        private void WriteToFile(string dataToWriteInFile)
        {
            string fileName = (string)Session["fileName"];
            string filePath = AppDomain.CurrentDomain.BaseDirectory + @"\" + fileName + ".txt";
            if (!System.IO.File.Exists(filePath))
            {
                System.IO.File.WriteAllText(filePath, dataToWriteInFile);
            }
            else
            {
                if (newSavingInFile)
                {
                    using (var sw = new System.IO.StreamWriter(filePath, false))
                    {
                        sw.Write("");
                        newSavingInFile = false;
                    }
                }
                else
                {
                    using (var sw = new System.IO.StreamWriter(filePath, true))
                    {
                        sw.Write(dataToWriteInFile);
                    }
                }
            }
        }

        [HttpPost]
        public string GetValuesFromSimulatorAndSave()
        {
            SimulatorInfo info = ClientSide.Instance.SendCommandsToSimulator();
            double lon = info.Lon;
            double lat = info.Lat;
            double throttle = info.Throttle;
            double rudder = info.Rudder;
            string dataToWriteInFile = lon.ToString() + "," + lat.ToString() + ","
                + throttle.ToString() + "," + rudder.ToString() + "\r\n";
            WriteToFile(dataToWriteInFile);
            return ToXml(lat, lon, throttle, rudder, 0);
        }

        [HttpGet]
        public ActionResult ReadAndAnimate(String file, int time)
        {
            Session["time"] = time;
            Session["filename"] = file;
            ReadFromFile();
            return View();
        }

        private void ReadFromFile()
        {
            new List<string>();
            numOfCurrentLine = 0;
            numOfLinesInFile = 0;
            string fileName = (string)Session["fileName"];
            string filePath = AppDomain.CurrentDomain.BaseDirectory + @"\" + fileName + ".txt";
            using (var sr = new System.IO.StreamReader(filePath))
            {
                string line;
                // Read and display lines from the file until the end of 
                // the file is reached.
                while ((line = sr.ReadLine()) != null)
                {
                    fileLinesList.Add(line);
                    numOfLinesInFile++;
                }
            }
        }

        [HttpPost]
        public string GetValuesFromFile()
        {
            // we assume that the format of the lines in the file is:
            // lon,lat,throttlr,rudder
            if (numOfCurrentLine < numOfLinesInFile)
            {
                string line = fileLinesList[numOfCurrentLine];
                string[] splitLine = line.Split(',');
                double lon = Double.Parse(splitLine[0]);
                double lat = Double.Parse(splitLine[1]);
                double throttle = Double.Parse(splitLine[2]);
                double rudder = Double.Parse(splitLine[3]);
                numOfCurrentLine++;
                return ToXml(lat, lon, throttle, rudder, 0);
            }
            return ToXml(0, 0, 0, 0, 1);
        }

        private string ToXml(double lat, double lon, double throttle, double rudder, double isEnd)
        {
            //Initiate XML stuff
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings();
            XmlWriter writer = XmlWriter.Create(sb, settings);
            writer.WriteStartDocument();
            writer.WriteStartElement("Location");
            writer.WriteElementString("Lon", lon.ToString());
            writer.WriteElementString("Lat", lat.ToString());
            writer.WriteElementString("Throttle", throttle.ToString());
            writer.WriteElementString("Rudder", rudder.ToString());
            writer.WriteElementString("IsEnd", isEnd.ToString());
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Flush();
            return sb.ToString();
        }
    }
}