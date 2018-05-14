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

namespace EventManager.BusinessService
{
    public interface IEventCampaignBusinessService
    {
        List<ApiEventCampaignModel> GetListAvailable();
    }

    public class EventCampaignBusinessService : IEventCampaignBusinessService
    {
        private IRepositoryAsync<EventCampaign> _repository;
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
                var entities = _repository.AllIncluding(c => c.City, c => c.Event, c => c.EventRegisters).Where(c => c.StartDateTime >= currentTime).OrderBy(c => c.EventCampaignID).OrderBy(c => c.StartDateTime).ToList();
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

    }
}
