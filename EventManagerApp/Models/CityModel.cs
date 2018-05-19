using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace EventManager.Web.Models
{
	public class CityModel
	{
		public int CityID { get; set; }
		public string Name { get; set; }
		public string Period 
		{
			
			get
			{
				if (this.StartDate == null || this.EndDate == null)
				{
					return "";
				}
				return ((DateTime)this.StartDate).ToString("dd/MM/yyyy") + " - " + ((DateTime)this.EndDate).ToString("dd/MM/yyyy");
			} 
		}

		public Nullable<DateTime> StartDate { get; set; }
		public Nullable<DateTime> EndDate { get; set; }
	}
}