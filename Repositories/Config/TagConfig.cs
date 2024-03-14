using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repositories.Config;

public class TagConfig : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.HasData(
            new Tag() {TagId = 1,TagName = "Oyun haberleri",Url = "oyun-haber"},
            new Tag() {TagId = 2,TagName = "Sinema haberleri",Url = "sinema-haber"},
            new Tag() {TagId = 3,TagName = "Vasıtalar",Url = "vasitalar"},
            new Tag() {TagId = 4,TagName = "Telefon",Url = "telefon"},
            new Tag() {TagId = 5,TagName = "Teknoloji",Url = "teknoloji"},
            new Tag() {TagId = 6,TagName = "Tavsiyeler",Url = "tavsiyeler"},
            new Tag() {TagId = 7,TagName = "Sistem",Url = "sistem"},
            new Tag() {TagId = 8,TagName = "haber",Url = "haber"}
            
            );
    }
}