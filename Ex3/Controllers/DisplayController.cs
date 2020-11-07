using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Ex3.Models;
using System.Text;
using System.Xml;
using System.Net;

namespace Ex3.Controllers
{
    public class DisplayController : Controller
    {
        // List of the lines written in the database file.
        private static List<string> fileLinesList = new List<string>();
        // numOfCurrentLine indicates what is the current line that we are reading from the file.
        private static int numOfCurrentLine = 0;
        // newSavingInFile indicates whether the user entred to the address /save/127.0.0.1/5400/4/10/flight1.
        private static bool newSavingInFile = false;


        // Establish the connection to the flight simulator.
        private void ConnectToSimulator(String ip, int port)
        {
            ClientSide.Instance.Ip = ip;
            ClientSide.Instance.Port = port;
            ClientSide.Instance.Connect();
            SimulatorInfo info = ClientSide.Instance.SampleFlightValues();
            // Send the initial values that were sampled by the flight simulator to the view.
            ViewBag.Lon = info.Lon;
            ViewBag.Lat = info.Lat;
            ViewBag.Throttle = info.Throttle;
            ViewBag.Rudder = info.Rudder;
        }

        // Default display of the of the world's map.
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        // Display of the current location of the aircraft from the flight simulator on the world's map.
        [HttpGet]
        public ActionResult DisplayLocation(String ip, int port)
        {
            // Disconnect the pervious connection to the flight simultor, if exists.
            if (ClientSide.Instance.IsConnectedToSimulator)
            {
                ClientSide.Instance.Disconnect();
            }
            // Check whether the first argument is an IP, and if not - replace the action to ReadAndAnimate action.
            System.Net.IPAddress IP = null;
            if (!IPAddress.TryParse(ip, out IP))
            {
                return RedirectToAction("ReadAndAnimate", new { file = ip, time = port });
            }
            // Connect to the flight simulator and return the required view.
            ConnectToSimulator(ip, port);
            return View();
        }

        /*
         * Display the route of the flight simulator -
         * sample the values from the flight simulator at a rate of once in the specified time.
         */
        [HttpGet]
        public ActionResult DisplayRoute(String ip, int port, int time)
        {
            // Disconnect the pervious connection to the flight simultor, if exists.
            if (ClientSide.Instance.IsConnectedToSimulator)
            {
                ClientSide.Instance.Disconnect();
            }
            // Connect to the flight simulator.
            ConnectToSimulator(ip, port);
            // Send the time value to the view.
            Session["time"] = time;
            return View();
        }

        // Get the sampled values from the flight simulator and send them to the view.
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
         * Display the route of the flight simulator and save the sampled values in a database file.
         * Sample the values at a rate of once in the specified time, during the specified number of seconds.
         */
        [HttpGet]
        public ActionResult Save(String ip, int port, int time, int seconds, string file)
        {
            /*
             * newSavingInFile indicates whether the user entred to the address /save/127.0.0.1/5400/4/10/flight1,
             * and if so - its previous content of the file should be deleted, if exists.
             */
            newSavingInFile = true;
            // Disconnect the pervious connection to the flight simultor, if exists.
            if (ClientSide.Instance.IsConnectedToSimulator)
            {
                ClientSide.Instance.Disconnect();
            }
            // Connect to the flight simulator.
            ConnectToSimulator(ip, port);
            // Send the required values to the view.
            Session["time"] = time;
            Session["seconds"] = seconds;
            Session["fileName"] = file;
            return View();
        }

        // Write the values that are sampled by the flight simulator in a file.
        private void WriteToFile(string dataToWriteInFile)
        {
            string fileName = (string)Session["fileName"];
            // Create the path of the file.
            string filePath = AppDomain.CurrentDomain.BaseDirectory + @"\" + fileName + ".txt";
            // If the file doesn't exist, create it and write the data in it.
            if (!System.IO.File.Exists(filePath))
            {
                System.IO.File.WriteAllText(filePath, dataToWriteInFile);
            }
            // Else - the file exists.
            {
                // If we have a new saving, we should delete the previous content of the file.
                if (newSavingInFile)
                {
                    using (var sw = new System.IO.StreamWriter(filePath, false))
                    {
                        sw.Write("");
                        newSavingInFile = false;
                    }
                }
                // Write the data in the file.
                using (var sw = new System.IO.StreamWriter(filePath, true))
                {
                    sw.Write(dataToWriteInFile);
                }
            }
        }

        // Get the sampled values from the flight simulator, save them in a file and send them to the view.
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
         * Load the saved flight values from the database file and display the route of the 
         * flight as an animation, at a rate of once in the specified time.
         * when the animation is finished, an alert pops up, informing that the scenario has ended.
         */
        [HttpGet]
        public ActionResult ReadAndAnimate(String file, int time)
        {
            // Send the required values to the view.
            Session["time"] = time;
            Session["fileName"] = file;
            // Read the data from the file.
            ReadFromFile();
            return View();
        }

        // Read all the flight data ftom the file.
        private void ReadFromFile()
        {
            // A list that contains the lines of the file.
            fileLinesList = new List<string>();
            numOfCurrentLine = 0;
            string fileName = (string)Session["fileName"];
            // Create the path of the file.
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

        /*
         * Get the flight values from the file - one flight values sample any time.
         * We assume that the format of the lines in the file is: lon,lat,throttle,rudder.
         */
        [HttpPost]
        public string GetValuesFromFile()
        {
            // As long as we have not reached the last line in the file:
            if (numOfCurrentLine < fileLinesList.Count)
            {
                // Split the line by ',' and extract the required values.
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

        // Write the XML data as a string.
        private string ToXml(double lat, double lon, double throttle, double rudder, double isEnd)
        {
            // Initiate XML stuff.
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