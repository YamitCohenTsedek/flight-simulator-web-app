using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ex3.Models;
using System.Timers;

namespace Ex3.Controllers
{
    public class DisplayController : Controller
    {
        private Timer timer;

        // GET: Display
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Display(String ip, int port)
        {

            ViewBag.ip = ip;
            ViewBag.port = port;
            ClientSide.Instance.Ip = ip;
            ClientSide.Instance.Port = port;
            ClientSide.Instance.Connect();
            SimulatorInfo info = ClientSide.Instance.SendCommandsToSimulator();
            ViewBag.lon = Convert.ToDouble(info.Lon);
            ViewBag.lat = Convert.ToDouble(info.Lat);
            return View();
        }

        [HttpGet]
        public ActionResult Display(String ip, int port, int time)
        {
            ViewBag.ip = ip;
            ViewBag.port = port;
            ClientSide.Instance.Ip = ip;
            ClientSide.Instance.Port = port;
            ClientSide.Instance.Time = time;
            ClientSide.Instance.Connect();
            SimulatorInfo info = ClientSide.Instance.SendCommandsToSimulator();
            ViewBag.lon = Convert.ToDouble(info.Lon);
            ViewBag.lat = Convert.ToDouble(info.Lat);
            timer = new Timer(1000 / time);
            timer.Elapsed += ThresholdReachedEvent;
            timer.AutoReset = true;
            timer.Enabled = true;

            Session["time"] = time;
            return View();
        }

        private void ThresholdReachedEvent(object source, System.Timers.ElapsedEventArgs e)
        {
            SimulatorInfo info = ClientSide.Instance.SendCommandsToSimulator();

        }
    }
}