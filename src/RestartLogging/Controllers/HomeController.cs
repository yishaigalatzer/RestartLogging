using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNet.Mvc;

namespace RestartLogging.Controllers
{
    public class HomeController : Controller 
    { 
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            ViewBag.TimeFromStart = (DateTime.Now - Startup.StartUpTime).TotalMilliseconds;
            ViewBag.ControllerActionTime = DateTime.Now;
            ViewBag.ProcessId = Process.GetCurrentProcess().Id;
        }

        public IActionResult Index()
        { 
            return Content("TimeFrom start: " + ViewBag.TimeFromStart + " process id:" + ViewBag.ProcessId, "text/html");
        }

        public IActionResult OtherIndex()
        {
            return View("Index");
        }

        public IActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View("~/Views/Shared/Error.cshtml");
        }
    }
}