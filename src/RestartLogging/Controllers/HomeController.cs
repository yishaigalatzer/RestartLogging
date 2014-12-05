using System;
using System.Diagnostics;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.Logging;

namespace RestartLogging.Controllers
{ 
    public class HomeController : Controller
    {
        private IDisposable scope;
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            ViewBag.TimeFromStart = (DateTime.Now - Startup.StartUpTime).TotalMilliseconds;
            ViewBag.ControllerActionTime = DateTime.Now;
            ViewBag.ProcessId = Process.GetCurrentProcess().Id;

            scope = Startup.TimingLogger.BeginScope("Action=" + context.ActionDescriptor.Name);
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            scope?.Dispose();
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult OtherIndex()
        {
            return Content("TimeFrom start: " + ViewBag.TimeFromStart + " process id:" + ViewBag.ProcessId, "text/html");
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