using System;
using System.Collections.Generic;

namespace EventManager.DataModel.Models
{
    public partial class AspNetRole
    {
        public AspNetRole()
        {
            this.AspNetUsers = new List<AspNetUser>();
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Discriminator { get; set; }
        public virtual ICollection<AspNetUser> AspNetUsers { get; set; }
    }
}
