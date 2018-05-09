using System;
using System.Collections.Generic;

namespace EventManager.DataModel.Models
{
    public partial class EventCategory
    {
        public EventCategory()
        {
            this.Events = new List<Event>();
        }

        public int EventCategoryID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual ICollection<Event> Events { get; set; }
    }
}
