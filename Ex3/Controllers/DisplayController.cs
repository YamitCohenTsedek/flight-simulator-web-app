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
        // list of the lines written in the data base file 
        private static List<string> fileLinesList = new List<string>();
        // indicates what is the current line that we are reading from the file
        private static int numOfCurrentLine = 0;
        // indicates if we have a new saving of the file
        private static bool newSavingInFile = false;


        // establish the connection to the flight simulator
        private void ConnectToSimulator(String ip, int port)
        {
            ClientSide.Instance.Ip = ip;
            ClientSide.Instance.Port = port;
            ClientSide.Instance.Connect();
            SimulatorInfo info = ClientSide.Instance.SampleFlightValues();
            // send the initial values that were sampled by the flight simulator to the view
            ViewBag.Lon = info.Lon;
            ViewBag.Lat = info.Lat;
            ViewBag.Throttle = info.Throttle;
            ViewBag.Rudder = info.Rudder;
        }

        // default display of the of the world's map
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        // display of the location of the flight simulator on the world's map
        [HttpGet]
        public ActionResult DisplayLocation(String ip, int port)
        {
            // disconnect the pervious connection to the flight simultor, if exists
            if (ClientSide.Instance.IsConnectedToSimulator)
            {
                ClientSide.Instance.Disconnect();
            }
            // check whether the first argument is indeed ip, and if not -
            // the action should be replaced to ReadAndAnimate action
            System.Net.IPAddress IP = null;
            if (!IPAddress.TryParse(ip, out IP))
            {
                return RedirectToAction("ReadAndAnimate", new { file = ip, time = port });
            }
            // connect to the flight simulator and return the required view
            ConnectToSimulator(ip, port);
            return View();
        }

        // display the route of the flight simulator - sample the values from the flight
        // simulator at a rate of once in the specified time
        [HttpGet]
        public ActionResult DisplayRoute(String ip, int port, int time)
        {
            // disconnect the pervious connection to the flight simultor, if exists
            if (ClientSide.Instance.IsConnectedToSimulator)
            {
                ClientSide.Instance.Disconnect();
            }
            // connect to the flight simulator
            ConnectToSimulator(ip, port);
            // send the time value to the view
            Session["time"] = time;
            return View();
        }

        // get the sampled values from the flight simulator and send them to the view
        [HttpPost]
        public string GetValuesFromSimulatorAndDisplayRoute()
        {
            SimulatorInfo info = ClientSide.Instance.SampleFlightValues();
            double lon = info.Lon;
            double lat = info.Lat;
            double throttle = info.Throttle;
            double rudder = info.Rudder;
            return ToXml(lat, lon, throttle, rudder, 0);
        }

        /*
         * display the route of the flight simulator and save the sampled values
         * in a data base file. Sample the values at a rate of once in the specified time,
         * and display the view for the specified number of seconds
         */
        [HttpGet]
        public ActionResult Save(String ip, int port, int time, int seconds, string file)
        {
            // indicates we have a new saving in the file, so it's previous
            // content shuold be deleted, if exists
            newSavingInFile = true;
            // disconnect the pervious connection to the flight simultor, if exists
            if (ClientSide.Instance.IsConnectedToSimulator)
            {
                ClientSide.Instance.Disconnect();
            }
            // connect to the flight simulator
            ConnectToSimulator(ip, port);
            // send the required values to the view
            Session["time"] = time;
            Session["seconds"] = seconds;
            Session["fileName"] = file;
            return View();
        }

        // write the values that are sampled by the flight simulator in a file 
        private void WriteToFile(string dataToWriteInFile)
        {
            string fileName = (string)Session["fileName"];
            // create the path of the file
            string filePath = AppDomain.CurrentDomain.BaseDirectory + @"\" + fileName + ".txt";
            // if the file doesn't exist, create it and write the data in it
            if (!System.IO.File.Exists(filePath))
            {
                System.IO.File.WriteAllText(filePath, dataToWriteInFile);
            }
            // else - the file exists
            {
                // if we have a new saving we should delete the previous content of the file
                if(newSavingInFile)
                {
                    using (var sw = new System.IO.StreamWriter(filePath, false))
                    {
                        sw.Write("");
                        newSavingInFile = false;
                    }
                }
                // write the data in the file
                using (var sw = new System.IO.StreamWriter(filePath, true))
                {
                    sw.Write(dataToWriteInFile);
                }
            }
        }

        // get the sampled values from the flight simulator, save them in a file
        // and send them to the view
        [HttpPost]
        public string GetValuesFromSimulatorAndSave()
        {
            SimulatorInfo info = ClientSide.Instance.SampleFlightValues();
            double lon = info.Lon;
            double lat = info.Lat;
            double throttle = info.Throttle;
            double rudder = info.Rudder;
            string dataToWriteInFile = lon.ToString() + "," + lat.ToString() + ","
                + throttle.ToString() + "," + rudder.ToString() + "\r\n";
            WriteToFile(dataToWriteInFile);
            return ToXml(lat, lon, throttle, rudder, 0);
        }

        /*
         * load the saved flight values from the data base file and display the route of the 
         * flight as an animation, at a rate of once in the specified time.
         * when the animation is finished, an alert pops up, informing that the scenario has ended.
         */
        [HttpGet]
        public ActionResult ReadAndAnimate(String file, int time)
        {
            // send the required values to the view
            Session["time"] = time;
            Session["fileName"] = file;
            // read the data from the file
            ReadFromFile();
            return View();
        }

        // read all the flight data ftom the file
        private void ReadFromFile()
        {
            // a list that includes the lines of the file
            fileLinesList =  new List<string>();
            numOfCurrentLine = 0;
            string fileName = (string)Session["fileName"];
            // create the path of the file
            string filePath = AppDomain.CurrentDomain.BaseDirectory + @"\" + fileName + ".txt";
            using (var sr = new System.IO.StreamReader(filePath))
            {
                string line;
                // Read lines from the file until the end of the file is reached.
                while ((line = sr.ReadLine()) != null)
                {
                    fileLinesList.Add(line);
                }
            }
        }

        // get the flight values from the file - one flight values sample every time
        // we assume that the format of the lines in the file is: lon,lat,throttle,rudder
        [HttpPost]
        public string GetValuesFromFile()
        {
            // if we still have not reached the last line in the file
            if(numOfCurrentLine < fileLinesList.Count)
            {
                // slit the liny by ',' and extract the required values
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

        // write XML data to a string
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