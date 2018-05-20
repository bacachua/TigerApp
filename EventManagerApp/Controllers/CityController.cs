using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

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

using System.Net.Http;
using System.Web.Http;
using JsonApiSerializer;


namespace EventManager.Web.Controllers
{
	[AllowAnonymous]
    public class CityController : ApiController
    {
        // GET: api/City
		[AllowAnonymous]
		[HttpGet]

		public HttpResponseMessage Get()
        {
			List<CityModel> cities = null;
			using (IDataContextAsync context = new GameManagerContext())
			using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
			{
				IRepositoryAsync<City> campaignRepository = new Repository<City>(context, unitOfWork);
				ICityBusinessService billingService = new CityBusinessService(campaignRepository);

				IQueryFluent<City> q = campaignRepository.Query(x => x.CityID > 0);
				cities = q.Select(y => new CityModel { CityID = y.CityID, Name = y.Name }).ToList();
				//eventCampaignList = q.SelectAsync().Result.ToList();
				//(t => new EventCampaignModel { EventCampaignID  = q.EventCampaignID});

				//eventCampaignList = asyncTask.Result.Select( new EventCampaignModel{ }};

				if (cities == null)
				{
					var myError = new Error
					{
						Status = Resources.ApiMsg.Failed,
						Message = Resources.ApiMsg.NoRecordFound
					};
					
					return Request.CreateResponse(HttpStatusCode.OK, myError);
					//throw new HttpResponseException(HttpStatusCode.NotFound);
				}
				else
				{
					return Request.CreateResponse(HttpStatusCode.OK, cities);
				}
			}
        }

        // GET: api/City/5
		public HttpResponseMessage Get(int id)
        {
			CityModel eventCampaignList = null;
			using (IDataContextAsync context = new GameManagerContext())
			using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
			{
				IRepositoryAsync<City> campaignRepository = new Repository<City>(context, unitOfWork);
				ICityBusinessService billingService = new CityBusinessService(campaignRepository);

				IQueryFluent<City> q = campaignRepository.Query(x => x.CityID == id);
				eventCampaignList = q.Select(y => new CityModel { CityID = y.CityID, Name = y.Name }).FirstOrDefault();
				
				if (eventCampaignList == null)
				{
					var myError = new Error
					{
						Status = Resources.ApiMsg.Failed,
						Message = Resources.ApiMsg.NoRecordFound
					};
					return Request.CreateResponse(HttpStatusCode.OK, myError);
					//throw new HttpResponseException(HttpStatusCode.NotFound);
				}
				else
				{
					return Request.CreateResponse(HttpStatusCode.OK, eventCampaignList);
				}
			}
        }

        // POST: api/City
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/City/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/City/5
        public void Delete(int id)
        {
        }
    }
}
