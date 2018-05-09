using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace EventManager.DataModel.Models.Mapping
{
    public class EventCategoryMap : EntityTypeConfiguration<EventCategory>
    {
        public EventCategoryMap()
        {
            // Primary Key
            this.HasKey(t => t.EventCategoryID);

            // Properties
            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(20);

            this.Property(t => t.Description)
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("EventCategory");
            this.Property(t => t.EventCategoryID).HasColumnName("EventCategoryID");
            this.Property(t => t.Name).HasColumnName("Name");
            this.Property(t => t.Description).HasColumnName("Description");
        }
    }
}
