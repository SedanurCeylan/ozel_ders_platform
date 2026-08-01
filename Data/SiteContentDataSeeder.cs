using Microsoft.EntityFrameworkCore;
using OzelDersYonetim.Models.Content;

namespace OzelDersYonetim.Data;

public sealed class SiteContentDataSeeder(ApplicationDbContext dbContext)
{
    public async Task SeedAsync()
    {
        if (!await dbContext.SiteSettings.AnyAsync())
        {
            dbContext.SiteSettings.Add(new SiteSetting
            {
                SiteName = "Matematik Atölyesi",
                TeacherName = "Sena Öğretmen",
                Email = "ogretmen@example.com",
                HeroTitle = "Matematik ezber değil, anlama yolculuğudur.",
                HeroDescription = "Her öğrencinin hızına, hedeflerine ve öğrenme biçimine göre şekillenen derslerle matematiği birlikte anlaşılır hâle getiriyoruz.",
                AboutTitle = "Her öğrenci matematiği anlayabilir.",
                AboutDescription = "Doğru anlatım, sabır ve kişiye özel bir yol haritasıyla matematik; kaygı kaynağı olmaktan çıkıp güçlü bir düşünme aracına dönüşür."
            });
        }

        if (!await dbContext.ContentSections.AnyAsync())
        {
            dbContext.ContentSections.AddRange(
                new ContentSection
                {
                    PageKey = "Trainings",
                    SectionKey = "personal-plan",
                    Title = "Kişiye Özel Ders Planı",
                    Subtitle = "Her öğrenci için ayrı rota",
                    Content = "Ders sıklığı, konu sırası ve çalışma kaynakları öğrencinin seviyesine ve hedeflerine göre belirlenir.",
                    DisplayOrder = 1
                },
                new ContentSection
                {
                    PageKey = "Trainings",
                    SectionKey = "progress-feedback",
                    Title = "Düzenli Geri Bildirim",
                    Subtitle = "Gelişim görünür olsun",
                    Content = "Ders, ödev ve deneme sonuçları düzenli değerlendirilerek öğrenci ve veliyle paylaşılır.",
                    DisplayOrder = 2
                });
        }

        var defaults = new[]
        {
            new ContentSection{PageKey="Home",SectionKey="benefit-roadmap",Title="Kişisel Yol Haritası",Subtitle="Her öğrenciye ayrı rota",Content="Seviyeyi, güçlü yanları ve eksikleri belirleyerek kişiye özel çalışma planı oluştururuz.",DisplayOrder=1},
            new ContentSection{PageKey="Home",SectionKey="benefit-understanding",Title="Anlayarak Öğrenme",Subtitle="Ezber yerine matematiksel düşünme",Content="Formüllerin arkasındaki mantığı keşfeder, bilgiyi farklı sorulara taşıyabilir hâle getiririz.",DisplayOrder=2},
            new ContentSection{PageKey="Home",SectionKey="benefit-progress",Title="Düzenli Gelişim",Subtitle="İlerleme görünür olsun",Content="Ders, ödev ve deneme sonuçlarını düzenli takip ederek gelişimi somut verilerle değerlendiririz.",DisplayOrder=3},
            new ContentSection{PageKey="Home",SectionKey="program-middle",Title="Ortaokul Matematiği",Subtitle="5–8. Sınıf",Content="Konu temellerini güçlendiren, yeni nesil soru becerisi ve düzenli çalışma alışkanlığı kazandıran program.",DisplayOrder=10},
            new ContentSection{PageKey="Home",SectionKey="program-lgs",Title="LGS Matematik",Subtitle="8. Sınıf · LGS",Content="Kazanım analizi, süre yönetimi, deneme takibi ve yeni nesil sorularla hedefe dönük hazırlık.",DisplayOrder=11},
            new ContentSection{PageKey="Home",SectionKey="program-support",Title="Okul Destek Programı",Subtitle="5–8. Sınıf",Content="Okul konularını sağlamlaştıran, yazılı başarısını ve matematik özgüvenini destekleyen birebir çalışma.",DisplayOrder=12},
            new ContentSection{PageKey="Home",SectionKey="cta",Title="Matematikte yeni bir sayfa açmaya hazır mısın?",Subtitle="İlk adım",Content="Tanışma görüşmesinde hedeflerini konuşalım, sana uygun çalışma planını birlikte oluşturalım.",DisplayOrder=20},
            new ContentSection{PageKey="Home",SectionKey="contact",Title="Tanışma görüşmesi iste",Subtitle="İletişim",Content="Öğrencinin sınıfını, ihtiyaçlarını ve hedefini paylaşın. En kısa sürede size dönüş yapalım.",DisplayOrder=30},
            new ContentSection{PageKey="Home",SectionKey="section-teacher",Title="Öğretmenini Tanı",Subtitle="Kişiye özel yaklaşım",Content="Matematiği anlaşılır, planlı ve öğrencinin hızına uygun bir öğrenme deneyimine dönüştürüyoruz.",DisplayOrder=2},
            new ContentSection{PageKey="Home",SectionKey="section-hero",Title="Matematiği Ezberlemek Yerine Anlamaya Başla",Subtitle="Kişiye özel matematik eğitimi",Content="5, 6, 7 ve 8. sınıf öğrencileri için düzenli takip, yeni nesil soru çözümü ve eğlenceli matematik oyunları.",DisplayOrder=1},
            new ContentSection{PageKey="Home",SectionKey="section-demo",Title="Demo Ders",Subtitle="Anlatım tarzını keşfet",Content="Kısa demo dersimizi izleyerek konu anlatımı ve soru çözme yaklaşımımızı yakından inceleyin.",DisplayOrder=3},
            new ContentSection{PageKey="Home",SectionKey="demo-featured",Title="Yeni Nesil Sorularda İlk Adım",Subtitle="https://www.youtube.com/",Content="Soruyu doğru okumayı, verileri ayırmayı ve çözüm yolunu planlamayı birlikte öğrenelim.",DisplayOrder=1,IsActive=true},
            new ContentSection{PageKey="Home",SectionKey="section-grades",Title="Sınıfına Uygun Matematik Desteği",Subtitle="5, 6, 7 ve 8. sınıflar",Content="Her sınıf düzeyine uygun konu planı, soru çalışması ve gelişim takibi.",DisplayOrder=4},
            new ContentSection{PageKey="Home",SectionKey="section-programs",Title="Eğitim Programları",Subtitle="Hedefine uygun başlangıç",Content="İhtiyacına uygun matematik programını seç.",DisplayOrder=5},
            new ContentSection{PageKey="Home",SectionKey="section-content",Title="İlgi Çekici İçerikler",Subtitle="Oku, keşfet, uygula",Content="Matematik ipuçları, çalışma önerileri ve kısa konu anlatımları.",DisplayOrder=6},
            new ContentSection{PageKey="Home",SectionKey="content-fractions",Title="Kesirleri Anlamanın Kolay Yolu",Subtitle="Matematik İpucu · 5–6. Sınıf",Content="Kesirleri parça-bütün ilişkisiyle görselleştirerek daha kalıcı öğrenmenin yolları.",DisplayOrder=1},
            new ContentSection{PageKey="Home",SectionKey="content-time",Title="Sınavda Zaman Yönetimi",Subtitle="Çalışma Önerisi · 7–8. Sınıf",Content="Soruları doğru sırayla çözmek ve süreyi dengeli kullanmak için uygulanabilir adımlar.",DisplayOrder=2},
            new ContentSection{PageKey="Home",SectionKey="content-mistakes",Title="Matematikte Sık Yapılan Hatalar",Subtitle="Veli ve Öğrenci Rehberi",Content="İşlem hatalarını azaltmak ve kontrol alışkanlığı geliştirmek için küçük ama etkili yöntemler.",DisplayOrder=3},
            new ContentSection{PageKey="Home",SectionKey="section-daily",Title="Günün Bilgisi",Subtitle="Her gün yeni bir matematik bilgisi",Content="Matematiğin şaşırtıcı, eğlenceli ve günlük hayatla bağlantılı yönlerini keşfet.",DisplayOrder=7},
            new ContentSection{PageKey="Home",SectionKey="section-games",Title="Matematik Oyunları",Subtitle="Oyna, öğren, hızlan",Content="İşlem Arenası herkese açık; üyelik gerekmeden hemen oynayabilirsin.",DisplayOrder=8},
            new ContentSection{PageKey="Home",SectionKey="section-documents",Title="Ücretsiz Dokümanlar",Subtitle="İndir ve çalış",Content="Herkese açık çalışma dosyalarını ücretsiz indir.",DisplayOrder=9},
            new ContentSection{PageKey="Home",SectionKey="section-process",Title="Dersler Nasıl İlerliyor?",Subtitle="Planlı ve görünür süreç",Content="Tanışmadan gelişim analizine kadar her adım öğrencinin ihtiyacına göre planlanır.",DisplayOrder=10},
            new ContentSection{PageKey="Home",SectionKey="process-analysis",Title="Tanışma ve Seviye Analizi",Content="Öğrencinin hedeflerini, güçlü yönlerini ve konu eksiklerini birlikte belirleriz.",DisplayOrder=1},
            new ContentSection{PageKey="Home",SectionKey="process-plan",Title="Kişisel Çalışma Planı",Content="Konu sırası, ders sıklığı ve ödev düzeni öğrenciye özel hazırlanır.",DisplayOrder=2},
            new ContentSection{PageKey="Home",SectionKey="process-practice",Title="Anlatım ve Uygulama",Content="Konu anlatımını kolaydan zora sorular ve yeni nesil çalışmalarla pekiştiririz.",DisplayOrder=3},
            new ContentSection{PageKey="Home",SectionKey="process-follow",Title="Takip ve Geri Bildirim",Content="Ödev, deneme ve konu gelişimini düzenli ölçer; öğrenci ve veliyi bilgilendiririz.",DisplayOrder=4},
            new ContentSection{PageKey="Home",SectionKey="section-tracking",Title="Öğrenci Takip Sistemi",Subtitle="Gelişim tek ekranda",Content="Dersler, ödevler, dokümanlar, denemeler, geri bildirimler ve oyun skorları kişisel öğrenci panelinde.",DisplayOrder=11},
            new ContentSection{PageKey="Home",SectionKey="tracking-lessons",Title="Ders ve Takvim",Content="Yaklaşan derslerini ve ders ayrıntılarını görüntüle.",DisplayOrder=1},
            new ContentSection{PageKey="Home",SectionKey="tracking-homework",Title="Ödev ve Dosya Teslimi",Content="Ödevlerini takip et, açıklama ve dosya ile teslim et.",DisplayOrder=2},
            new ContentSection{PageKey="Home",SectionKey="tracking-progress",Title="Gelişim ve Geri Bildirim",Content="Deneme sonuçlarını, konu gelişimini ve öğretmen yorumlarını incele.",DisplayOrder=3},
            new ContentSection{PageKey="Home",SectionKey="section-stats",Title="Şeffaf ve Ölçülebilir Gelişim",Subtitle="Gerçek veriye dayalı takip",Content="İstatistik değerlerini yalnızca doğrulanabilir verilerle yayınlayın.",DisplayOrder=12,IsActive=false},
            new ContentSection{PageKey="Home",SectionKey="stat-experience",Title="Eğitim Deneyimi",Subtitle="Yıl",Content="1+",DisplayOrder=1,IsActive=false},
            new ContentSection{PageKey="Home",SectionKey="section-testimonials",Title="Öğrenci ve Veli Yorumları",Subtitle="Deneyimlerini anlatıyorlar",Content="Yorumlar izin alınarak ve kişisel bilgiler korunarak yayınlanır.",DisplayOrder=13,IsActive=false},
            new ContentSection{PageKey="Home",SectionKey="testimonial-sample",Title="Öğrenci Velisi",Subtitle="8. Sınıf",Content="Düzenli takip sayesinde eksiklerimizi daha net görmeye başladık.",DisplayOrder=1,IsActive=false},
            new ContentSection{PageKey="Home",SectionKey="section-faq",Title="Sık Sorulan Sorular",Subtitle="Merak ettikleriniz",Content="Dersler ve öğrenci takip sistemiyle ilgili sık sorulan sorular.",DisplayOrder=14},
            new ContentSection{PageKey="Home",SectionKey="faq-online",Title="Dersler çevrim içi mi yapılıyor?",Content="Dersler ihtiyaca göre çevrim içi, yüz yüze veya hibrit olarak planlanabilir.",DisplayOrder=1},
            new ContentSection{PageKey="Home",SectionKey="faq-homework",Title="Ödev takibi yapılıyor mu?",Content="Evet. Ödevler öğrenci panelinden paylaşılır, teslimler incelenir ve geri bildirim verilir.",DisplayOrder=2},
            new ContentSection{PageKey="Home",SectionKey="faq-progress",Title="Velilere bilgi veriliyor mu?",Content="Öğrencinin gelişimi, deneme sonuçları ve önemli durumlar düzenli olarak paylaşılabilir.",DisplayOrder=3},
            new ContentSection{PageKey="Home",SectionKey="homeui-hero-primary",Title="Matematik Yolculuğuna Başla",Content="Eğitim programlarına yönlendiren ana buton.",DisplayOrder=1},
            new ContentSection{PageKey="Home",SectionKey="homeui-hero-secondary",Title="Demo Dersi İzle",Content="Demo ders alanına yönlendiren ikinci buton.",DisplayOrder=2},
            new ContentSection{PageKey="Home",SectionKey="homeui-trust-plan",Title="Seviyeye özel plan",Content="Hero güven rozeti",DisplayOrder=3},
            new ContentSection{PageKey="Home",SectionKey="homeui-trust-follow",Title="Birebir öğrenci takibi",Content="Hero güven rozeti",DisplayOrder=4},
            new ContentSection{PageKey="Home",SectionKey="homeui-trust-interactive",Title="Etkileşimli içerikler",Content="Hero güven rozeti",DisplayOrder=5},
            new ContentSection{PageKey="Home",SectionKey="homeui-trust-homework",Title="Düzenli ödev sistemi",Content="Hero güven rozeti",DisplayOrder=6},
            new ContentSection{PageKey="Home",SectionKey="homeui-mini-task",Title="Denklemin Şifresi",Subtitle="5",Content="Bir sayının 3 katının 5 fazlası 20 ise bu sayı kaçtır?",DisplayOrder=10},
            new ContentSection{PageKey="Home",SectionKey="homeui-mini-meta",Title="6. Sınıf",Subtitle="+20 puan",Content="Mini görevin sınıf ve puan bilgisi.",DisplayOrder=11},
            new ContentSection{PageKey="Home",SectionKey="homeui-mini-success",Title="Doğru cevap mesajı",Content="Harika! Doğru cevabı buldun.|Önce 20 − 5 = 15, sonra 15 ÷ 3 = 5.",DisplayOrder=12},
            new ContentSection{PageKey="Home",SectionKey="homeui-mini-hint",Title="Yanlış cevap ipucu",Content="Yaklaştın.|Önce 20’den 5’i çıkarmayı dene.",DisplayOrder=13},
            new ContentSection{PageKey="Home",SectionKey="homeui-streak",Title="3 günlük çalışma serisi",Content="Mini görev yanında gösterilen seri bilgisi.",DisplayOrder=14},
            new ContentSection{PageKey="Home",SectionKey="homeui-badge",Title="Yeni rozet",Content="Mini görev yanında gösterilen rozet bilgisi.",DisplayOrder=15},
            new ContentSection{PageKey="Home",SectionKey="homeui-lab-fractions",Title="Parça–bütün ilişkisini gör.",Subtitle="Kesir Laboratuvarı",Content="Kesirleri renkli parçalarla oluştur, karşılaştır ve anlamlandır.",DisplayOrder=20},
            new ContentSection{PageKey="Home",SectionKey="homeui-lab-geometry",Title="Şekilleri her açıdan incele.",Subtitle="Geometri Atölyesi",Content="Kenar, açı ve yüz ilişkilerini hareketli modellerle keşfet.",DisplayOrder=21},
            new ContentSection{PageKey="Home",SectionKey="homeui-lab-equation",Title="Dengeyi koruyarak çöz.",Subtitle="Denklem Makinesi",Content="Bir işlemin denklemin iki tarafını nasıl etkilediğini deneyimle.",DisplayOrder=22},
            new ContentSection{PageKey="Home",SectionKey="homeui-game-arena",Title="İşlem Arenası",Subtitle="AKTİF OYUN",Content="Süreye karşı işlemleri çöz, hızlı cevaplarla bonus puan kazan ve kendi rekorunu kır.",DisplayOrder=23},
            new ContentSection{PageKey="Home",SectionKey="homeui-game-geometry",Title="Geometri Kaşifi",Subtitle="YAKINDA",Content="Şekilleri incele, ölçümleri hesapla ve görsel keşif görevlerini tamamla.",DisplayOrder=24},
            new ContentSection{PageKey="Home",SectionKey="homeui-footer",Title="Matematiği keşfet · çöz · ilerle",Subtitle="Footer sloganı",Content="Matematiği yalnızca çözülen bir ders değil, anlaşılır ve keşfedilebilir bir düşünme alanına dönüştürüyoruz.",DisplayOrder=30},
            new ContentSection{PageKey="About",SectionKey="quote",Title="Eğitim anlayışı",Content="Öğretmenin görevi cevabı vermek değil, öğrencinin cevaba giden yolu görebilmesini sağlamaktır.",DisplayOrder=1},
            new ContentSection{PageKey="About",SectionKey="approach",Title="Öğrencinin sesini duyan ders deneyimi",Subtitle="Eğitim yaklaşımım",Content="Öğrencinin nerede zorlandığını birlikte keşfeder, konuyu günlük hayatla ilişkilendirir ve küçük ama kalıcı adımlarla ilerleriz.",DisplayOrder=2},
            new ContentSection{PageKey="About",SectionKey="patience",Title="Sabır",Content="Her öğrencinin öğrenme hızına ve sürecine saygı duyarız.",DisplayOrder=3},
            new ContentSection{PageKey="About",SectionKey="clarity",Title="Netlik",Content="Karmaşık konuları küçük, anlaşılır ve uygulanabilir adımlara böleriz.",DisplayOrder=4},
            new ContentSection{PageKey="About",SectionKey="continuity",Title="Süreklilik",Content="Gelişimi düzenli çalışma, takip ve geri bildirimle kalıcı hâle getiririz.",DisplayOrder=5},
            new ContentSection{PageKey="Trainings",SectionKey="grade-5-6",Title="Temel Güçlendirme Programı",Subtitle="5–6. Sınıf",Content="Temel kavramlar, işlem becerisi, problem çözme ve düzenli ödev takibiyle güçlü bir matematik altyapısı.",DisplayOrder=1},
            new ContentSection{PageKey="Trainings",SectionKey="grade-7",Title="Yeni Nesil Soru Programı",Subtitle="7. Sınıf",Content="Cebir, oran, yüzdeler ve geometri konularını yorumlama ve yeni nesil sorulara uygulama çalışmaları.",DisplayOrder=2},
            new ContentSection{PageKey="Trainings",SectionKey="grade-8-lgs",Title="LGS Hazırlık Programı",Subtitle="8. Sınıf",Content="Kazanım analizi, eksik tamamlama, süre yönetimi, deneme takibi ve LGS soru stratejileri.",DisplayOrder=3},
            new ContentSection{PageKey="SecondaryEducation",SectionKey="grade5",Title="5. Sınıf Matematik",Subtitle="5. Sınıf",Content="Doğal Sayılar|Kesirler|Ondalık Gösterimler|Yüzdeler|Alan ve Çevre|Veri Yorumlama",DisplayOrder=5},
            new ContentSection{PageKey="SecondaryEducation",SectionKey="grade6",Title="6. Sınıf Matematik",Subtitle="6. Sınıf",Content="Çarpanlar ve Katlar|Tam Sayılar|Kesirlerle İşlemler|Oran|Cebirsel İfadeler|Alan ve Çember",DisplayOrder=6},
            new ContentSection{PageKey="SecondaryEducation",SectionKey="grade7",Title="7. Sınıf Matematik",Subtitle="7. Sınıf",Content="Rasyonel Sayılar|Denklemler|Oran ve Orantı|Yüzdeler|Çokgenler|Olasılık",DisplayOrder=7},
            new ContentSection{PageKey="SecondaryEducation",SectionKey="grade8",Title="8. Sınıf Matematik",Subtitle="8. Sınıf",Content="Üslü İfadeler|Kareköklü İfadeler|Özdeşlikler|Doğrusal Denklemler|Üçgenler|Olasılık|LGS Problemleri",DisplayOrder=8}
        };
        var existingKeys = (await dbContext.ContentSections.Select(x => x.PageKey + "|" + x.SectionKey).ToListAsync()).Concat(dbContext.ContentSections.Local.Select(x => x.PageKey + "|" + x.SectionKey)).ToHashSet();
        dbContext.ContentSections.AddRange(defaults.Where(x => existingKeys.Add(x.PageKey + "|" + x.SectionKey)));

        var dailySection = await dbContext.ContentSections.SingleOrDefaultAsync(x=>x.PageKey=="Home"&&x.SectionKey=="section-daily");
        if(dailySection is not null && dailySection.Title=="Günün Matematik Sorusu") { dailySection.Title="Günün Bilgisi";dailySection.Subtitle="Her gün yeni bir matematik bilgisi";dailySection.Content="Matematiğin şaşırtıcı, eğlenceli ve günlük hayatla bağlantılı yönlerini keşfet.";dailySection.UpdatedAt=DateTime.UtcNow; }

        if(!await dbContext.DailyFacts.AnyAsync())
        {
            var facts = new (string Title,string Content,string Category)[]
            {
                ("Sıfır her zaman yokluk demek değildir","Sıfır, sayı doğrusunda pozitif ve negatif sayıların tam ortasındadır. Aynı zamanda basamak değerinde sayıların büyüklüğünü tamamen değiştirebilir.","Sayılar"),
                ("Arıların petekleri neden altıgendir?","Düzgün altıgenler boşluk bırakmadan birleşir ve aynı alanı çevrelemek için az malzeme kullanır. Bu nedenle petekler oldukça verimli bir geometrik yapıdır.","Geometri"),
                ("Pi sayısı hiç bitmez","Pi sayısının ondalık basamakları sonsuza kadar devam eder ve düzenli biçimde tekrar etmez. Pi, çemberin çevresinin çapına oranıdır.","İlginç Matematik"),
                ("Bir kâğıdı 42 kez katlamak","Kuramsal olarak bir kâğıdı 42 kez katlayabilseydik kalınlığı Ay'a ulaşabilecek kadar büyürdü. Bunun nedeni kalınlığın her katta iki katına çıkmasıdır.","İlginç Matematik"),
                ("Asal sayıların sonu yoktur","Öklid, iki binden fazla yıl önce asal sayıların sonsuz tane olduğunu kanıtladı. Ne kadar büyük bir asal sayı bulursak bulalım daha büyüğü vardır.","Matematik Tarihi"),
                ("Fibonacci dizisi doğada görülür","1, 1, 2, 3, 5, 8 şeklinde ilerleyen Fibonacci dizisine ayçiçeği tohumlarında, çam kozalaklarında ve bazı yaprak dizilimlerinde rastlanır.","Günlük Hayatta Matematik"),
                ("Yüzde aslında kesirdir","Yüzde işareti, bir miktarın yüz eş parçadan kaçını ifade eder. Örneğin %25 ile 25/100 ve 1/4 aynı değerdir.","Sayılar"),
                ("Üçgen neden güçlüdür?","Üçgenin kenar uzunlukları sabit olduğunda şekli bozulmaz. Bu yüzden köprülerde, çatılarda ve kulelerde üçgen destekler sık kullanılır.","Geometri"),
                ("Negatif sayıların kabulü zaman aldı","Bugün kolayca kullandığımız negatif sayılar, geçmişte bazı matematikçiler tarafından anlamsız kabul ediliyordu. Borç ve sıcaklık gibi örnekler anlaşılmalarını kolaylaştırdı.","Matematik Tarihi"),
                ("Saatlerde modüler matematik vardır","Saat 10'a 5 saat eklediğimizde 15 yerine 3 deriz. Bu işlem, sayıların belirli bir değerden sonra başa döndüğü modüler aritmetiğe örnektir.","Günlük Hayatta Matematik"),
                ("0,999… aslında 1'e eşittir","Sonsuza kadar dokuzla devam eden 0,999… sayısı 1'e eşittir. Aralarında pozitif büyüklükte hiçbir fark bulunmaz.","İlginç Matematik"),
                ("Bir deste kartta çok fazla sıralama vardır","52 kartlık bir destenin farklı sıralanma sayısı 52 faktöriyeldir. Bu sayı, Dünya'daki atom sayılarıyla karşılaştırılabilecek kadar büyüktür.","İlginç Matematik"),
                ("Eşittir işareti 1557'de kullanıldı","Robert Recorde, sürekli 'eşittir' yazmamak için birbirine paralel iki çizgi kullandı. Ona göre iki şey paralel çizgiler kadar eşit olamazdı.","Matematik Tarihi"),
                ("Bir çemberde sonsuz simetri vardır","Çember, merkezinden geçen her doğruya göre simetriktir. Merkezden sonsuz farklı doğru geçirilebildiği için sonsuz simetri ekseni vardır.","Geometri"),
                ("Barkodlarda matematiksel kontrol bulunur","Barkodların son rakamı genellikle diğer rakamlardan hesaplanan kontrol basamağıdır. Böylece yanlış okunan bir kod sistem tarafından fark edilebilir.","Günlük Hayatta Matematik"),
                ("Tek sayıların toplamı kare oluşturur","İlk n tek sayının toplamı n²'ye eşittir. Örneğin 1+3+5+7=16, yani 4²'dir.","Sayılar"),
                ("Mobius şeridinin tek yüzü vardır","Bir kâğıt şeridi yarım tur çevrilip uçları birleştirildiğinde oluşan Möbius şeridinin yalnızca bir yüzü ve bir kenarı vardır.","Geometri"),
                ("Matematik müzikte ritmi açıklar","Nota süreleri tam, yarım ve çeyrek gibi kesirlerle ifade edilir. Ritim kalıpları matematiksel oranlara dayanır.","Günlük Hayatta Matematik"),
                ("En küçük asal sayı 2'dir","2, hem en küçük hem de tek çift asal sayıdır. Diğer bütün çift sayılar 2'ye bölündükleri için asal olamaz.","Sayılar"),
                ("Google adı çok büyük bir sayıdan gelir","Google adı, 1'in arkasına 100 sıfır yazılarak oluşan googol kelimesinden esinlenmiştir.","İlginç Matematik"),
                ("Dört renk bir harita için yeterlidir","Düzlem üzerindeki her harita, komşu bölgeler farklı renkte olacak şekilde en fazla dört renkle boyanabilir.","Geometri"),
                ("Olasılık hava tahmininde kullanılır","Yağmur olasılığı geçmiş veriler, atmosfer ölçümleri ve matematiksel modeller kullanılarak hesaplanır.","Günlük Hayatta Matematik"),
                ("Roma rakamlarında sıfır yoktu","Roma sayı sisteminde sıfır için bir sembol bulunmuyordu. Basamak değerli sayı sistemlerinin gelişmesi hesaplamaları büyük ölçüde kolaylaştırdı.","Matematik Tarihi"),
                ("Bir sayıyı sıfıra bölmek tanımsızdır","Sıfıra bölmenin sonucu belirli bir sayı olamaz; çünkü sıfırla hangi sayıyı çarparsak çarpalım sonuç sıfırdır.","Sayılar"),
                ("Kar taneleri altı katlı simetri gösterir","Su moleküllerinin buz kristali oluşturma biçimi nedeniyle kar tanelerinde çoğunlukla altı kollu geometrik simetri görülür.","Geometri"),
                ("GPS konum bulmak için geometri kullanır","GPS cihazları, birden fazla uydudan gelen sinyallerin süresini ölçerek uzaklıkları hesaplar ve konumu geometrik olarak belirler.","Günlük Hayatta Matematik"),
                ("Palindrom sayılar iki yönden aynıdır","121 veya 1331 gibi sayılar soldan sağa ve sağdan sola aynı okunur. Bu sayılara palindrom sayı denir.","İlginç Matematik"),
                ("Pascal üçgeninde birçok örüntü vardır","Pascal üçgeninde her sayı üstündeki iki sayının toplamıdır. Üçgende kombinasyonlar, Fibonacci sayıları ve kuvvet açılımları görülebilir.","Sayılar"),
                ("Perspektif çizimleri geometriye dayanır","Resimde uzak nesnelerin küçük görünmesi, kaçış noktaları ve doğrular kullanılarak matematiksel perspektifle gösterilir.","Günlük Hayatta Matematik"),
                ("Matematikte sonsuzluk bir sayı değildir","Sonsuzluk, bitmeyen veya sınırsız bir süreci anlatan kavramdır. Normal sayılar gibi her işlemde kullanılamaz.","İlginç Matematik")
            };
            dbContext.DailyFacts.AddRange(facts.Select((x,i)=>new DailyFact{Title=x.Title,Content=x.Content,Category=x.Category,DisplayOrder=i+1}));
        }

        await dbContext.SaveChangesAsync();
    }
}
