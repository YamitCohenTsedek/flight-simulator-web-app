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
        private InfoServer server;

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
            InfoServer.Instance.Ip = ip;
            InfoServer.Instance.Port = port.ToString();
            SimulatorInfo info  = InfoServer.Instance.GetInfoFromSimulator();
            Session["lon"] = info.Lon;
            Session["lat"] = info.Lat;
            return View();

        }
    }
}