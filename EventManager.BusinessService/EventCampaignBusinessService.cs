using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventManager.DataModel.Models;
using Service.Pattern;
using EventManager.Repository;
using Repository.Pattern;
using Repository.Pattern.Repositories;
using EventManager.ApiModels;
using Repository.Pattern.DataContext;
using Repository.Pattern.UnitOfWork;
using Repository.Pattern.Ef6;
using System.Linq.Expressions;
using System.IO;
using System.Globalization;
using Repository.Pattern.Infrastructure;
using LinqKit;
namespace EventManager.BusinessService
{
    public interface IEventCampaignBusinessService
    {
        List<ApiEventCampaignModel> GetListAvailable();
        bool IsValidTimeRegister(ApiEventRegisterModel model, out int code);
        bool RegisterEvent(ApiEventRegisterModel model, out int status);
		List<ApiEventCampaignModel> GetListByCity(int cityid);
        void SendNotificationBeforeOrLateEventTime(int numOfMinute, string to, string message, eEventRegisterStatus status);
		List<ApiEventRegisterUserModel> GetEventRegisterByQRCode(string qrCode);
		ApiEventCampaignModel GetEventCampaignById(int campaignId);
		List<ApiEventCampaignModel> GetListAvailableByCityEventPeriod(int cityId, DateTime fromDateTime, DateTime toDateTime);
        ApiEventCampaignModel GetEventCampaignDetail(int id);
    }

    public class EventCampaignBusinessService : IEventCampaignBusinessService
    {
        private IRepositoryAsync<EventCampaign> _repository;
        private IRepositoryAsync<EventRegister> _eventRegisterRepo;
        private INotificationService iNotificationService;
        public EventCampaignBusinessService()
        {

        }

