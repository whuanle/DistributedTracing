using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    public class FormatController : Controller
    {
        [HttpGet]
        public string Get()
        {
            return "Hello!";
        }

        [HttpGet("{helloTo}", Name = "GetFormat")]
        public string Get(string helloTo)
        {
            var formattedHelloString = $"Hello, {helloTo}!";
            return formattedHelloString;
        }
    }
}
