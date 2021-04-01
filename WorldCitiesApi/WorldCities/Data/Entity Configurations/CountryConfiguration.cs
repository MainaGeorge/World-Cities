using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorldCities.Data.Entity_Models;

namespace WorldCities.Data.Entity_Configurations
{
    public class CountryConfiguration : IEntityTypeConfiguration<Country>
    {
        public void Configure(EntityTypeBuilder<Country> builder)
        {
            builder.ToTable("countries");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).IsRequired();
        }
    }
}
