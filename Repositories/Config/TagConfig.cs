using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repositories.Config;

public class TagConfig : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.HasData(
            new Tag() {TagId = 1,TagName = "Haber",Url = "haber"},
            new Tag() {TagId = 2,TagName = "Makale",Url = "makale"},
            new Tag() {TagId = 3,TagName = "Araba",Url = "araba"},
            new Tag() {TagId = 4,TagName = "Dijital",Url = "dijital"}
            );
    }
}