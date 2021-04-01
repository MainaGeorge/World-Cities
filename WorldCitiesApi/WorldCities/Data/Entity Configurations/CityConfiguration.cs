using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorldCities.Data.Entity_Models;

namespace WorldCities.Data.Entity_Configurations
{
    public class CityConfiguration : IEntityTypeConfiguration<City>
    {
        public void Configure(EntityTypeBuilder<City> builder)
        {
            builder.ToTable("cities");
            builder.HasKey(p => p.Id);

            builder.HasOne(p => p.Country)
                .WithMany(p => p.Cities)
                .HasForeignKey(p => p.CountryId);

            builder.Property(p => p.Latitude).HasColumnType("decimal(7,4)");
            builder.Property(p => p.Longitude).HasColumnType("decimal(7,4)");
        }
    }
}
