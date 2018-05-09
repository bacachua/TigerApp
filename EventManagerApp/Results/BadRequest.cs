using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Utilities;
using Newtonsoft.Json;
using System.Web.Mvc;

namespace EventManager.Web.Results
{
	public class BadRequest : JsonResult
	{
		public BadRequest()
		{
		}

		public BadRequest(string message)
		{
			this.Data = message;
		}

		public BadRequest(object data)
		{
			this.Data = data;
		}

		public override void ExecuteResult(ControllerContext context)
		{
			context.RequestContext.HttpContext.Response.StatusCode = 400;
			base.ExecuteResult(context);
		}

	}

}