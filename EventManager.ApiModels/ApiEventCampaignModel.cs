using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventManager.ApiModels
{
    public class ApiEventCampaignModel
    {
        public int EventCampaignID { get; set; }

		public string EventName { get; set; }

		public int EventID { get; set; }
        public string EventName { get; set; }
        public string CityName { get; set; }
		public Nullable<int> CityID { get; set; }
		public Nullable<DateTime> StartDateTime { get; set; }
		public Nullable<DateTime> EndDateTime { get; set; }
        public Nullable<int> TimeToPlayPerSession { get; set; }
        public Nullable<int> NumberOfPlayer1Time { get; set; }
        public DateTime TimeAvailableToPlay { get; set; }
        public List<EventCampaignTimeAvailable> EventCampaignTimeAvailables { get; set; }
    }
    public class EventCampaignTimeAvailable
    {
        public Nullable<int> NumberOfPlayer1Time { get; set; }
        public DateTime TimeAvailableToPlay { get; set; }
    }
}
