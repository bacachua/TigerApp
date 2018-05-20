using EventManager.DataModel.Models;
using EventManager.Web.Models;
using EventManager.Web.Results;
using Repository.Pattern.DataContext;
using Repository.Pattern.Ef6;
using Repository.Pattern.Repositories;
using Repository.Pattern.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using EventManager.BusinessService;
using System.Web.Http;
using Repository.Pattern.Infrastructure;
using System.Data.Entity.Validation;


namespace EventManager.Web.Controllers
{
	 [RoutePrefix("api/EventRegister")]
    public class EventRegisterController : ApiController
    {
        // GET: api/EventRegister
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/EventRegister/5
		[AllowAnonymous]
		[HttpGet]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/EventRegister
		[AllowAnonymous]
		[HttpPost]
		public HttpResponseMessage Post(EventRegisterModel model)
        {
			if (!ModelState.IsValid)
			{
				var myError = new Error
				{
					Status = Resources.ApiMsg.Failed,
					Message = Resources.ApiMsg.NotValidModel
				};
				return Request.CreateResponse(HttpStatusCode.OK, myError);
				//return Newtonsoft.Json.JsonConvert.SerializeObject(myError);
			}

			var eventRegisterModel = new EventRegisterModel()
			{
				UserId = model.UserId,
				StartDateTime = model.StartDateTime,
				EventCampaignID = model.EventCampaignID,
				Status = model.Status 
			};

			try
			{
				// TODO: Add update logic here
				EventRegister eventRegister = new EventRegister();
				eventRegister.ObjectState = ObjectState.Added;
				eventRegister.EventCampaignID= model.EventCampaignID;
				eventRegister.UserId = model.UserId;
				eventRegister.StartDateTime = model.StartDateTime;
				eventRegister.Status = 0;
				using (IDataContextAsync context = new GameManagerContext())
				using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
				{
					IRepositoryAsync<EventRegister> customerRepository = new Repository<EventRegister>(context, unitOfWork);
					IEventRegisterBusinessService customerService = new EventRegisterBusinessService(customerRepository);
					// Testing changes to graph while disconncted from it's orginal DataContext
					// Saving changes while graph was previous DataContext that was already disposed
					customerRepository.Insert(eventRegister);
					// customerRepository.InsertOrUpdateGraph(customerForUpdateDeleteGraphTest);
					unitOfWork.SaveChanges();
				}
			}
			catch (DbEntityValidationException ex)
			{
				var error = ex.EntityValidationErrors.First().ValidationErrors.First();
				//this.ModelState.AddModelError(error.PropertyName, Resources.UIValidation.DealerNameRequied);
				var myError = new Error
				{
					Status = Resources.ApiMsg.Failed,
					Message = Resources.ApiMsg.InternalError + ex.Message
				};
				return Request.CreateResponse(HttpStatusCode.OK, myError);
				//return Newtonsoft.Json.JsonConvert.SerializeObject(myError);

			}
			catch (Exception ex)
			{
				//Debug.WriteLine(ex.Message);
				//return RedirectToAction("List");
				var myError = new Error
				{
					Status = Resources.ApiMsg.Failed,
					Message = Resources.ApiMsg.InternalError + ex.Message
				};
				return Request.CreateResponse(HttpStatusCode.OK, myError);
				//return Newtonsoft.Json.JsonConvert.SerializeObject(myError);
			}

			var okResult = new Error
			{
				Status = Resources.ApiMsg.Success,
				Message = "Game successfully registered for user"
			};
			return Request.CreateResponse(HttpStatusCode.OK, okResult);
			//return Newtonsoft.Json.JsonConvert.SerializeObject(okResult);
        }

        // PUT: api/EventRegister/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/EventRegister/5
        public void Delete(int id)
        {
        }

		[AllowAnonymous]
		[HttpGet]

		
		public string Tuan(int id)
		{
			return id.ToString();
		}

		[AllowAnonymous]
		[HttpGet]
		[Route("api/api/EventRegister/ByUser")]
		public APIResponse RegisterEventByUser(string userId)
		{
			List<EventRegisterModel> eventCampaignList = null;
			using (IDataContextAsync context = new GameManagerContext())
			using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
			{
				IRepositoryAsync<EventRegister> eventRegisterRepository = new Repository<EventRegister>(context, unitOfWork);
				IEventRegisterBusinessService billingService = new EventRegisterBusinessService(eventRegisterRepository);

				IQueryFluent<EventRegister> q = eventRegisterRepository.Query(x => x.UserId == userId);
				eventCampaignList = q.Select(y => new EventRegisterModel { UserId = y.UserId, StartDateTime = y.StartDateTime, EventCampaignID =y.EventCampaignID, Status = y.Status,NumberOfPlayer1Time = y.NumberOfPlayer1Time, TimeToPlayPerSession =  y.TimeToPlayPerSession }).ToList();

				if (eventCampaignList == null)
				{
					return new APIResponse() { Status = eResponseStatus.Fail, Result = Resources.ApiMsg.NoRecordFound };			
		     		//throw new HttpResponseException(HttpStatusCode.NotFound);
				}
				else
				{
					return new APIResponse() { Status = eResponseStatus.Success, Result = eventCampaignList };		
					//return Request.CreateResponse(HttpStatusCode.OK, eventCampaignList);
				}
			}
		}
    }
}
