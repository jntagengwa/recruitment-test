using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InterviewTest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ListController : ControllerBase
    {
        private readonly ILogger<ListController> logger;

        public ListController(ILogger<ListController> logger)
        {
            this.logger = logger;
        }

        /*
         * List API methods goe here
         * */
    }
}
