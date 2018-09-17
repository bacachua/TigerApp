using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;

namespace EventManager.DataModel.Models
{
    public partial class City : Entity
    {
        public City()
        {
            this.EventCampaigns = new List<EventCampaign>();
			this.AspNetUsers =  new List<AspNetUser>();
        }

        public int CityID { get; set; }
        public string Name { get; set; }
		public int Position { get; set; }
		public bool EvtHappened { get; set; }
        public virtual ICollection<EventCampaign> EventCampaigns { get; set; }

		public virtual ICollection<AspNetUser> AspNetUsers { get; set; }
    }
}
