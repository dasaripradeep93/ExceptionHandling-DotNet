using Microsoft.AspNetCore.Mvc;

namespace ExceptionHandlingSerilog.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ExceptionsController : Controller
    {

        private readonly ILogger<ExceptionsController> _logger;

        public ExceptionsController(ILogger<ExceptionsController> logger)
        {
            _logger = logger;
            _logger.LogInformation("ExceptionsController constructor called");
        }

        [HttpGet]
        [Route("GetException")]
        public void GetException()
        {
            try
            {
                _logger.LogInformation("GetException method called");
                int i = 0;
                int err = 12 % i;
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Error occurred in GetException controller");
            }
        }
    }
}