		public List<ApiEventCampaignModel> GetListAvailableByCityEventPeriod(int cityId, DateTime fromDateTime, DateTime toDateTime)
		{
			var currentTime = DateTime.Now;
			
			if (fromDateTime > toDateTime)
				return null;
			if (toDateTime < currentTime)
				return null;
			List<ApiEventCampaignModel> models = new List<ApiEventCampaignModel>();
			using (IDataContextAsync context = new GameManagerContext())
			using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
			{
				_repository = new Repository<EventCampaign>(context, unitOfWork);
				//var currentTime = DateTime.Now;
				var entities = _repository.AllIncluding(c => c.City, c => c.Event, c => c.EventRegisters).Where((c => c.StartDateTime >= fromDateTime && c.EndDateTime <= toDateTime && (c.CityID == cityId))).OrderBy(c => c.EventCampaignID).OrderBy(c => c.StartDateTime).ToList();
				models = entities.Select(c => new ApiEventCampaignModel()
				{
					EventCampaignID = c.EventCampaignID,
					EventName = c.Event.Name,
					EventID = c.Event.EventID,
					ImagePath = c.Event.ImagePath,
					CityName = c.City.Name,
					CityID = c.CityID,
					StartDateTime = c.StartDateTime,
					EndDateTime = c.EndDateTime,
					TimeToPlayPerSession = c.TimeToPlayPerSession,
					NumberOfPlayer1Time = c.NumberOfPlayer1Time
				}).ToList();
				List<int> statusList = new List<int>() { (int)eEventRegisterStatus.New, (int)eEventRegisterStatus.Reminded };
				foreach (var item in models)
				{
					item.EventCampaignTimeAvailables = new List<EventCampaignTimeAvailable>();
					var entity = entities.FirstOrDefault(c => c.EventCampaignID == item.EventCampaignID);
					var currentDate = DateTime.Now;
					var startTime = new DateTime(item.StartDateTime.Value.Year, item.StartDateTime.Value.Month, item.StartDateTime.Value.Day, item.StartDateTime.Value.Hour, item.StartDateTime.Value.Minute, 0);
					while (startTime < item.EndDateTime.Value)
					{
						if (startTime > currentDate)
						{

							var eventRegisters = entity.EventRegisters.Where(c => c.StartDateTime == startTime && statusList.Contains(c.Status)).ToList();
							var numOfPlayerAvailable = item.NumberOfPlayer1Time - eventRegisters.Sum(c => c.NumberOfPlayer1Time);

							if (numOfPlayerAvailable > 0)
							{
								item.EventCampaignTimeAvailables.Add(new EventCampaignTimeAvailable()
								{
									NumberOfPlayer1Time = numOfPlayerAvailable,
									TimeAvailableToPlay = startTime
								});
							}
						}
						startTime = startTime.AddMinutes(item.TimeToPlayPerSession.Value);
					}
					if (item.EventCampaignTimeAvailables.Count() > 0)
					{
						item.TimeAvailableToPlay = item.EventCampaignTimeAvailables[0].TimeAvailableToPlay;
						item.NumberOfPlayer1Time = item.EventCampaignTimeAvailables[0].NumberOfPlayer1Time;
					}
				}
			}
			return models;
			/*List<ApiEventCampaignModel> models = new List<ApiEventCampaignModel>();
			using (IDataContextAsync context = new GameManagerContext())
			using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
			{
				_repository = new Repository<EventCampaign>(context, unitOfWork);

				var entities = _repository.AllIncluding(c => c.City, c => c.Event, c => c.EventRegisters).Where((c => c.StartDateTime >= fromDateTime  && c.EndDateTime > currentTime && (c.CityID == cityId))).OrderBy(c => c.EventCampaignID).OrderBy(c => c.StartDateTime).ToList();
				models = entities.Select(c => new ApiEventCampaignModel()
				{
					EventCampaignID = c.EventCampaignID,
					EventName = c.Event.Name,
					EventID = c.EventID,
					CityName = c.City.Name,
					CityID = c.CityID,
					StartDateTime = c.StartDateTime,
					EndDateTime = c.EndDateTime,
					TimeToPlayPerSession = c.TimeToPlayPerSession,
					NumberOfPlayer1Time = c.NumberOfPlayer1Time
				}).ToList();
				List<int> statusList = new List<int>() { (int)eEventRegisterStatus.New, (int)eEventRegisterStatus.Reminded };
				foreach (var item in models)
				{
					var entity = entities.FirstOrDefault(c => c.EventCampaignID == item.EventCampaignID);
					var startTime = new DateTime(item.StartDateTime.Value.Year, item.StartDateTime.Value.Month, item.StartDateTime.Value.Day, item.StartDateTime.Value.Hour, item.StartDateTime.Value.Minute, 0);
					while (startTime <= item.EndDateTime.Value)
					{
						var eventRegisters = entity.EventRegisters.Where(c => c.StartDateTime == startTime && statusList.Contains(c.Status)).ToList();
						var numOfPlayerAvailable = item.NumberOfPlayer1Time - eventRegisters.Sum(c => c.NumberOfPlayer1Time);
						if (numOfPlayerAvailable > 0)
						{
							item.TimeAvailableToPlay = startTime;
							item.NumberOfPlayer1Time = numOfPlayerAvailable;
							break;
						}
						startTime = startTime.AddMinutes(item.TimeToPlayPerSession.Value);
					}
				}
			}
			return models;*/
		}
        public List<ApiEventCampaignModel> GetListAvailable()
        {
            List<ApiEventCampaignModel> models = new List<ApiEventCampaignModel>();
            using (IDataContextAsync context = new GameManagerContext())
            using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
            {
                _repository = new Repository<EventCampaign>(context, unitOfWork);                
                var currentTime = DateTime.Now;
                var entities = _repository.AllIncluding(c => c.City, c => c.Event, c => c.EventRegisters).Where(c => c.StartDateTime >= currentTime || c.EndDateTime > currentTime).OrderBy(c => c.EventCampaignID).OrderBy(c => c.StartDateTime).ToList();
                models = entities.Select(c => new ApiEventCampaignModel()
                {
                    EventCampaignID = c.EventCampaignID,
                   
					EventID = c.EventID,
                    CityName = c.City.Name,
					CityID  = c.CityID,
                    StartDateTime = c.StartDateTime,
                    EndDateTime = c.EndDateTime,
                    TimeToPlayPerSession = c.TimeToPlayPerSession,
                    NumberOfPlayer1Time = c.NumberOfPlayer1Time
                }).ToList();

				IRepositoryAsync<Event> _repository1;
				_repository1 = new Repository<Event>(context, unitOfWork);
				foreach(var item in models)
				{

					var test = _repository1.Find(item.EventID);
					if(test != null)
					{
						item.EventName = test.Name;
						item.ImagePath = test.ImagePath;
					}

				}
                List<int> statusList = new List<int>() { (int)eEventRegisterStatus.New, (int)eEventRegisterStatus.Reminded, (int)eEventRegisterStatus.Played,(int)eEventRegisterStatus.Late };
                foreach (var item in models)
                {
                    item.EventCampaignTimeAvailables = new List<EventCampaignTimeAvailable>();
                    var entity = entities.FirstOrDefault(c => c.EventCampaignID == item.EventCampaignID);
                    var currentDate = DateTime.Now;
                    var startTime = new DateTime(item.StartDateTime.Value.Year, item.StartDateTime.Value.Month, item.StartDateTime.Value.Day, item.StartDateTime.Value.Hour, item.StartDateTime.Value.Minute, 0);
                    while (startTime < item.EndDateTime.Value)
                    {
						if (startTime > currentDate && (startTime.Hour < 12 || startTime.Hour > 14))
                        {
							
                            var eventRegisters = entity.EventRegisters.Where(c => c.StartDateTime == startTime && statusList.Contains(c.Status) ).ToList();
                            var numOfPlayerAvailable = item.NumberOfPlayer1Time - eventRegisters.Sum(c => c.NumberOfPlayer1Time);
							
                            if (numOfPlayerAvailable > 0 )
                            {
                                item.EventCampaignTimeAvailables.Add(new EventCampaignTimeAvailable()
                                {
                                    NumberOfPlayer1Time = numOfPlayerAvailable,
                                    TimeAvailableToPlay = startTime
                                });
							}
							//else
							//{
							//	item.EventCampaignTimeAvailables.Add(new EventCampaignTimeAvailable()
							//	{
							//		NumberOfPlayer1Time = 0,
							//		TimeAvailableToPlay = startTime
							//	});
							//}
                        }
                        startTime = startTime.AddMinutes(item.TimeToPlayPerSession.Value);
                    }
                    if(item.EventCampaignTimeAvailables.Count() > 0)
                    {
                        item.TimeAvailableToPlay = item.EventCampaignTimeAvailables[0].TimeAvailableToPlay;
                        item.NumberOfPlayer1Time = item.EventCampaignTimeAvailables[0].NumberOfPlayer1Time;
					}				
                }
            }
			List<ApiEventCampaignModel> modelsCopy  = new List<ApiEventCampaignModel>();
			foreach (var item in models)
			{
				if(!item.TimeAvailableToPlay.Year.ToString().Equals("1"))
				{
					modelsCopy.Add(item);
				}
			}
			return modelsCopy;
        }
        public ApiEventCampaignModel GetEventCampaignDetail(int id)
        {
            var model = new ApiEventCampaignModel();
            using (IDataContextAsync context = new GameManagerContext())
            using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
            {
                _repository = new Repository<EventCampaign>(context, unitOfWork);
                var currentTime = DateTime.Now;
                var entity = _repository.AllIncluding(c => c.City, c => c.Event, c => c.EventRegisters).FirstOrDefault(c => c.EventCampaignID == id);
                
                model.EventCampaignID = entity.EventCampaignID;
                model.EventName = entity.Event.Name;
				model.ImagePath = entity.Event.ImagePath;
                model.CityName = entity.City.Name;
				model.CityID = entity.CityID;
                model.StartDateTime = entity.StartDateTime;
                model.EndDateTime = entity.EndDateTime;
                model.TimeToPlayPerSession = entity.TimeToPlayPerSession;
                model.NumberOfPlayer1Time = entity.NumberOfPlayer1Time;

                model.EventCampaignTimeAvailables = new List<EventCampaignTimeAvailable>();
                var currentDate = DateTime.Now;
                var startTime = new DateTime(model.StartDateTime.Value.Year, model.StartDateTime.Value.Month, model.StartDateTime.Value.Day, model.StartDateTime.Value.Hour, model.StartDateTime.Value.Minute, 0);
                while (startTime < model.EndDateTime.Value)
                {
                    if (startTime > currentDate && (startTime.Hour < 12 || startTime.Hour > 14))
                    {
                        var eventRegisters = entity.EventRegisters.Where(c => c.StartDateTime == startTime).ToList();
                        var numOfPlayerAvailable = model.NumberOfPlayer1Time - eventRegisters.Sum(c => c.NumberOfPlayer1Time);
                        if (numOfPlayerAvailable > 0)
                        {
                            model.EventCampaignTimeAvailables.Add(new EventCampaignTimeAvailable()
                            {
                                NumberOfPlayer1Time = numOfPlayerAvailable,
                                TimeAvailableToPlay = startTime
                            });
						}
						else
						{
							model.EventCampaignTimeAvailables.Add(new EventCampaignTimeAvailable()
							{
								NumberOfPlayer1Time = 0,
								TimeAvailableToPlay = startTime
							});
						}
                    }
                    startTime = startTime.AddMinutes(model.TimeToPlayPerSession.Value);
                }
                if (model.EventCampaignTimeAvailables.Count() > 0)
                {
                    model.TimeAvailableToPlay = model.EventCampaignTimeAvailables[0].TimeAvailableToPlay;
                    model.NumberOfPlayer1Time = model.EventCampaignTimeAvailables[0].NumberOfPlayer1Time;
                }
            }
            return model;
        }
        public bool IsValidTimeRegister(ApiEventRegisterModel model, out int errorCode)
        {
			errorCode = 0;
            using (IDataContextAsync context = new GameManagerContext())
            using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
            {
				IRepository<EventRegister>  _repositoryGameRegisting = new Repository<EventRegister>(context, unitOfWork);

				IEnumerable<EventRegister> checkeventRegisters = _repositoryGameRegisting.Queryable().Where(x => x.UserId == model.UserId && x.StartDateTime.Year == model.StartDateTime.Year
					&& x.StartDateTime.Month == model.StartDateTime.Month && x.StartDateTime.Day == model.StartDateTime.Day && x.Status !=1).AsEnumerable<EventRegister>();

				if (checkeventRegisters != null && checkeventRegisters.Count() >= 5)
				{
					errorCode = 1;
					return false;
				}
				
				if (checkeventRegisters != null)
				{
					DateTime OneHourInAdvance = model.StartDateTime;
					DateTime OneHourLater = model.StartDateTime;
					OneHourInAdvance = OneHourInAdvance.AddMinutes(-15);
					OneHourLater = OneHourLater.AddMinutes(15);
					foreach (EventRegister evRegist in checkeventRegisters)
					{
						if ((evRegist.StartDateTime > OneHourInAdvance) && (evRegist.StartDateTime < OneHourLater))
						{
							errorCode = 2;
							return false;
						}
					}
				}
 
				
                _repository = new Repository<EventCampaign>(context, unitOfWork);
                var entity = _repository.AllIncluding(c => c.EventRegisters).FirstOrDefault(c=>c.EventCampaignID == model.EventCampaignID);
                if(model.StartDateTime < entity.StartDateTime || model.EndDateTime >= entity.EndDateTime || (model.StartDateTime.Hour >= 12 && model.StartDateTime.Hour < 14))
                {
					errorCode = 3;
                    return false;
                }
                else
                {
                    List<int> statusList = new List<int>() { (int)eEventRegisterStatus.New, (int)eEventRegisterStatus.Reminded, (int)eEventRegisterStatus.Played,(int)eEventRegisterStatus.Late };
                    var eventRegisters = entity.EventRegisters.Where(c => c.StartDateTime == model.StartDateTime && statusList.Contains(c.Status)).ToList();
                    var numOfPlayerAvailable = entity.NumberOfPlayer1Time - eventRegisters.Sum(c => c.NumberOfPlayer1Time);
					if (numOfPlayerAvailable <= 0)
					{
						errorCode = 4;
					}
                    return numOfPlayerAvailable > 0;
                }
            }
        }

