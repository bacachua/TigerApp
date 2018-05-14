using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;

namespace EventManager.DataModel.Models
{
	public partial class EventRegister : Entity
    {
        public int EventRegisterID { get; set; }
        public string UserId { get; set; }
        public Nullable<System.DateTime> StartDateTime { get; set; }
        public Nullable<System.DateTime> EndDateTime { get; set; }
        public Nullable<int> TimeToPlayPerSession { get; set; }
        public Nullable<int> NumberOfPlayer1Time { get; set; }
        public Nullable<bool> Active { get; set; }
        public Nullable<int> EventCampaignID { get; set; }
        public virtual AspNetUser AspNetUser { get; set; }
        public virtual EventCampaign EventCampaign { get; set; }

		public int Status { get; set; }
    }
}
