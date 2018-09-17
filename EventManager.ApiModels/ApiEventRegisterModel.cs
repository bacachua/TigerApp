using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventManager.ApiModels
{
    public class ApiEventRegisterModel
    {
        public int EventRegisterID { get; set; }
        public string UserId { get; set; }
		public string UserName { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public int TimeToPlayPerSession { get; set; }
        public int NumberOfPlayer1Time { get; set; }
        public Nullable<bool> Active { get; set; }
        public int EventCampaignID { get; set; }
        public int Status { get; set; }
		public DateTime PlayedDate { get; set; }
		
    }
    public class ApiEventRegisterUserModel: ApiEventRegisterModel
    {
        public string EventName { get; set; }
		public int EventID { get; set; }
        public string CityName { get; set; }
		public string ImagePath { get; set; }
		public int? CityID { get; set; }
        public string StatusName { get { return Enum.GetName(typeof(eEventRegisterStatus), Status); } }
    }
    public enum eEventRegisterStatus: int
    {
        New = 0,
        Cancelled,
        Reminded,
        Late,
        Played
    }
}
