using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventManager.DataModel.Models;
using Repository.Pattern.Repositories;

namespace EventManager.Repository.Repositories
{
	public  static class EventCampaignRepository
	{
		public static IEnumerable<EventCampaign> GetEventCampaignList(this IRepositoryAsync<EventCampaign> repository, int page, int pageSize, out int cnt)
		{
			return repository.
					Query(x => x.CityID >=0)
											.OrderBy(x => x
											.OrderBy(y => y.Event.Name)).SelectPage(page, pageSize, out cnt).AsEnumerable();

		}

		public static IEnumerable<EventCampaign> GetApartmentsList(this IRepositoryAsync<EventCampaign> repository, int cityId, int page, int pageSize, out int cnt)
		{
			return repository.
					Query(x => x.CityID == cityId)
											.OrderBy(x => x
											.OrderBy(y => y.Event.Name)).SelectPage(page, pageSize, out cnt).AsEnumerable();


		}
	}
}