        public bool RegisterEvent(ApiEventRegisterModel model, out int status)
        {
			status = 0;
			var valid = IsValidTimeRegister(model, out status);
            if (valid)
            {
                EventRegister entity = new EventRegister();
                entity.Status = (int)eEventRegisterStatus.New;                
                entity.EventCampaignID = model.EventCampaignID;
                entity.NumberOfPlayer1Time = model.NumberOfPlayer1Time;
                entity.StartDateTime = model.StartDateTime;
                entity.EndDateTime = model.StartDateTime.AddMinutes(model.TimeToPlayPerSession);
                entity.TimeToPlayPerSession = model.TimeToPlayPerSession;
                entity.UserId = model.UserId.ToString();
                entity.Active = true;
				entity.Rewarded = "N";
				entity.PlayedDate = DateTime.Now;
                using (IDataContextAsync context = new GameManagerContext())
                using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
                {
                    _eventRegisterRepo = new Repository<EventRegister>(context, unitOfWork);
                    _eventRegisterRepo.Insert(entity);
                    unitOfWork.SaveChanges();
                }
            }
            return valid;
        }
        public void SendNotificationBeforeOrLateEventTime(int numOfMinute, string to, string message, eEventRegisterStatus status)
        {
            var currentDate = DateTime.Now;
            var compareDate = currentDate.AddMinutes(numOfMinute);

            Expression<Func<EventRegister, bool>> filter = c => true;
            if (status == eEventRegisterStatus.Reminded)
            {
                filter = filter.And(c => c.Status == (int)eEventRegisterStatus.New && c.StartDateTime > currentDate && c.StartDateTime <= compareDate);
            }
            else//late
            {
                filter = filter.And(c =>
                                        (c.Status == (int)eEventRegisterStatus.New || c.Status == (int)eEventRegisterStatus.Reminded)
                                        &&
                                        compareDate > c.StartDateTime
                                        );
            }
            using (IDataContextAsync context = new GameManagerContext())
            using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
            {
                _eventRegisterRepo = new Repository<EventRegister>(context, unitOfWork);
                var entities = _eventRegisterRepo.AllIncluding(c=>c.AspNetUser).Where(filter).ToList();
                if(entities.Count() > 0)
                {
                    iNotificationService = new NotificationService();
                    foreach (var item in entities)
                    {
                        iNotificationService.NotifyAsync(item.AspNetUser.DeviceId, message);
                        item.Status = (int)status;
                        item.AspNetUser = null;
                        _eventRegisterRepo.Update(item);
                    }
                    unitOfWork.SaveChanges();
                }
            }
        }
        public List<ApiEventCampaignModel> GetListByCity(int cityid)
		{
			/*List<ApiEventCampaignModel> models = new List<ApiEventCampaignModel>();
			using (IDataContextAsync context = new GameManagerContext())
			using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
			{
				_repository = new Repository<EventCampaign>(context, unitOfWork);
				var currentTime = DateTime.Now;
				var entities = _repository.AllIncluding(c => c.City, c => c.Event, c => c.EventRegisters).Where( (c => c.StartDateTime >= currentTime && c.CityID == cityid)).OrderBy(c => c.EventCampaignID).OrderBy(c => c.StartDateTime).ToList();
				models = entities.Select(c => new ApiEventCampaignModel()
				{
					EventCampaignID = c.EventCampaignID,
					EventID = c.EventID,
					EventName = c.Event.Name,
					CityName = c.City.Name,
					CityID = c.CityID,
					StartDateTime = c.StartDateTime,
					EndDateTime = c.EndDateTime,
					TimeToPlayPerSession = c.TimeToPlayPerSession,
					NumberOfPlayer1Time = c.NumberOfPlayer1Time
				}).ToList();
				foreach (var item in models)
				{
					var entity = entities.FirstOrDefault(c => c.EventCampaignID == item.EventCampaignID);
					var startTime = new DateTime(item.StartDateTime.Value.Year, item.StartDateTime.Value.Month, item.StartDateTime.Value.Day, item.StartDateTime.Value.Hour, item.StartDateTime.Value.Minute, 0);
					while (startTime <= item.EndDateTime.Value)
					{
						if (!entity.EventRegisters.Any(c => c.StartDateTime.Value <= startTime && startTime < c.EndDateTime.Value))
						{
							item.TimeAvailableToPlay = startTime;
							break;
						}
						startTime = startTime.AddMinutes(item.TimeToPlayPerSession.Value);
					}
				}
			}
			return models;*/

			List<ApiEventCampaignModel> models = new List<ApiEventCampaignModel>();
			using (IDataContextAsync context = new GameManagerContext())
			using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
			{
				_repository = new Repository<EventCampaign>(context, unitOfWork);
				var currentTime = DateTime.Now;
				var entities = _repository.AllIncluding(c => c.City, c => c.Event, c => c.EventRegisters).Where((c => c.CityID == cityid)).OrderBy(c => c.EventCampaignID).OrderBy(c => c.StartDateTime).ToList();
				models = entities.Select(c => new ApiEventCampaignModel()
				{
					EventCampaignID = c.EventCampaignID,
					
					EventID = c.EventID,
					CityName = c.City.Name,
					CityID = c.CityID,
					StartDateTime = c.StartDateTime,
					EndDateTime = c.EndDateTime,
					TimeToPlayPerSession = c.TimeToPlayPerSession,
					NumberOfPlayer1Time = c.NumberOfPlayer1Time
				}).ToList();
				List<int> statusList = new List<int>() { (int)eEventRegisterStatus.New, (int)eEventRegisterStatus.Reminded };

				IRepositoryAsync<Event> _repository1;
				_repository1 = new Repository<Event>(context, unitOfWork);
				foreach (var item in models)
				{
					var test = _repository1.Find(item.EventID);
					if (test != null)
					{
						item.EventName = test.Name;
						item.ImagePath = test.ImagePath;
					}

				}
				foreach (var item in models)
				{
					item.EventCampaignTimeAvailables = new List<EventCampaignTimeAvailable>();
					var entity = entities.FirstOrDefault(c => c.EventCampaignID == item.EventCampaignID);
					var currentDate = DateTime.Now;
					var startTime = new DateTime(item.StartDateTime.Value.Year, item.StartDateTime.Value.Month, item.StartDateTime.Value.Day, item.StartDateTime.Value.Hour, item.StartDateTime.Value.Minute, 0);
					while (startTime < item.EndDateTime.Value)
					{
						if (startTime > currentDate && (startTime.Hour < 12 || startTime.Hour > 14))
						{

							var eventRegisters = entity.EventRegisters.Where(c => c.StartDateTime == startTime && statusList.Contains(c.Status)).ToList();
							var numOfPlayerAvailable = item.NumberOfPlayer1Time - eventRegisters.Sum(c => c.NumberOfPlayer1Time);

							if (numOfPlayerAvailable > 0 )
							{
								item.EventCampaignTimeAvailables.Add(new EventCampaignTimeAvailable()
								{
									NumberOfPlayer1Time = numOfPlayerAvailable,
									TimeAvailableToPlay = startTime
								});
							}
							else
							{
								item.EventCampaignTimeAvailables.Add(new EventCampaignTimeAvailable()
								{
									NumberOfPlayer1Time = 0,
									TimeAvailableToPlay = startTime
								});
							}
						}
						startTime = startTime.AddMinutes(item.TimeToPlayPerSession.Value);
					}
					if (item.EventCampaignTimeAvailables.Count() > 0)
					{
						item.TimeAvailableToPlay = item.EventCampaignTimeAvailables[0].TimeAvailableToPlay;
						item.NumberOfPlayer1Time = item.EventCampaignTimeAvailables[0].NumberOfPlayer1Time;
					}
				}
			}
			return models;
		}
        public List<ApiEventRegisterUserModel> GetEventRegisterByQRCode(string qrCode)
        {
            using (IDataContextAsync context = new GameManagerContext())
            using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
            {
                _eventRegisterRepo = new Repository<EventRegister>(context, unitOfWork);
				var models = _eventRegisterRepo.Queryable().Where(c => c.AspNetUser.QRCode == qrCode && (c.Status == 0 || c.Status == 2 || c.Status == 3)).OrderByDescending(c => c.StartDateTime).Select(c => new ApiEventRegisterUserModel()
                {
                    EventRegisterID = c.EventRegisterID,
                    StartDateTime = c.StartDateTime,
                    EndDateTime = c.EndDateTime,
                    TimeToPlayPerSession = c.TimeToPlayPerSession,
                    NumberOfPlayer1Time = c.NumberOfPlayer1Time,
                    Active = c.Active,
                    EventCampaignID = c.EventCampaignID.Value,
                    CityName = c.EventCampaign.City.Name,
					CityID = c.EventCampaign.CityID,
					EventID = c.EventCampaign.EventID,
                    Status = c.Status,
					UserId = c.UserId,					
					UserName = c.AspNetUser.FirstName + " " + c.AspNetUser.LastName,
					PlayedDate = c.PlayedDate
                }).ToList();
				IRepositoryAsync<Event> _repository1;
				_repository1 = new Repository<Event>(context, unitOfWork);
				foreach (var item in models)
				{
					var test = _repository1.Find(item.EventID);
					if (test != null)
					{
						item.EventName = test.Name;
						item.ImagePath = test.ImagePath;
					}

				}
                return models;
            }
        }

