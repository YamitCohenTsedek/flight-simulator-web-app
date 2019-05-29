using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ex3.Models;

namespace Ex3.Controllers
{
    public class DisplayController : Controller
    {
        private ClientSide server;

        public DisplayController()
        {

        }

        // GET: Display
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Display(String ip, int port)
        {
            ClientSide.Instance.Ip = ip;
            ClientSide.Instance.Port = port;
            ClientSide.Instance.Connect();
            SimulatorInfo info  = ClientSide.Instance.SendCommandsToSimulator();
            Session["lon"] = info.Lon;
            Session["lat"] = info.Lat;
            return View();

        }
    }
}