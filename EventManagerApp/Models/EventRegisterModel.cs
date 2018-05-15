using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace EventManager.Web.Models
{
	public class EventRegisterModel
	{
		public int EventRegisterID { get; set; }

		[Required]
		public string UserId { get; set; }
		[Required]
		public Nullable<System.DateTime> StartDateTime { get; set; }
		[Required]
		public Nullable<System.DateTime> EndDateTime { get; set; }
		public Nullable<int> TimeToPlayPerSession { get; set; }
		public Nullable<int> NumberOfPlayer1Time { get; set; }
		public Nullable<bool> Active { get; set; }
		[Required]
		public Nullable<int> EventCampaignID { get; set; }
		public int Status { get; set; }
	}
}