using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace EventManager.Web.Models
{
    // Models used as parameters to AccountController actions.

    public class AddExternalLoginBindingModel
    {
        [Required]
        [Display(Name = "External access token")]
        public string ExternalAccessToken { get; set; }
    }

    public class ChangePasswordBindingModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class RegisterBindingModel
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
		public System.DateTime BirthDate { get; set; }

		public string SignatureImgPath { get; set; }
		public int Status { get;set; }

		public int UserType { get; set; }
	
    }

	public class AccountUpdateBindingModel
	{
		public string Id { get; set; }

		[Required]
		[Display(Name = "Email")]
		public string Email { get; set; }

		
		public string Address { get; set; }
		
		public string FullName { get; set; }
		
		
		
		public int CityId { get; set; }

		public string QRCode { get; set; }

		public string FirstName { get; set; }
		public string LastName { get; set; }
		
		public System.DateTime BirthDate { get; set; }	
	}

    public class RegisterExternalBindingModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class RemoveLoginBindingModel
    {
        [Required]
        [Display(Name = "Login provider")]
        public string LoginProvider { get; set; }

        [Required]
        [Display(Name = "Provider key")]
        public string ProviderKey { get; set; }
    }

    public class SetPasswordBindingModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
