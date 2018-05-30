using Swashbuckle.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Description;

namespace EventManager.Web.Controllers
{
	public class FileOperationFilterTemp : IOperationFilter
	{
		public void Apply(Operation operation, SchemaRegistry schemaRegistry, ApiDescription apiDescription)
		{
			if (operation.operationId.ToLower() == "Account_PostSignatureImage")
			{
				if (operation.parameters == null)
					operation.parameters = new List<Parameter>(1);
				else
					operation.parameters.Clear();
				operation.parameters.Add(new Parameter
				{
					name = "upFile",
					@in = "formData",
					description = "Upload software package",
					required = true,
					type = "file"
				});
				operation.consumes.Add("multipart/form-data");
			}
		}
	}
}