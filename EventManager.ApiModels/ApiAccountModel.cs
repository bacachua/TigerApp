using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventManager.ApiModels
{
	public class ApiAccountModel
	{
		public string Id { get; set; }
		public string Address { get; set; }
		public string City { get; set; }
		public string State { get; set; }
		public string PostalCode { get; set; }
		public string Email { get; set; }
		public Nullable<bool> EmailConfirmed { get; set; }
		public string PasswordHash { get; set; }
		public string SecurityStamp { get; set; }
		public string PhoneNumber { get; set; }
		public Nullable<bool> PhoneNumberConfirmed { get; set; }
		public Nullable<bool> TwoFactorEnabled { get; set; }
		public Nullable<System.DateTime> LockoutEndDateUtc { get; set; }
		public Nullable<bool> LockoutEnabled { get; set; }
		public Nullable<int> AccessFailedCount { get; set; }
		public string UserName { get; set; }
		public string FirstName { get; set; }
		public string MiddleName { get; set; }
		public string LastName { get; set; }
		public string FullName { get; set; }
		public Nullable<System.DateTime> LastLoginDate { get; set; }
		public Nullable<System.DateTime> LastPasswordChangedDate { get; set; }
		public Nullable<int> LoginCount { get; set; }
		public string Gender { get; set; }
		public string IdentityNumber { get; set; }
		public Nullable<System.DateTime> BirthDate { get; set; }
		public Nullable<int> CityId { get; set; }
		public string QRCode { get; set; }
		public string Comment { get; set; }

		public string DeviceId { get; set; }
	}
}
