using System;
public class EventRegisterModel
	{
		public int EventRegisterID { get; set; }
		public string UserId { get; set; }
		public Nullable<DateTime> StartDateTime { get; set; }
		public Nullable<DateTime> EndDateTime { get; set; }
		public int TimeToPlayPerSession { get; set; }
		public Nullable<int> NumberOfPlayer1Time { get; set; }
		public Nullable<bool> Active { get; set; }
		public Nullable<int> EventCampaignID { get; set; }
		public int Status { get; set; }

		public string EventName {get;set;}
		public string ImagePath { get; set; }

		public int EventID { get; set; }
	}

	public class EventRegisterStatusModel
	{
		public int EventRegisterID { get; set; }
	}                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                               