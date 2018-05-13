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
		public string Get()
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
						Status = "failed",
						Message = "not found"
					};
					
					return Newtonsoft.Json.JsonConvert.SerializeObject(myError);
					
				}
				else
				{
					//ret = Ok(test);
					//DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(List<EventCampaign>));
					//MemoryStream msObj = new MemoryStream();
					//js.WriteObject(msObj, eventCampaignList);
					//msObj.Position = 0;
					//StreamReader sr = new StreamReader(msObj);

					//// "{\"Description\":\"Share Knowledge\",\"Name\":\"C-sharpcorner\"}"  
					//string json = sr.ReadToEnd();

					//sr.Close();
					//msObj.Close();  
					return Newtonsoft.Json.JsonConvert.SerializeObject(eventCampaignList);
					//return json;
				}


			}
        }

        // GET: api/EventCampaign/5
		[AllowAnonymous]
		[HttpGet]
        public string Get(int id)
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
						Status = "failed",
						Message = "not found"
					};

					return Newtonsoft.Json.JsonConvert.SerializeObject(myError);

				}
				else
				{
					return Newtonsoft.Json.JsonConvert.SerializeObject(eventCampaign);

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
