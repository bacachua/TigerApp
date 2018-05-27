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
using System.Drawing;
using System.Drawing.Imaging;

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
		public APIResponse GetListAvailableByCityEventPeriod(EventCampaignByCityEventPeriod model)
		{
			IEventCampaignBusinessService eVentCampaignSrv = new EventCampaignBusinessService();
			var result = eVentCampaignSrv.GetListAvailableByCityEventPeriod(model.CityID, model.StartDateTime, model.EndDateTime);
			return new APIResponse() { Status = eResponseStatus.Success, Result = result };
		}

        [AllowAnonymous]
        [HttpGet]
        public APIResponse EventCampaignDetail(int id)
        {
            IEventCampaignBusinessService eVentCampaignSrv = new EventCampaignBusinessService();
            var result = eVentCampaignSrv.GetEventCampaignDetail(id);
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
			bool result ;            
			try
			{
				IEventCampaignBusinessService eVentCampaignSrv = new EventCampaignBusinessService();
				result = eVentCampaignSrv.RegisterEvent(model);
			}catch(Exception ex)
			{
				return new APIResponse() { Status = eResponseStatus.Fail, Result = "Đăng ký trò chơi Không thành công" };
			}
           
			if (!result)
			{
				return new APIResponse() { Status = eResponseStatus.Fail, Result = "Đăng ký trò chơi Không thành công" };
			}
			return new APIResponse() { Status = eResponseStatus.Success, Result = "Đăng ký trò chơi thành công" };
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
		[Route("api/EventCampaign/GetEventCampaignById/{campaignById}")]
		public APIResponse GetEventCampaignById(int campaignById)
		{
			IEventCampaignBusinessService eVentCampaignSrv = new EventCampaignBusinessService();
			var result = eVentCampaignSrv.GetEventCampaignById(campaignById);
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