		public ApiEventCampaignModel GetEventCampaignById(int campaignId)
		{
			List<ApiEventCampaignModel> models = new List<ApiEventCampaignModel>();
			using (IDataContextAsync context = new GameManagerContext())
			using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
			{
				_repository = new Repository<EventCampaign>(context, unitOfWork);
				var currentTime = DateTime.Now;
				var entities = _repository.AllIncluding(c => c.City, c => c.Event, c => c.EventRegisters).Where((c => c.StartDateTime >= currentTime && c.EventCampaignID == campaignId)).OrderBy(c => c.EventCampaignID).OrderBy(c => c.StartDateTime).ToList();
				models = entities.Select(c => new ApiEventCampaignModel()
				{
					EventCampaignID = c.EventCampaignID,
					EventName = c.Event.Name,
					EventID = c.EventID,
					CityName = c.City.Name,
					CityID = c.CityID,
					StartDateTime = c.StartDateTime,
					EndDateTime = c.EndDateTime,
					TimeToPlayPerSession = c.TimeToPlayPerSession,
					NumberOfPlayer1Time = c.NumberOfPlayer1Time
				}).ToList();
				foreach (var item in models)
				{
					var entity = entities.FirstOrDefault(c => c.EventCampaignID == item.EventCampaignID);
					var startTime = new DateTime(item.StartDateTime.Value.Year, item.StartDateTime.Value.Month, item.StartDateTime.Value.Day, item.StartDateTime.Value.Hour, item.StartDateTime.Value.Minute, 0);
					while (startTime <= item.EndDateTime.Value)
					{
						if (!entity.EventRegisters.Any(c => c.StartDateTime <= startTime && startTime < c.EndDateTime))
						{
							item.TimeAvailableToPlay = startTime;
							break;
						}
						startTime = startTime.AddMinutes(item.TimeToPlayPerSession.Value);
					}
				}
			}
			if (models == null || models.Count == 0) return null;
			return models[0];
		}

		
    }
}
