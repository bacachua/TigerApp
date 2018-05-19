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
using EventManager.ApiModels;


namespace EventManager.Web.Controllers
{
	[AllowAnonymous]
	public class CityController : ApiController
	{
		// GET: api/City
		[AllowAnonymous]
		[HttpGet]
		public APIResponse Get()
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
				foreach (var c in cities)
				{

					IEventCampaignBusinessService evtCampaignBusinessService = new EventCampaignBusinessService();
					List<ApiEventCampaignModel> evtList = evtCampaignBusinessService.GetListByCity(c.CityID);
					if (evtList != null && (evtList.Count > 0))
					{
						if (evtList.Count > 1)
						{
							c.StartDate = evtList[0].StartDateTime;
							c.EndDate = evtList[evtList.Count - 1].EndDateTime;
						}
						else
						{
							c.StartDate = evtList[0].StartDateTime;
							c.EndDate = evtList[0].EndDateTime;
						}
					}

				}

				return new APIResponse() { Status = eResponseStatus.Success, Result = cities.ToList() };
			}
		}


		// GET: api/City/5
		[AllowAnonymous]
		[HttpGet]
		[Route("api/City/Get/{cityId}")]
		public APIResponse Get(int cityId)
		{
			List<CityModel> cities = null;
			using (IDataContextAsync context = new GameManagerContext())
			using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
			{
				IRepositoryAsync<City> campaignRepository = new Repository<City>(context, unitOfWork);
				ICityBusinessService billingService = new CityBusinessService(campaignRepository);

				IQueryFluent<City> q = campaignRepository.Query(x => x.CityID == cityId);
				cities = q.Select(y => new CityModel { CityID = y.CityID, Name = y.Name }).ToList();

				foreach (var c in cities)
				{

					IEventCampaignBusinessService evtCampaignBusinessService = new EventCampaignBusinessService();
					List<ApiEventCampaignModel> evtList = evtCampaignBusinessService.GetListByCity(c.CityID);
					if (evtList != null && (evtList.Count > 0))
					{
						if (evtList.Count > 1)
						{
							c.StartDate = evtList[0].StartDateTime;
							c.EndDate = evtList[evtList.Count - 1].EndDateTime;
						}
						else
						{
							c.StartDate = evtList[0].StartDateTime;
							c.EndDate = evtList[0].EndDateTime;
						}
					}

				}
			}

			return new APIResponse() { Status = eResponseStatus.Success, Result = cities.ToList() };
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
