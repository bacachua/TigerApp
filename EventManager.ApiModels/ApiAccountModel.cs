using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace EventManager.ApiModels
{
	public class ApiAccountModel
	{
		public string Id { get; set; }

		[Required]
		[Display(Name = "Email")]
		public string Email { get; set; }

		[Required]
		[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
		[DataType(DataType.Password)]
		[Display(Name = "Password")]
		public string Password { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "Confirm password")]
		[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
		public string ConfirmPassword { get; set; }
		public string Address { get; set; }
		public string City { get; set; }
		public string FullName { get; set; }
		public string State { get; set; }
		public string PostalCode { get; set; }
		public Nullable<bool> EmailConfirmed { get; set; }
		public string PasswordHash { get; set; }
		public string SecurityStamp { get; set; }
		public string PhoneNumber { get; set; }
		public Nullable<bool> PhoneNumberConfirmed { get; set; }
		public Nullable<bool> TwoFactorEnabled { get; set; }
		public Nullable<System.DateTime> LockoutEndDateUtc { get; set; }
		public Nullable<bool> LockoutEnabled { get; set; }
		public Nullable<int> AccessFailedCount { get; set; }
		public string DeviceId { get; set; }
		public int CityId { get; set; }
		public string QRCode { get; set; }
		public string FirstName { get; set; }
		public string MiddleName { get; set; }
		public string LastName { get; set; }
		public string UserName { get; set; }
		public Nullable<System.DateTime> BirthDate { get; set; }
		public int Status { get; set; }
	}

}
