using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Repositories.Config;

public class CategoryConfig :  IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(p => p.CategoryId);
        builder.Property(p => p.CategoryName).IsRequired();
        
        
        builder.HasData(
            new Category() { CategoryId = 1, CategoryName = "Teknoloji" ,Description = "Teknoloji ile ilgili olan konular.",Icon = "fa fa-desktop",CategoryUrl = "teknoloji"},
            new Category() { CategoryId = 2, CategoryName = "Günlük yaşam" ,Description = "Günlük hayatta popüler olmuş ya da direkt günlük hayatla ilişkili konular.",Icon = "fa fa-newspaper-o",CategoryUrl = "gunluk_yasam"},
            new Category() { CategoryId = 3, CategoryName = "Diğer" ,Description = "Diğer konularla ilgili olmayan ya da şu anlık mevcut olmayan konular.",Icon = "fa fa-font-awesome",CategoryUrl = "diger"}
        );
    }
}