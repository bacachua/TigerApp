using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace EventManager.DataModel.Models.Mapping
{
    public class EventRegisterMap : EntityTypeConfiguration<EventRegister>
    {
        public EventRegisterMap()
        {
            // Primary Key
            this.HasKey(t => t.EventRegisterID);

            // Properties
            this.Property(t => t.UserId)
                .IsRequired()
                .HasMaxLength(128);

            // Table & Column Mappings
            this.ToTable("EventRegister");
            this.Property(t => t.EventRegisterID).HasColumnName("EventRegisterID");
            this.Property(t => t.UserId).HasColumnName("UserId");
            this.Property(t => t.StartDateTime).HasColumnName("StartDateTime");
            this.Property(t => t.EndDateTime).HasColumnName("EndDateTime");
            this.Property(t => t.TimeToPlayPerSession).HasColumnName("TimeToPlayPerSession");
            this.Property(t => t.NumberOfPlayer1Time).HasColumnName("NumberOfPlayer1Time");
            this.Property(t => t.Active).HasColumnName("Active");
            this.Property(t => t.EventCampaignID).HasColumnName("EventCampaignID");
			this.Property(t => t.Status).HasColumnName("Status");
			this.Property(t => t.PlayedDate).HasColumnName("PlayedDate");
            // Relationships
            this.HasRequired(t => t.AspNetUser)
                .WithMany(t => t.EventRegisters)
                .HasForeignKey(d => d.UserId);
            this.HasOptional(t => t.EventCampaign)
                .WithMany(t => t.EventRegisters)
                .HasForeignKey(d => d.EventCampaignID);

        }
    }
}
