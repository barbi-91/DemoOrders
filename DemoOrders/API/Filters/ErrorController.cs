using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace DemoOrders.API.Filters
{
    [Route("api/[controller]")]
    [ApiController]
    public class ErrorController : ControllerBase
    {
        [Route("/error")]
        [HttpGet]
        public IActionResult HandleError()
        {
            var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var exception = context?.Error;

            // You can log the exception here if needed

            return Problem(
                detail: exception?.Message,
                statusCode: 500,
                title: "An unexpected error occurred."
            );
        }
    }
}
