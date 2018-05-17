using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using EventManager.DataModel;
using EventManager.BusinessService;
using EventManager.Repository;
using Repository.Pattern.DataContext;
using Repository.Pattern.UnitOfWork;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.Repositories;
using Repository.Pattern.Ef6;
using Newtonsoft.Json;
using EventManager.DataModel.Models;
using EventManager.Web.Results;
using System.Runtime.Serialization.Json;
using System.IO;
using EventManager.Web.Models;
using EventManager.ApiModels;
using System.Web;

namespace EventManager.Web.Controllers
{
	
    public class EventCampaignController : ApiController
    {		
        // POST: api/EventCampaign
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/EventCampaign/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/EventCampaign/5
        public void Delete(int id)
        {
        }

        [AllowAnonymous]
        [HttpGet]
        public APIResponse GetListAvailable()
        {
            IEventCampaignBusinessService eVentCampaignSrv = new EventCampaignBusinessService();
            var result = eVentCampaignSrv.GetListAvailable();
            return new APIResponse() { Status = eResponseStatus.Success, Result = result };
        }
        [AllowAnonymous]
        [HttpPost]
        public APIResponse IsValidTimeRegister(ApiEventRegisterModel model)
        {
            IEventCampaignBusinessService eVentCampaignSrv = new EventCampaignBusinessService();
            var result = eVentCampaignSrv.IsValidTimeRegister(model);
            return new APIResponse() { Status = eResponseStatus.Success, Result = result };
        }
        [AllowAnonymous]
        [HttpPost]
        public APIResponse RegisterEvent(ApiEventRegisterModel model)
        {
            IEventCampaignBusinessService eVentCampaignSrv = new EventCampaignBusinessService();
            var result = eVentCampaignSrv.RegisterEvent(model);
            return new APIResponse() { Status = eResponseStatus.Success, Result = result };
        }

		[AllowAnonymous]
		[HttpPost]
		public APIResponse EventCampaignByCity(int cityId)
		{
			IEventCampaignBusinessService eVentCampaignSrv = new EventCampaignBusinessService();
			var result = eVentCampaignSrv.GetListByCity(cityId);
			return new APIResponse() { Status = eResponseStatus.Success, Result = result };
		}
        [AllowAnonymous]
        [HttpGet]
        [Route("api/EventCampaign/GetEventRegisterByQRCode/{qrCode}")]
        public APIResponse GetEventRegisterByQRCode(string qrCode)
        {
            IEventCampaignBusinessService eVentCampaignSrv = new EventCampaignBusinessService();
            var result = eVentCampaignSrv.GetEventRegisterByQRCode(qrCode);
            return new APIResponse() { Status = eResponseStatus.Success, Result = result };
        }

        [Route("api/EventCampaign/PostSignatureImage")]
        [AllowAnonymous]
        public APIResponse PostSignatureImage()
        {
            var filePath = HttpContext.Current.Server.MapPath("~/SignatureImages/");
            string message = "";
            eResponseStatus status = eResponseStatus.Success;
            try
            {                
                var httpRequest = HttpContext.Current.Request;
                var postedFile = httpRequest.Files[0];
                if (postedFile != null && postedFile.ContentLength > 0)
                {
                    int MaxContentLength = 1024 * 1024 * 1; //Size = 1 MB
                    IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".gif", ".png" };
                    var ext = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf('.'));
                    var extension = ext.ToLower();
                    if (!AllowedFileExtensions.Contains(extension))
                    {
                        status = eResponseStatus.Fail;
                        message = string.Format("Please Upload image of type .jpg,.gif,.png.");
                    }
                    else if (postedFile.ContentLength > MaxContentLength)
                    {
                        status = eResponseStatus.Fail;
                        message = string.Format("Please Upload a file upto 1 mb.");
                    }
                    else
                    {
                        filePath = filePath + postedFile.FileName + extension;
                        postedFile.SaveAs(filePath);
                    }
                }
            }
            catch (Exception ex)
            {
                status = eResponseStatus.Fail;
                message = ex.Message;
            }
            return new APIResponse() { Status = status, Message = message, Result = filePath };
        }
    }
}
