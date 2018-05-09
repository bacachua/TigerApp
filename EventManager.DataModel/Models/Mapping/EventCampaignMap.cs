using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace EventManager.DataModel.Models.Mapping
{
    public class EventCampaignMap : EntityTypeConfiguration<EventCampaign>
    {
        public EventCampaignMap()
        {
            // Primary Key
            this.HasKey(t => t.EventCampaignID);

            // Properties
            // Table & Column Mappings
            this.ToTable("EventCampaign");
            this.Property(t => t.EventCampaignID).HasColumnName("EventCampaignID");
            this.Property(t => t.EventID).HasColumnName("EventID");
            this.Property(t => t.CityID).HasColumnName("CityID");
            this.Property(t => t.StartDateTime).HasColumnName("StartDateTime");
            this.Property(t => t.EndDateTime).HasColumnName("EndDateTime");
            this.Property(t => t.TimeToPlayPerSession).HasColumnName("TimeToPlayPerSession");
            this.Property(t => t.NumberOfPlayer1Time).HasColumnName("NumberOfPlayer1Time");
            this.Property(t => t.Active).HasColumnName("Active");

            // Relationships
            this.HasOptional(t => t.City)
                .WithMany(t => t.EventCampaigns)
                .HasForeignKey(d => d.CityID);
            this.HasOptional(t => t.Event)
                .WithMany(t => t.EventCampaigns)
                .HasForeignKey(d => d.CityID);

        }
    }
}
