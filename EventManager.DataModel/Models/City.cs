using System;
using System.Collections.Generic;

namespace EventManager.DataModel.Models
{
    public partial class City
    {
        public City()
        {
            this.EventCampaigns = new List<EventCampaign>();
        }

        public int CityID { get; set; }
        public string Name { get; set; }
        public virtual ICollection<EventCampaign> EventCampaigns { get; set; }
    }
}
