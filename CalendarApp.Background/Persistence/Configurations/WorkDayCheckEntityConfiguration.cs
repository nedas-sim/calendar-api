using CalendarApp.Background.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CalendarApp.Background.Persistence.Configurations;

public class WorkDayCheckEntityConfiguration
    : IEntityTypeConfiguration<WorkDayCheck>
{
    public void Configure(EntityTypeBuilder<WorkDayCheck> builder)
    {
        builder.HasIndex(x => new
        {
            x.Date,
            x.CountryCode,
        }).IsUnique();

        builder.HasKey(x => new
        {
            x.Date,
            x.CountryCode,
        });
        
        builder.Property(x => x.Date).IsRequired();
        builder.Property(x => x.CountryCode).IsRequired().HasMaxLength(3);
        builder.Property(x => x.IsWorkDay).IsRequired();
    }
}
