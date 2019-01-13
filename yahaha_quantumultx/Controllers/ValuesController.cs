using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace yahaha_quantumultx.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        [HttpGet("byName")]
        public ActionResult<string> Get(string url, string exprie)
        {
            return $"{url},{exprie}";
        }
    }
}
