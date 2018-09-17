
using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;

namespace EventManager.DataModel.Models
{
	public partial class EventRegister : Entity
    {
        public int EventRegisterID { get; set; }
        public string UserId { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public int TimeToPlayPerSession { get; set; }
        public int NumberOfPlayer1Time { get; set; }
        public Nullable<bool> Active { get; set; }
        public Nullable<int> EventCampaignID { get; set; }
        public virtual AspNetUser AspNetUser { get; set; }
        public virtual EventCampaign EventCampaign { get; set; }
		public virtual int Point { get; set; }
		public int Status { get; set; }

		public string Rewarded { get; set; }
		public DateTime PlayedDate { get; set; }
		
    }
}
