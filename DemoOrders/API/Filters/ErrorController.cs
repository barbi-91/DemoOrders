using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace DemoOrders.API.Filters
{
    [Route("api/[controller]")]
    [ApiController]
    public class ErrorController : ControllerBase
    {
        private readonly ILogger<ErrorController> _logger;
        public ErrorController(ILogger<ErrorController> logger)
        {
            _logger = logger;
        }

        [Route("/error")]
        [HttpGet]
        public IActionResult HandleError()
        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var exception = context?.Error;

            // Loging the exception here if needed
            _logger.LogError(exception, "Unhandled exception occurred.");

            return Problem(
                detail: exception?.Message,
                statusCode: 500,
                title: "An unexpected error occurred."
            );
        }
    }
}
