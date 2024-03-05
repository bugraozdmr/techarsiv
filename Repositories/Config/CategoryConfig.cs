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
        
        // ["Mobil", "Laptop", "Alışveriş", "Hobiler", "Taşıtlar", "Günlük Yaşam", "Oyunlar", "Yazılım", "Donanım", "Diğer"]
        
        
        builder.HasData(
            // mobil
            new Category() { CategoryId = 1, CategoryName = "iPhone" ,Description = "iPhone cihazları hakkında oluşturulmuş konular",CategoryUrl = "iphone",CommonFilter = 1},
            new Category() { CategoryId = 2, CategoryName = "Android" ,Description = "Android cihazlar hakkında oluşturulmuş konular",CategoryUrl = "android",CommonFilter = 1},
            new Category() { CategoryId = 3, CategoryName = "Diğer telefonlar" ,Description = "Diğer telefon modelleri hakkında oluşturulmuş konular",CategoryUrl = "diger-telefonlar",CommonFilter = 1},
            new Category() { CategoryId = 4, CategoryName = "Akıllı saatler" ,Description = "Akıllı saat önerileri ve incelemeleri",CategoryUrl = "akilli-saatler",CommonFilter = 1},
            new Category() { CategoryId = 5, CategoryName = "Kulaklıklar" ,Description = "Kulaklık tavsiyeleri ve incelemeleri",CategoryUrl = "kulakliklar",CommonFilter = 1},
            new Category() { CategoryId = 6, CategoryName = "Telefon tavsiyeleri" ,Description = "Telefon tavsiye ve önerileri",CategoryUrl = "telefon-tavsiyeleri",CommonFilter = 1},
            new Category() { CategoryId = 7, CategoryName = "iPad" ,Description = "iPad cihazları hakkındaki konular",CategoryUrl = "ipad",CommonFilter = 1},
            new Category() { CategoryId = 8, CategoryName = "Diğer tabletler" ,Description = "Diğer tablet cihazları hakkındaki konular",CategoryUrl = "diger-tabletler",CommonFilter = 1},
            
            
            // laptop
            new Category() { CategoryId = 9, CategoryName = "Macbook" ,Description = "Macbooklar hakkında konular",CategoryUrl = "macbook",CommonFilter = 2},
            new Category() { CategoryId = 10, CategoryName = "Oyuncu bilgisayarları" ,Description = "Oyun dünyasında sizleri üzmeyecek canavarlar",CategoryUrl = "oyuncu-bilgisayarlari",CommonFilter = 2},
            new Category() { CategoryId = 11, CategoryName = "Diğer laptoplar" ,Description = "Diğer laptop cihazları hakkında konular",CategoryUrl = "diger-laptoplar",CommonFilter = 2},
            
            
            // alisveris
            new Category() { CategoryId = 12, CategoryName = "Karşılaştırma" ,Description = "Ürünler arasında karşılaştırma konuları",CategoryUrl = "karsilastirma",CommonFilter = 3},
            new Category() { CategoryId = 13, CategoryName = "Web'den alışveriş" ,Description = "Web'den alışveriş önerileri",CategoryUrl = "webden-alisveris",CommonFilter = 3},
            
            
            // hobiler
            new Category() { CategoryId = 14, CategoryName = "Evcil hayvanlar" ,Description = "Evcil hayvan bakım ve yapılması gerekenler hakkında konular",CategoryUrl = "evcil-hayvanlar",CommonFilter = 4},
            new Category() { CategoryId = 15, CategoryName = "Yemek" ,Description = "Yemek tarifleri ve önerileri ile ilgili konular",CategoryUrl = "yemek",CommonFilter = 4},
            
            
            
            // tasitlar
            new Category() { CategoryId = 16, CategoryName = "Otomobil" ,Description = "Otomobil önerileri ve incelemeleri hakkındaki konular",CategoryUrl = "otomobil",CommonFilter = 5},
            new Category() { CategoryId = 17, CategoryName = "Motosiklet" ,Description = "Motosiklet önerileri ve incelemeleri hakkındaki konular",CategoryUrl = "motosiklet",CommonFilter = 5},
            new Category() { CategoryId = 18, CategoryName = "elektrikli scooter" ,Description = "elektrikli scooter önerileri ve incelemeleri hakkındaki konular",CategoryUrl = "elektrikli-scooter",CommonFilter = 5},
            new Category() { CategoryId = 19, CategoryName = "Bisiklet" ,Description = "Bisiklet ve incelemeleri hakkındaki konular",CategoryUrl = "bisiklet",CommonFilter = 5},
            new Category() { CategoryId = 20, CategoryName = "Diğer araçlar" ,Description = "Diğer araçlar hakkında öneriler ve incelemeler",CategoryUrl = "diger-araclar",CommonFilter = 5},
            
            
            // günlük yasam
            new Category() { CategoryId = 21, CategoryName = "Televizyonlar" ,Description = "Televizyonlar hakkında açılmış konular",CategoryUrl = "televizyonlar",CommonFilter = 6},
            new Category() { CategoryId = 22, CategoryName = "Ses sistemleri" ,Description = "Ses sistemleri hakkında açılmış konular",CategoryUrl = "ses-sistemleri",CommonFilter = 6},
            new Category() { CategoryId = 23, CategoryName = "Klimalar" ,Description = "Klimalar hakkında açılmış konular",CategoryUrl = "klimalar",CommonFilter = 6},
            new Category() { CategoryId = 24, CategoryName = "Ev ürünleri" ,Description = "Ev ürünleri hakkında açılmış konular",CategoryUrl = "ev-urunleri",CommonFilter = 6},
            new Category() { CategoryId = 25, CategoryName = "Mobilyalar" ,Description = "Mobilyalar hakkında açılmış konular",CategoryUrl = "mobilyalar",CommonFilter = 6},
            new Category() { CategoryId = 26, CategoryName = "Diğer günlük yaşam ürünleri" ,Description = "Diğer günlük yaşam ürünleri hakkında açılmış konular",CategoryUrl = "diger-gunluk-yasam-urunleri",CommonFilter = 6},
                
            //oyunlar
            new Category() { CategoryId = 27, CategoryName = "Mobil oyunlar" ,Description = "Mobil oyunlar hakkında açılmış konular",CategoryUrl = "mobil-oyunlar",CommonFilter = 7},
            new Category() { CategoryId = 28, CategoryName = "Bilgisayar oyunları" ,Description = "Bilgisayar oyunları hakkında açılmış konular",CategoryUrl = "bilgisayar-oyunlari",CommonFilter = 7},
            new Category() { CategoryId = 29, CategoryName = "Konsol oyunları" ,Description = "Konsol oyunları hakkında açılmış konular",CategoryUrl = "konsol-oyunlari",CommonFilter = 7},
            new Category() { CategoryId = 30, CategoryName = "Oyun tavsiyeleri" ,Description = "Oyun tavsiyeleri hakkında oluşturulmuş konular",CategoryUrl = "oyun-tavsiyeleri",CommonFilter = 7},
            new Category() { CategoryId = 31, CategoryName = "Online oyunlar" ,Description = "Çevrimiçi oyunlar hakkında haberler",CategoryUrl = "online-oyunlar",CommonFilter = 7},
            new Category() { CategoryId = 32, CategoryName = "Oyun haberleri" ,Description = "Oyunlar hakkındaki haberler",CategoryUrl = "oyun-haberleri",CommonFilter = 7},
            new Category() { CategoryId = 33, CategoryName = "Oyun incelemeleri" ,Description = "Kullanıcıların oyun incelemeleri",CategoryUrl = "oyun-incelemeleri",CommonFilter = 7},
            new Category() { CategoryId = 34, CategoryName = "Dijital oyun servisleri" ,Description = "Oyun servisleri yardım ve önerileri",CategoryUrl = "dijital-oyun-servisleri",CommonFilter = 7},
            
            // yazılım
            new Category() { CategoryId = 35, CategoryName = "Programlama" ,Description = "Programlama hakkında öneri ve tavsiyeler",CategoryUrl = "programlama",CommonFilter = 8},
            new Category() { CategoryId = 36, CategoryName = "Geliştirme ortamları" ,Description = "Kullanıcı dostu geliştirme ortamları ve önerileri",CategoryUrl = "gelistirme-ortamlari",CommonFilter = 8},
            new Category() { CategoryId = 37, CategoryName = "Yapay zeka" ,Description = "Yapay zeka araçları",CategoryUrl = "yapay-zeka",CommonFilter = 8},
            new Category() { CategoryId = 38, CategoryName = "Diğer yazılımlar" ,Description = "Diğer yazılımlar",CategoryUrl = "diger-yazilimlar",CommonFilter = 8},
            
            // donanim
            new Category() { CategoryId = 39, CategoryName = "Teknik destek" ,Description = "Donanım teknik destek konuları",CategoryUrl = "teknik-destek",CommonFilter = 9},
            new Category() { CategoryId = 40, CategoryName = "Diğer donanımlar" ,Description = "Diğer donanımlar",CategoryUrl = "diger-donanimlar",CommonFilter = 9},
            new Category() { CategoryId = 41, CategoryName = "Monitörler" ,Description = "Monitör tercih ve önerileri",CategoryUrl = "monitor",CommonFilter = 9},
            new Category() { CategoryId = 42, CategoryName = "Bilgisayar Toplama" ,Description = "Bilgisayar toplamaya dair açılmış konular",CategoryUrl = "bilgisayar-toplama",CommonFilter = 9},
            new Category() { CategoryId = 43, CategoryName = "Oyun ekipmanları" ,Description = "Oyuncu ekipmanları tavsiyeleri konuları",CategoryUrl = "oyun-ekipmanları",CommonFilter = 9},
            
            
            // diger
            new Category() { CategoryId = 44, CategoryName = "Okul öneri ve tavsiyeleri" ,Description = "Kullnıcılar tarafından okul öneri ve tavsiyeleri",CategoryUrl = "okul-tavsiyeleri",CommonFilter = 10},
            new Category() { CategoryId = 45, CategoryName = "Diğer" ,Description = "Diğer",CategoryUrl = "diger",CommonFilter = 10}
        );
    }
}