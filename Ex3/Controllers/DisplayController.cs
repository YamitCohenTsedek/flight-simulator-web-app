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
            SimulatorInfo info  = ClientSide.Instance.SendCommandsToSimulator();
            ViewBag.Lon = info.Lon;
            ViewBag.Lat = info.Lat;
            return View();
        }
    }
}