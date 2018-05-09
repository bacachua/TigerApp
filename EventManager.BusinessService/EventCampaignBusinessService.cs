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

namespace EventManager.BusinessService
{
	public interface IEventCampaignBusinessService : IService<EventCampaign>
	{
		
	}

	public class EventCampaignBusinessService : Service<EventCampaign>, IEventCampaignBusinessService
	{
		private readonly IRepositoryAsync<EventCampaign> _repository;
		public EventCampaignBusinessService(IRepositoryAsync<EventCampaign> repository)
			: base(repository)
		{
			_repository = repository;
		}

	
	}
}
