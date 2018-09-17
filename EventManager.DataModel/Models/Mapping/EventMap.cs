using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace EventManager.DataModel.Models.Mapping
{
    public class EventMap : EntityTypeConfiguration<Event>
    {
        public EventMap()
        {
            // Primary Key
            this.HasKey(t => t.EventID);

            // Properties
            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.Description)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("Event");
            this.Property(t => t.EventID).HasColumnName("EventID");
            this.Property(t => t.EventCategoryID).HasColumnName("EventCategoryID");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Description).HasColumnName("Description");
            this.Property(t => t.TimeToPlayPerSession).HasColumnName("TimeToPlayPerSession");
            this.Property(t => t.NumberOfPlayer1Time).HasColumnName("NumberOfPlayer1Time");
			this.Property(t => t.ImagePath).HasColumnName("ImagePath");
			

            // Relationships
            this.HasOptional(t => t.EventCategory)
                .WithMany(t => t.Events)
                .HasForeignKey(d => d.EventCategoryID);

        }
    }
}
