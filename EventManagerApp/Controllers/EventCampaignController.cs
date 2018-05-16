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
    }
}
