using Microsoft.AspNetCore.Mvc;

namespace MasterNet.WebApi.Controllers
{
    [ApiController]
    [Route("Demo")]
    public class DemoController
    {
        [HttpGet("getString")]
        public string GetNombre()
        {
            return "Alder";
        }
    }
}
