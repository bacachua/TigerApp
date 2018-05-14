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
        // GET: api/EventCampaig
        [AllowAnonymous]
		[HttpGet]
		public HttpResponseMessage Get()
        {
			List<EventCampaignModel> eventCampaignList = null;
			using (IDataContextAsync context = new GameManagerContext())
			using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
			{
				IRepositoryAsync<EventCampaign> campaignRepository = new Repository<EventCampaign>(context, unitOfWork);
				IEventCampaignBusinessService billingService = new EventCampaignBusinessService(campaignRepository);

				IQueryFluent<EventCampaign> q = campaignRepository.Query(x => x.EventCampaignID > 0);
				eventCampaignList = q.Select(y => new EventCampaignModel { EventCampaignID = y.EventCampaignID, EventID = y.EventID, EventName = y.Event.Name, CityID = y.City.CityID, CityName = y.City.Name, StartDateTime = y.StartDateTime, EndDateTime = y.EndDateTime, TimeToPlayPerSession = y.TimeToPlayPerSession, NumberOfPlayer1Time = y.NumberOfPlayer1Time, Active = y.Active, WaitingTime = 0 }).ToList();
				//eventCampaignList = q.SelectAsync().Result.ToList();
				//(t => new EventCampaignModel { EventCampaignID  = q.EventCampaignID});
				//eventCampaignList = asyncTask.Result.Select( new EventCampaignModel{ }};
				
				if (eventCampaignList == null)
				{
					var myError = new Error
					{
						Status = Resources.ApiMsg.Failed,
						Message = Resources.ApiMsg.NoRecordFound
					};

					return Request.CreateResponse(HttpStatusCode.OK, myError);					
				}
				else
				{
					return Request.CreateResponse(HttpStatusCode.OK, eventCampaignList);
				}
			}
        }

        // GET: api/EventCampaign/5
		[AllowAnonymous]
		[HttpGet]
		public HttpResponseMessage Get(int id)
        {
           	EventCampaignModel eventCampaign = null;
			using (IDataContextAsync context = new GameManagerContext())
			using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
			{
				IRepositoryAsync<EventCampaign> campaignRepository = new Repository<EventCampaign>(context, unitOfWork);
				IEventCampaignBusinessService billingService = new EventCampaignBusinessService(campaignRepository);

				IQueryFluent<EventCampaign> q = campaignRepository.Query(x => x.EventCampaignID == id);
				eventCampaign = q.Select(y => new EventCampaignModel { EventCampaignID = y.EventCampaignID, EventID = y.EventID,EventName = y.Event.Name, CityID = y.City.CityID, CityName = y.City.Name, StartDateTime = y.StartDateTime, EndDateTime = y.EndDateTime, TimeToPlayPerSession = y.TimeToPlayPerSession, NumberOfPlayer1Time = y.NumberOfPlayer1Time, Active = y.Active, WaitingTime = 0 }).FirstOrDefault();
				//eventCampaignList = q.SelectAsync().Result.ToList();
				//(t => new EventCampaignModel { EventCampaignID  = q.EventCampaignID});
				//eventCampaignList = asyncTask.Result.Select( new EventCampaignModel{ }};

				if (eventCampaign == null)
				{
					var myError = new Error
					{
						Status = Resources.ApiMsg.Failed,
						Message = Resources.ApiMsg.NoRecordFound
					};
					return Request.CreateResponse(HttpStatusCode.OK, myError);	
				}
				else
				{
					return Request.CreateResponse(HttpStatusCode.OK, eventCampaign);
				}
			}
        }

		
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
            var result = new List<ApiEventCampaignModel>();
            using (IDataContextAsync context = new GameManagerContext())
            using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
            {
                IRepositoryAsync<EventCampaign> campaignRepository = new Repository<EventCampaign>(context, unitOfWork);
                IEventCampaignBusinessService eVentCampaignSrv = new EventCampaignBusinessService(campaignRepository);
                result = eVentCampaignSrv.GetListAvailable();
            }
            return new APIResponse() { Status = eResponseStatus.Success, Result = result };
        }
    }
}
