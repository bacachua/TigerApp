using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace EventManager.Web.Controllers
{
    [Authorize]
	[RoutePrefix("api/Values")]
    public class ValuesController : ApiController
    {
        // GET api/values
        public string Get()
        {
            var userName = this.RequestContext.Principal.Identity.Name;
            return String.Format("Hello, {0}.", userName);
        }

		[AllowAnonymous]
		[HttpGet]
		
		[Route("api/Values/Hello")]
		public string Hello(string userName)
		{
			return String.Format("Hello, {0}.", userName);
		}
    }
}
