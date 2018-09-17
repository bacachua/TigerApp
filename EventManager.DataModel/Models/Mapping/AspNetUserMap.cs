using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace EventManager.DataModel.Models.Mapping
{
    public class AspNetUserMap : EntityTypeConfiguration<AspNetUser>
    {
        public AspNetUserMap()
        {
            // Primary Key
            this.HasKey(t => t.Id);

            // Properties
            this.Property(t => t.Id)
                .IsRequired()
                .HasMaxLength(128);

            this.Property(t => t.Address)
                .HasMaxLength(128);

            this.Property(t => t.City)
                .HasMaxLength(80);

            this.Property(t => t.State)
                .HasMaxLength(70);

            this.Property(t => t.PostalCode)
                .HasMaxLength(15);

            this.Property(t => t.Email)
                .HasMaxLength(256);

            this.Property(t => t.PasswordHash)
                .HasMaxLength(128);

            this.Property(t => t.SecurityStamp)
                .HasMaxLength(100);

            this.Property(t => t.PhoneNumber)
                .HasMaxLength(50);

            this.Property(t => t.UserName)
                .HasMaxLength(256);

            this.Property(t => t.FirstName)
                .HasMaxLength(100);

            this.Property(t => t.MiddleName)
                .HasMaxLength(50);

            this.Property(t => t.LastName)
                .HasMaxLength(70);

            this.Property(t => t.FullName)
                .HasMaxLength(255);

            this.Property(t => t.Gender)
                .HasMaxLength(1);

            this.Property(t => t.IdentityNumber)
                .HasMaxLength(15);

            this.Property(t => t.QRCode)
                .HasMaxLength(50);

            this.Property(t => t.Comment)
                .HasMaxLength(100);

            // Table & Column Mappings
            this.ToTable("AspNetUsers");
            this.Property(t => t.Id).HasColumnName("Id");
            this.Property(t => t.Address).HasColumnName("Address");
            this.Property(t => t.City).HasColumnName("City");
            this.Property(t => t.State).HasColumnName("State");
            this.Property(t => t.PostalCode).HasColumnName("PostalCode");
            this.Property(t => t.Email).HasColumnName("Email");
            this.Property(t => t.EmailConfirmed).HasColumnName("EmailConfirmed");
            this.Property(t => t.PasswordHash).HasColumnName("PasswordHash");
            this.Property(t => t.SecurityStamp).HasColumnName("SecurityStamp");
            this.Property(t => t.PhoneNumber).HasColumnName("PhoneNumber");
            this.Property(t => t.PhoneNumberConfirmed).HasColumnName("PhoneNumberConfirmed");
            this.Property(t => t.TwoFactorEnabled).HasColumnName("TwoFactorEnabled");
            this.Property(t => t.LockoutEndDateUtc).HasColumnName("LockoutEndDateUtc");
            this.Property(t => t.LockoutEnabled).HasColumnName("LockoutEnabled");
            this.Property(t => t.AccessFailedCount).HasColumnName("AccessFailedCount");
            this.Property(t => t.UserName).HasColumnName("UserName");
            this.Property(t => t.FirstName).HasColumnName("FirstName");
            this.Property(t => t.MiddleName).HasColumnName("MiddleName");
            this.Property(t => t.LastName).HasColumnName("LastName");
            this.Property(t => t.FullName).HasColumnName("FullName");
            this.Property(t => t.LastLoginDate).HasColumnName("LastLoginDate");
            this.Property(t => t.LastPasswordChangedDate).HasColumnName("LastPasswordChangedDate");
            this.Property(t => t.LoginCount).HasColumnName("LoginCount");
            this.Property(t => t.Gender).HasColumnName("Gender");
            this.Property(t => t.IdentityNumber).HasColumnName("IdentityNumber");
            this.Property(t => t.BirthDate).HasColumnName("BirthDate");
            this.Property(t => t.CityId).HasColumnName("CityId");
            this.Property(t => t.QRCode).HasColumnName("QRCode");
            this.Property(t => t.Comment).HasColumnName("Comment");
            this.Property(t => t.SignatureImgPath).HasColumnName("SignatureImgPath");
			this.Property(t => t.UserType).HasColumnName("UserType");

            // Relationships
            this.HasMany(t => t.AspNetRoles)
                .WithMany(t => t.AspNetUsers)
                .Map(m =>
                    {
                        m.ToTable("AspNetUserRoles");
                        m.MapLeftKey("UserId");
                        m.MapRightKey("RoleId");
                    });
			this.HasRequired(t => t.UserCity)
			  .WithMany(t => t.AspNetUsers)
			  .HasForeignKey(d => d.CityId);

        }
    }
}
