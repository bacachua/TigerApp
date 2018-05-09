using System;
using System.Collections.Generic;

namespace EventManager.DataModel.Models
{
    public partial class Event
    {
        public Event()
        {
            this.EventCampaigns = new List<EventCampaign>();
        }

        public int EventID { get; set; }
        public Nullable<int> EventCategoryID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Nullable<int> TimeToPlayPerSession { get; set; }
        public Nullable<int> NumberOfPlayer1Time { get; set; }
        public virtual EventCategory EventCategory { get; set; }
        public virtual ICollection<EventCampaign> EventCampaigns { get; set; }
    }
}
