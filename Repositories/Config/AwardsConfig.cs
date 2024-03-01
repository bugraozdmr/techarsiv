using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repositories.Config;

public class AwardsConfig : IEntityTypeConfiguration<Awards>
{
    public void Configure(EntityTypeBuilder<Awards> builder)
    {
        builder.HasData(
            new Awards() { AwardsId = 1, Title = "ilk iki haneli mesaj sayısı", point = 30 },
            new Awards() { AwardsId = 2, Title = "ilk iki haneli konu sayısı", point = 50 },
            new Awards() { AwardsId = 3, Title = "yıl dönümünüz", point = 100 }
        );
    }
}