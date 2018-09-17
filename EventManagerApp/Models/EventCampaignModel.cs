using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EventManager.Web.Models
{
	public class EventCampaignModel
	{
		public EventCampaignModel()
        {
            
        }

        public int EventCampaignID { get; set; }
        public Nullable<int> EventID { get; set; }
		public string EventName { get; set; }
		public string ImagePath { get; set; }
        public Nullable<int> CityID { get; set; }
        public Nullable<System.DateTime> StartDateTime { get; set; }
        public Nullable<System.DateTime> EndDateTime { get; set; }
        public Nullable<int> TimeToPlayPerSession { get; set; }
        public Nullable<int> NumberOfPlayer1Time { get; set; }
		public Nullable<bool> Active { get; set; }		
		public int WaitingTime { get; set; }
		public string CityName { get; set; }
		public DateTime TimeToPlay { get; set; }
	}
}