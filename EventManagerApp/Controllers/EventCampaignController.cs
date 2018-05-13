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
				eventCampaignList = q.Select(y => new EventCampaignModel { EventCampaignID = y.EventCampaignID, CityID = y.City.CityID, EventID = y.EventID, StartDateTime = y.StartDateTime, EndDateTime = y.EndDateTime, TimeToPlayPerSession = y.TimeToPlayPerSession, NumberOfPlayer1Time = y.NumberOfPlayer1Time, Active = y.Active, WaitingTime = 0, CityName = y.City.Name }).ToList();
							
				if (eventCampaignList == null)
				{
					throw new HttpResponseException(HttpStatusCode.NotFound);
					
				}
				else
				{
					return Request.CreateResponse(eventCampaignList);					
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
				eventCampaign = q.Select(y => new EventCampaignModel { EventCampaignID = y.EventCampaignID, CityID = y.City.CityID, EventID = y.EventID, StartDateTime = y.StartDateTime, EndDateTime = y.EndDateTime, TimeToPlayPerSession = y.TimeToPlayPerSession, NumberOfPlayer1Time = y.NumberOfPlayer1Time, Active = y.Active, WaitingTime = 0, CityName = y.City.Name }).FirstOrDefault();
				if (eventCampaign == null)
				{
					throw new HttpResponseException(HttpStatusCode.NotFound);

				}
				else
				{
					return Request.CreateResponse(eventCampaign);
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
    }
}
