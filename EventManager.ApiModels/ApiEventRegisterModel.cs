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
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public int TimeToPlayPerSession { get; set; }
        public int NumberOfPlayer1Time { get; set; }
        public Nullable<bool> Active { get; set; }
        public int EventCampaignID { get; set; }
        public int Status { get; set; }
    }
    public enum eEventRegisterStatus: int
    {
        New = 0
    }
}
