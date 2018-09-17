using Repository.Pattern.Ef6;
using System;
using System.Collections.Generic;

namespace EventManager.DataModel.Models
{
    public partial class AspNetUser : Entity
    {
        public AspNetUser()
        {
            this.AspNetUserClaims = new List<AspNetUserClaim>();
            this.AspNetUserLogins = new List<AspNetUserLogin>();
            this.EventRegisters = new List<EventRegister>();
            this.AspNetRoles = new List<AspNetRole>();
			this.UserGiftRedeems = new List<UserGiftRedeem>();

        }

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
        public int CityId { get; set; }

		public City UserCity { get; set; }
        public string QRCode { get; set; }
        public string Comment { get; set; }
        public string SignatureImgPath { get; set; }

		public int UserType { get; set; }
        public string DeviceId { get; set; }

		
        public virtual ICollection<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual ICollection<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual ICollection<EventRegister> EventRegisters { get; set; }
        public virtual ICollection<AspNetRole> AspNetRoles { get; set; }
		public virtual ICollection<AspNetUserImg> AspNetUserImages { get; set; }
		public virtual ICollection<UserGiftRedeem> UserGiftRedeems { get; set; }

	
    }
}
