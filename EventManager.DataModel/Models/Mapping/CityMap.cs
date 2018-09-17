using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace EventManager.DataModel.Models.Mapping
{
    public class CityMap : EntityTypeConfiguration<City>
    {
        public CityMap()
        {
            // Primary Key
            this.HasKey(t => t.CityID);

            // Properties
            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(20);

            // Table & Column Mappings
            this.ToTable("City");
            this.Property(t => t.CityID).HasColumnName("CityID");
            this.Property(t => t.Name).HasColumnName("Name");
			this.Property(t => t.EvtHappened).HasColumnName("EvtHappened");
			this.Property(t => t.Position).HasColumnName("Position");
        }
    }
}
