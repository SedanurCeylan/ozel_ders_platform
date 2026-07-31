# Matematik Atölyesi — Kullanım Kılavuzu

Matematik Atölyesi; özel ders öğretmeninin öğrencilerini, derslerini, ödevlerini, dokümanlarını, duyurularını, gelişim kayıtlarını ve bildirimlerini tek panelden yönetmesini sağlayan Türkçe bir ASP.NET Core uygulamasıdır.

Uygulamada iki kullanıcı rolü bulunur:

- **Admin:** Öğretmen ve sistem yöneticisi
- **Student:** Yalnızca kendi bilgilerine erişebilen öğrenci

## İçindekiler

1. [Teknik bilgiler](#teknik-bilgiler)
2. [İlk kurulum](#ilk-kurulum)
3. [Uygulamayı çalıştırma](#uygulamayı-çalıştırma)
4. [Yönetici kullanım kılavuzu](#yönetici-kullanım-kılavuzu)
5. [Öğrenci kullanım kılavuzu](#öğrenci-kullanım-kılavuzu)
6. [İşlem Arenası](#işlem-arenası)
7. [E-posta ayarları](#e-posta-ayarları)
8. [Dosya yüklemeleri](#dosya-yüklemeleri)
9. [Otomatik hatırlatmalar](#otomatik-hatırlatmalar)
10. [Güvenlik](#güvenlik)
11. [Veritabanı ve migration](#veritabanı-ve-migration)
12. [Sorun giderme](#sorun-giderme)

## Teknik bilgiler

- ASP.NET Core MVC ve Razor Pages
- .NET 8
- ASP.NET Core Identity
- Entity Framework Core 8
- SQLite
- Bootstrap ve özel responsive CSS
- Chart.js ile gelişim grafikleri
- SMTP tabanlı HTML e-posta sistemi
- ASP.NET Core `BackgroundService` ile otomatik hatırlatmalar

Ana proje dosyası `OzelDersYonetim.csproj`, yerel veritabanı ise `app.db` dosyasıdır.

## İlk kurulum

### Gereksinimler

- .NET 8 SDK
- macOS, Windows veya Linux
- Gerçek e-posta gönderilecekse bir SMTP hesabı

.NET kurulumunu kontrol edin:

```bash
dotnet --version
```

### Proje klasörüne geçiş

Klasör adında boşluk bulunduğu için yolu çift tırnakla kullanın:

```bash
cd "/Users/seda/Documents/ablam web"
```

### Yönetici hesabını tanımlama

Yönetici parolası kaynak koda veya `appsettings.json` dosyasına yazılmaz. User Secrets kullanılır:

```bash
dotnet user-secrets set "AdminSeed:Email" "ogretmen@example.com"
dotnet user-secrets set "AdminSeed:Password" "Guclu-Gecici-Parola1!"
```

Parola en az 10 karakter olmalı; büyük harf, küçük harf, rakam ve özel karakter içermelidir.

> Terminalde `dquote>` görünürse çift tırnaklardan biri kapanmamıştır. `Control + C` ile komutu iptal edip satırı yeniden, düz çift tırnaklarla yazın.

Uygulama ilk açılışta:

- Veritabanı migration'larını uygular.
- `Admin` ve `Student` rollerini oluşturur.
- User Secrets ile verilen yönetici hesabını oluşturur.
- Varsayılan site içeriklerini ve İşlem Arenası sorularını hazırlar.

## Uygulamayı çalıştırma

Projeyi derleyin:

```bash
dotnet build
```

Uygulamayı başlatın:

```bash
dotnet run
```

Tarayıcıdan açın:

- Ana site: [http://localhost:5080](http://localhost:5080)
- Giriş: [http://localhost:5080/Identity/Account/Login](http://localhost:5080/Identity/Account/Login)
- Yönetici paneli: [http://localhost:5080/Admin](http://localhost:5080/Admin)
- Öğrenci paneli: [http://localhost:5080/Student](http://localhost:5080/Student)

Uygulamayı durdurmak için terminalde `Control + C` kullanın.

Kod değişikliklerinden sonra çalışan uygulamayı durdurup tekrar `dotnet run` komutunu çalıştırın.

## Genel web sitesi

Ziyaretçiler giriş yapmadan şu sayfaları görüntüleyebilir:

- Ana Sayfa
- Hakkımda
- Eğitimler
- Ortaöğretim içerikleri

Navbar kullanıcı durumuna göre değişir:

- Misafirde **Öğrenci Girişi**
- Öğrencide **Öğrenci Panelim**
- Yöneticide **Yönetim Paneli**

Genel sayfalardaki dinamik metinler yönetici panelindeki **Genel İçerikler** ve **Site Ayarları** bölümlerinden yönetilir.

## Yönetici kullanım kılavuzu

Yönetici hesabıyla giriş yapıldığında kullanıcı `/Admin` alanına yönlendirilir.

### Genel Bakış

Dashboard üzerinde aşağıdaki özetler gösterilir:

- Toplam, aktif ve pasif öğrenci sayıları
- Bugünkü ve yaklaşan dersler
- Bekleyen, geciken ve değerlendirme bekleyen ödevler
- Toplam doküman sayısı
- Gönderilemeyen e-postalar
- Bu ay yapılan dersler
- Son öğrenciler, teslimler, sınav sonuçları ve e-posta hataları

### Öğrenci yönetimi

**Tüm Öğrenciler** sayfasında:

- Ad, soyad veya e-posta ile arama
- Sınıf seviyesi filtresi
- Aktif/pasif filtresi
- Çevrim içi, yüz yüze veya hibrit ders filtresi
- Ödev teslim durumu filtresi
- Kayıt tarihine göre sıralama

yapılabilir.

Yeni öğrenci kaydında şu bilgiler tutulabilir:

- Ad, soyad ve profil fotoğrafı
- E-posta, telefon ve doğum tarihi
- Sınıf seviyesi ve okul
- Veli adı, telefonu ve e-postası
- Ders türü ve ders biçimi
- Kayıt tarihi ve öğretmen notu
- Hesap aktiflik durumu

Öğrenci oluştururken isteğe bağlı olarak giriş hesabı da oluşturulabilir. Geçici parola yalnızca Identity tarafından hash'lenerek saklanır; veritabanında düz metin tutulmaz.

Öğrenci detayında:

- Genel bilgiler
- Ders programı ve katılım
- Ödevler ve teslimler
- Paylaşılan dokümanlar
- Deneme sonuçları
- Konu gelişimi
- Özel öğretmen notları
- E-posta geçmişi

birlikte görüntülenir.

Yönetici ayrıca:

- Öğrenciyi düzenleyebilir.
- Aktif veya pasif yapabilir.
- Akademik geçmişi korunacak şekilde arşivleyebilir.
- Profil fotoğrafını değiştirebilir.
- Öğrenci hesabı oluşturabilir.
- Geçici parola belirleyerek şifreyi sıfırlayabilir.
- Öğrenciye, veliye veya her ikisine e-posta gönderebilir.

### Ders ve katılım yönetimi

**Takvim ve Dersler** bölümünde:

- Bir veya birden fazla öğrenciye ders oluşturulur.
- Başlangıç ve bitiş tarihi belirlenir.
- Çevrim içi veya yüz yüze ders seçilir.
- Çevrim içi toplantı bağlantısı eklenir.
- Ders konusu ve öğretmen notları tutulur.
- Planlandı, onaylandı, tamamlandı, ertelendi veya iptal durumları yönetilir.

Ders sonrasında öğrenci bazında:

- Katıldı/katılmadı bilgisi
- Çözülen soru sayısı
- Performans notu

kaydedilebilir. Katılım oranı öğrenci detayında otomatik hesaplanır.

### Ödev yönetimi

**Tüm Ödevler** bölümünde:

- Tek öğrenciye, birden fazla öğrenciye veya sınıf seviyesine ödev atanabilir.
- Başlık, konu, açıklama, başlangıç ve son teslim tarihi girilebilir.
- Maksimum puan belirlenebilir.
- PDF, görsel veya desteklenen ek dosya yüklenebilir.
- Geç teslim, dosya yükleme ve öğrenci açıklaması seçenekleri yönetilebilir.
- Ödev aktif veya pasif yapılabilir.

Teslim detayında yönetici:

- Öğrencinin açıklamasını ve yüklediği dosyayı görür.
- Puan verir.
- Öğretmen geri bildirimi yazar.
- Yeniden teslim ister.

Ödev atama ve değerlendirme işlemleri öğrenciye site içi bildirim oluşturabilir; SMTP ayarlıysa e-posta da gönderilir.

### Doküman yönetimi

Doküman erişim türleri:

- Herkese açık
- Giriş yapan öğrencilere açık
- Belirli öğrencilere özel

Yönetici doküman yükleyebilir, kategori belirleyebilir, öğrenci seçebilir, düzenleyebilir ve erişime kapatabilir. Öğrencinin görüntüleme ve indirme zamanı kaydedilir.

### Duyuru yönetimi

Duyurular:

- Tüm öğrencilere
- Belirli sınıf seviyesine
- Seçilen öğrencilere

yayınlanabilir. Başlangıç/bitiş tarihi, aktiflik, ek PDF veya görsel ve e-posta gönderim tercihi bulunur. Yayından kaldırılan duyuruların okuma geçmişi korunur.

### Deneme ve gelişim takibi

Öğrenci seçildikten sonra:

- Deneme sınavı sonucu eklenebilir.
- Doğru, yanlış ve boş sayıları girilebilir.
- Net değeri `Doğru - (Yanlış / 4)` formülüyle hesaplanır.
- Matematik puanı, süre ve öğretmen yorumu eklenebilir.
- Sonuç isteğe bağlı olarak veliye e-posta ile gönderilebilir.
- Konu bazlı başarı yüzdesi ve gelişim durumu kaydedilebilir.
- Gelişim kaydı veliye bildirilebilir.
- Yalnızca yöneticinin görebildiği özel öğretmen notları eklenebilir.

Öğrenci ve yönetici ekranlarında net değişimi grafikle gösterilir.

### Öğretmen notları

Öğretmen notları öğrenciye gösterilmez. Kullanılabilen kategoriler:

- Genel
- Akademik
- Ödev
- Katılım
- Veli görüşmesi
- Gelişim
- Hatırlatma

Notlar önemli olarak işaretlenebilir.

### E-posta geçmişi

Her gönderim için:

- Alıcı
- Konu
- E-posta türü
- İçerik
- Başarı durumu
- Hata mesajı
- Gönderim tarihi

kaydedilir. Liste başarı durumuna ve arama metnine göre filtrelenebilir. Başarısız e-postalar yeniden gönderilebilir.

SMTP ayarlı değilse uygulama çökmez; gönderim başarısız olarak kaydedilir.

### İşlem geçmişi

Önemli yönetici işlemleri Audit Log'a yazılır. İşlem geçmişi, işlem ve kayıt türüne göre filtrelenebilir. Kayıtlarda kullanıcı, zaman, IP adresi ve ilgili kayıt bilgileri bulunur.

### Oyun yönetimi

Yönetici:

- İşlem Arenası skorlarını görebilir.
- Doğru/yanlış, doğruluk, ortalama cevap süresi ve toplam puanı inceleyebilir.
- Şüpheli derecede hızlı oturumları görebilir.
- Hatalı oyun sonuçlarını silebilir.
- Soru bankasını sınıf ve konuya göre filtreleyebilir.
- Yeni çoktan seçmeli soru ekleyebilir.
- Soruyu aktif veya pasif yapabilir.

### Site içeriği ve ayarlar

**Genel İçerikler** bölümünden sayfa/bölüm bazlı metinler, başlıklar, sıralama ve aktiflik yönetilir.

**Site Ayarları** bölümünden site adı, öğretmen bilgileri, iletişim bilgileri ve genel metinler güncellenir.

## Öğrenci kullanım kılavuzu

Öğrenci hesabı yönetici tarafından oluşturulur. Öğrenci e-posta adresi ve geçici parolayla giriş yapar. İlk girişte parola değişikliği istenebilir.

### Panelim

Dashboard üzerinde:

- Yaklaşan ders sayısı
- Bekleyen ve geciken ödevler
- Değerlendirilen ödevler
- Okunmamış bildirimler
- Paylaşılan dokümanlar
- En yakın ders
- Yaklaşan teslim tarihleri
- Son duyurular
- Son dokümanlar
- Öğretmen geri bildirimleri

görüntülenir.

### Profilim

Öğrenci kişisel, okul ve veli bilgilerini görüntüler. E-posta ve temel kimlik bilgileri yönetici kayıtlarıyla ilişkilidir. Hesap sayfasından ad, soyad ve telefon bilgileri; şifre sayfasından parola güncellenebilir.

### Derslerim

Öğrenci yalnızca kendisine atanmış yaklaşan ve geçmiş dersleri görür. Çevrim içi derslerde toplantı bağlantısına ders detayından erişir.

### Ödevlerim

Öğrenci:

- Aktif, tamamlanan ve geciken ödevleri filtreler.
- Ödev açıklamasını ve eklerini görüntüler.
- Açıklama yazar.
- Desteklenen dosyayı yükler.
- Teslim durumunu takip eder.
- Puanı ve öğretmen geri bildirimini okur.
- Yeniden teslim istendiyse yeni sürüm yükler.

### Dokümanlarım

Öğrenci genel ve kendisine özel dokümanları kategoriye göre görüntüler ve indirir. Başka öğrenciye ait özel dokümanlara erişemez.

### Duyurular

Öğrenci kendisine, sınıfına veya tüm öğrencilere gönderilmiş aktif duyuruları görür. Duyuru açıldığında okunma zamanı kaydedilir.

### Gelişimim

Öğrenci:

- Deneme sınavı sonuçlarını
- Net değişimi grafiğini
- Konu gelişim yüzdelerini
- Ödev başarı oranını
- Katılım oranını
- Öğretmen geri bildirimlerini

görüntüleyebilir. Yöneticinin özel öğretmen notları burada gösterilmez.

### Bildirimler

Bildirim türleri arasında yeni ödev, değerlendirme, ders, doküman, duyuru ve hatırlatmalar bulunur. Öğrenci bildirime tıklayarak ilgili kayda gider veya tüm bildirimleri okundu olarak işaretler.

### Matematik Oyunları

Öğrenci panelindeki **Matematik Oyunları** menüsünden İşlem Arenası'na ulaşılır. Geometri Kaşifi kartı şimdilik **Yakında** durumundadır.

## İşlem Arenası

İşlem Arenası 5, 6, 7 ve 8. sınıflar için hız ve doğruluk odaklı matematik oyunudur.

Oyun başlamadan önce:

- Sınıf seviyesi
- Konu
- Kolay, orta veya zor seviye
- 30 saniyelik Sprint, 45 saniyelik Hızlı veya 60 saniyelik Klasik mod

seçilir. Varsayılan sınıf öğrencinin profilinden alınır.

Oyun sırasında üst alanda:

- Kalan süre
- Puan
- Doğru sayısı
- Yanlış sayısı
- Doğru cevap serisi
- Öğrenci ve rakip ilerleme çubukları

gösterilir.

Puanlama:

- Sorunun temel puanı zorluk seviyesine göre belirlenir.
- İlk 3 saniyedeki doğru cevap `+50` hız bonusu kazanır.
- İlk 6 saniyedeki doğru cevap `+25` hız bonusu kazanır.
- Üçlü ve beşli doğru serilerde ek seri bonusu verilir.

Seçeneklerin sırası her soruda karıştırılır; doğru cevap aynı yerde gösterilmez. Soru zorluğu öğrencinin doğru/yanlış performansına göre uyarlanabilir.

Oyun sonunda:

- Toplam puan
- Doğru ve yanlış sayısı
- Başarı yüzdesi
- Ortalama cevap süresi
- En uzun seri
- Önceki rekor ve yeni rekor durumu
- Yanlış cevapların açıklamaları

gösterilir.

Puan istemciye güvenilerek kaydedilmez. Soru, öğrenci sahipliği, cevap süresi ve puan sunucuda doğrulanır. Tamamlanmış oturum tekrar tamamlanamaz ve öğrenci başka öğrencinin sonucuna erişemez.

## E-posta ayarları

### Ana sayfa iletişim formu — EmailJS

Ana sayfadaki iletişim formu EmailJS Browser SDK kullanır. EmailJS hesabında bir e-posta servisi ve şablon oluşturduktan sonra **Yönetim Paneli → Site Ayarları → İletişim formu · EmailJS** bölümüne şu üç değeri girin:

- Service ID
- Template ID
- Public Key

EmailJS şablonunda kullanılabilen form değişkenleri:

- `{{from_name}}`
- `{{reply_to}}`
- `{{phone}}`
- `{{grade}}`
- `{{message}}`
- `{{to_email}}`

EmailJS özel anahtarını site ayarlarına veya kaynak koda eklemeyin. Ayarlar girilmeden form dış servise istek göndermez ve ziyaretçiye yapılandırma uyarısı gösterir.

### Sistem e-postaları — SMTP

SMTP parolasını `appsettings.json` içine yazmayın. User Secrets kullanın:

```bash
dotnet user-secrets set "EmailSettings:SmtpServer" "smtp.example.com"
dotnet user-secrets set "EmailSettings:SmtpPort" "587"
dotnet user-secrets set "EmailSettings:SenderName" "Matematik Atölyesi"
dotnet user-secrets set "EmailSettings:SenderEmail" "ogretmen@example.com"
dotnet user-secrets set "EmailSettings:Username" "ogretmen@example.com"
dotnet user-secrets set "EmailSettings:Password" "SMTP-UYGULAMA-PAROLASI"
dotnet user-secrets set "EmailSettings:EnableSsl" "true"
dotnet user-secrets set "EmailSettings:SiteUrl" "http://localhost:5080"
```

Gmail gibi sağlayıcılarda normal hesap parolası yerine uygulama parolası gerekebilir.

E-posta gönderilebilen başlıca durumlar:

- Yeni öğrenci hesabı
- Şifre sıfırlama
- Yeni ödev ve değerlendirme
- Ders planlama/değişiklik
- Doküman paylaşımı
- Duyuru
- Otomatik ödev/ders hatırlatması
- Öğrenci veya veliye özel mesaj
- Deneme sonucu ve gelişim bilgilendirmesi

## Dosya yüklemeleri

Varsayılan maksimum dosya boyutu 20 MB'dir:

```json
"FileUploads": {
  "MaximumSizeMb": 20
}
```

Ödev teslimlerinde desteklenen temel türler:

- PDF
- JPG/JPEG
- PNG
- DOCX

Dosyalarda uzantı ve içerik türü kontrol edilir. Benzersiz dosya adı üretilir ve kullanıcıdan gelen dosya yolu doğrudan kullanılmaz.

## Otomatik hatırlatmalar

Arka plan servisi düzenli olarak yaklaşan/geciken ödevleri ve yaklaşan dersleri kontrol eder. Aynı hatırlatmanın tekrar gönderilmemesi için gönderim kaydı tutulur.

`appsettings.json` ayarı:

```json
"Reminders": {
  "Enabled": true,
  "CheckIntervalMinutes": 15
}
```

Hatırlatmaları kapatmak için `Enabled` değerini `false` yapın.

## Güvenlik

- Admin ve Student için rol tabanlı yetkilendirme uygulanır.
- Öğrenci sorgularında kullanıcı kimliği sunucu tarafında alınır.
- Öğrenci başka öğrencinin ödev, ders, doküman, bildirim veya oyun kaydına erişemez.
- Formlarda CSRF koruması kullanılır.
- Parolalar Identity tarafından hash'lenir.
- Geçici parolalar e-posta geçmişinde saklanmaz.
- Dosya türü, MIME ve boyut kontrolleri uygulanır.
- Hassas yapılandırmalar User Secrets veya ortam değişkeninde tutulur.
- Güvenlik başlıkları ve hesap kilitleme politikası etkindir.
- Öğrenci silmek yerine akademik geçmişi koruyan arşivleme uygulanır.
- Yönetici işlemleri Audit Log ile izlenir.

## Veritabanı ve migration

Uygulama SQLite kullanır. Veritabanı dosyası:

```text
app.db
```

Uygulama başlatılırken bekleyen migration'lar otomatik uygulanır. Manuel kullanım:

```bash
dotnet ef database update
```

Yeni migration oluşturmak için:

```bash
dotnet ef migrations add MigrationAdi
dotnet ef database update
```

Gerçek veriler bulunan `app.db` dosyasını silmeyin. Büyük değişikliklerden önce yedeğini alın:

```bash
cp app.db app-backup.db
```

## Proje yapısı

```text
Areas/Admin/       Yönetici controller ve görünümleri
Areas/Student/     Öğrenci controller ve görünümleri
Areas/Identity/    Giriş ve hesap yönetimi
Configuration/     E-posta, yükleme, hatırlatma ve seed ayarları
Controllers/       Genel site controller'ları
Data/              DbContext, seed işlemleri ve migration'lar
Models/            Veritabanı ve form modelleri
Services/          İş kuralları, dosya, e-posta, bildirim ve oyun servisleri
Views/             Genel site görünümleri
wwwroot/           CSS, JavaScript ve statik dosyalar
app.db             SQLite veritabanı
```

## Sorun giderme

### `dquote>` görünüyor

Komuttaki çift tırnak kapanmamıştır. `Control + C` ile iptal edip komutu yeniden yazın.

### `localhost` açılmıyor

- Terminalde `dotnet run` işleminin çalıştığını kontrol edin.
- Terminalde yazan gerçek adresi kullanın.
- Varsayılan adres `http://localhost:5080` şeklindedir.
- Port başka uygulama tarafından kullanılıyorsa eski `dotnet run` işlemini `Control + C` ile durdurun.

### Değişiklik görünmüyor

Çalışan uygulamayı durdurun ve tekrar başlatın:

```bash
dotnet run
```

Gerekirse tarayıcıda zorla yenileme yapın: macOS için `Command + Shift + R`.

### E-posta gitmiyor

- SMTP sunucu ve portunu kontrol edin.
- Gönderen e-posta adresini kontrol edin.
- SSL ayarını kontrol edin.
- Sağlayıcı uygulama parolası istiyorsa normal parola kullanmayın.
- Yönetici panelindeki **E-posta Geçmişi** sayfasından hata metnini inceleyin.

### Öğrenci giriş yapamıyor

- Öğrenci için Identity hesabı oluşturulduğunu kontrol edin.
- Öğrencinin aktif olduğunu kontrol edin.
- Gerekirse öğrenci detayından şifreyi sıfırlayın.
- Beş başarısız girişten sonra hesap 15 dakika kilitlenebilir.

### Oyun hemen bitiyor veya soru görünmüyor

- Uygulamayı güncel kodla yeniden başlatın.
- Yeni bir oyun oturumu oluşturun.
- Sınıf/konu seçimine uygun aktif soru bulunduğunu yönetici soru bankasından kontrol edin.
- Hiç cevaplanmamış oturumlar boş sonuç olarak kaydedilmez.

### Derleme kontrolü

```bash
dotnet build --no-restore
```

Başarılı sonuçta `0 Hata` görünmelidir.
