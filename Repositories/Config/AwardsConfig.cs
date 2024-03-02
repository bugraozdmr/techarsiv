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
            new Awards() { AwardsId = 3, Title = "yıl dönümünüz", point = 100 },
            new Awards() { AwardsId = 4, Title = "bir hafta", point = 7 },
            new Awards() { AwardsId = 5, Title = "bir ay", point = 30 },
            new Awards() { AwardsId = 6, Title = "50 mesaj", point = 50 },
            new Awards() { AwardsId = 7, Title = "100 mesaj", point = 101 },
            new Awards() { AwardsId = 8, Title = "500 mesaj", point = 500 },
            new Awards() { AwardsId = 9, Title = "50 konu", point = 100 }
        );
    }
}