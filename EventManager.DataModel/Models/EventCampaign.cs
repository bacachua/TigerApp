using System;
using System.Collections.Generic;
using Repository.Pattern.Ef6;

namespace EventManager.DataModel.Models
{
    public partial class EventCampaign : Entity
    {
        public EventCampaign()
        {
            this.EventRegisters = new List<EventRegister>();
        }

        public int EventCampaignID { get; set; }
        public int EventID { get; set; }
        public Nullable<int> CityID { get; set; }
        public DateTime StartDateTime { get; set; }
        public System.DateTime EndDateTime { get; set; }
        public Nullable<int> TimeToPlayPerSession { get; set; }
        public Nullable<int> NumberOfPlayer1Time { get; set; }
        public Nullable<bool> Active { get; set; }
        public virtual City City { get; set; }
        public virtual Event Event { get; set; }
		public virtual int Point { get; set; }
        public virtual ICollection<EventRegister> EventRegisters { get; set; }
    }
}
