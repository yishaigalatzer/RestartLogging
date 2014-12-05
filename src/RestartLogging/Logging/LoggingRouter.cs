using System.Threading.Tasks;
using Microsoft.AspNet.Routing;
using Microsoft.Framework.Logging;

namespace RestartLogging.Logging
{
    public class LoggingRouter : IRouter
    {
        private readonly IRouter _innerRouter;
        private readonly ILogger _logger;

        public LoggingRouter(IRouter innerRouter, ILogger logger)
        {
            _innerRouter = innerRouter;
            _logger = logger;
        }

        public string GetVirtualPath(VirtualPathContext context)
        {
            return _innerRouter.GetVirtualPath(context);
        }

        public async Task RouteAsync(RouteContext context)
        {
            using (_logger.BeginScope("EnterMVC"))
            {
                await _innerRouter.RouteAsync(context);
            }
        }
    }
}
