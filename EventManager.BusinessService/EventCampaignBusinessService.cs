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

using Repository.Pattern.Infrastructure;

namespace EventManager.BusinessService
{
    public interface IEventCampaignBusinessService
    {
        List<ApiEventCampaignModel> GetListAvailable();
        bool IsValidTimeRegister(ApiEventRegisterModel model);
        bool RegisterEvent(ApiEventRegisterModel model);
		List<ApiEventCampaignModel> GetListByCity(int cityid);
        void SendNotificationBeforeOrLateEventTime(int numOfMinute, string _serverKey, string _senderId, string to, string title, string body, int status);
		List<ApiEventRegisterUserModel> GetEventRegisterByQRCode(string qrCode);        
    }

    public class EventCampaignBusinessService : IEventCampaignBusinessService
    {
        private IRepositoryAsync<EventCampaign> _repository;
        private IRepositoryAsync<EventRegister> _eventRegisterRepo;
        private INotificationService iNotificationService;
        public EventCampaignBusinessService()
        {

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
                    EventName = c.Event.Name,
                    CityName = c.City.Name,
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
                        var eventRegisters = entity.EventRegisters.Where(c=>c.StartDateTime == startTime && statusList.Contains(c.Status)).ToList();
                        var numOfPlayerAvailable = item.NumberOfPlayer1Time - eventRegisters.Sum(c => c.NumberOfPlayer1Time);
                        if(numOfPlayerAvailable > 0)
                        {
                            item.TimeAvailableToPlay = startTime;
                            item.NumberOfPlayer1Time = numOfPlayerAvailable;
                            break;
                        }
                        startTime = startTime.AddMinutes(item.TimeToPlayPerSession.Value);
                    }
                }
            }
            return models;
        }
        public bool IsValidTimeRegister(ApiEventRegisterModel model)
        {
            using (IDataContextAsync context = new GameManagerContext())
            using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
            {
                _repository = new Repository<EventCampaign>(context, unitOfWork);
                var entity = _repository.AllIncluding(c => c.EventRegisters).FirstOrDefault(c=>c.EventCampaignID == model.EventCampaignID);
                if(model.StartDateTime < entity.StartDateTime || model.EndDateTime >= entity.EndDateTime)
                {
                    return false;
                }
                else
                {
                    List<int> statusList = new List<int>() { (int)eEventRegisterStatus.New, (int)eEventRegisterStatus.Reminded };
                    var eventRegisters = entity.EventRegisters.Where(c => c.StartDateTime == model.StartDateTime && statusList.Contains(c.Status)).ToList();
                    var numOfPlayerAvailable = entity.NumberOfPlayer1Time - eventRegisters.Sum(c => c.NumberOfPlayer1Time);                    
                    return numOfPlayerAvailable > 0;
                }
            }
        }

        public bool RegisterEvent(ApiEventRegisterModel model)
        {
            var valid = IsValidTimeRegister(model);
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
        public void SendNotificationBeforeOrLateEventTime(int numOfMinute, string _serverKey, string _senderId, string to, string title, string body, int status)
        {
            var currentDate = DateTime.Now;
            var compareDate = currentDate.AddMinutes(numOfMinute);

            Expression<Func<EventRegister, bool>> filter = c => true;
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
                        iNotificationService.NotifyAsync(_serverKey, _senderId, item.AspNetUser.DeviceId, title, body);
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
			List<ApiEventCampaignModel> models = new List<ApiEventCampaignModel>();
			using (IDataContextAsync context = new GameManagerContext())
			using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
			{
				_repository = new Repository<EventCampaign>(context, unitOfWork);
				var currentTime = DateTime.Now;
				var entities = _repository.AllIncluding(c => c.City, c => c.Event, c => c.EventRegisters).Where( (c => c.StartDateTime >= currentTime && c.CityID == cityid)).OrderBy(c => c.EventCampaignID).OrderBy(c => c.StartDateTime).ToList();
				models = entities.Select(c => new ApiEventCampaignModel()
				{
					EventCampaignID = c.EventCampaignID,
					EventName = c.Event.Name,
					CityName = c.City.Name,
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
			return models;
		}
        public List<ApiEventRegisterUserModel> GetEventRegisterByQRCode(string qrCode)
        {
            using (IDataContextAsync context = new GameManagerContext())
            using (IUnitOfWorkAsync unitOfWork = new UnitOfWork(context))
            {
                _eventRegisterRepo = new Repository<EventRegister>(context, unitOfWork);
                var model = _eventRegisterRepo.Queryable().Where(c => c.AspNetUser.QRCode == qrCode).OrderByDescending(c => c.StartDateTime).Select(c => new ApiEventRegisterUserModel()
                {
                    EventRegisterID = c.EventRegisterID,
                    StartDateTime = c.StartDateTime.Value,
                    EndDateTime = c.EndDateTime.Value,
                    TimeToPlayPerSession = c.TimeToPlayPerSession.Value,
                    NumberOfPlayer1Time = c.NumberOfPlayer1Time.Value,
                    Active = c.Active,
                    EventCampaignID = c.EventCampaignID.Value,
                    EventName = c.EventCampaign.Event.Name,
                    CityName = c.EventCampaign.City.Name,
                    Status = c.Status
                }).ToList();
                return model;
            }
        }        
    }
}
