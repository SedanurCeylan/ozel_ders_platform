# Matematik Atölyesi

Özel matematik dersi süreçlerini tek panelden yönetmek için geliştirilmiş, Türkçe bir ASP.NET Core uygulamasıdır. Genel web sitesi, yönetici paneli ve öğrenci paneli aynı uygulama içinde çalışır.

> [!IMPORTANT]
> Proje derleniyor ve Release paketi üretilebiliyor. Deploy öncesinde üretim e-posta/site adresi değerlerini tanımlayın, kalıcı `/data` diski bağlayın ve [deploy kontrol listesini](#deploy-öncesi-zorunlu-kontroller) tamamlayın.

## Özellikler

### Genel web sitesi

- Yönetilebilir ana sayfa, hakkımda ve eğitim içerikleri
- Sabit navigasyon ve mobil uyumlu arayüz
- Demo dersler ve YouTube bağlantıları
- Herkese açık matematik içerikleri ve dokümanlar
- Günün matematik bilgisi
- Öğrenci yorumları
- Instagram, e-posta ve iletişim bilgileri
- İşlem Arenası ve seviye belirleme testi

### Yönetici paneli

- Öğrenci ve öğrenci hesabı yönetimi
- Ders planlama ve katılım kaydı
- Ödev oluşturma, teslim alma, puanlama ve geri bildirim
- Genel veya öğrenciye özel doküman paylaşımı
- Duyuru, bildirim ve ek dosya yönetimi
- Deneme sınavı ve konu gelişimi takibi
- Günün bilgileri ve öğrenci yorumlarının onaylanması
- Ana sayfa ve genel site içeriklerinin düzenlenmesi
- SMTP/e-posta ayarları ve e-posta kayıtları
- İşlem Arenası soru yönetimi
- İşlem geçmişi (audit log)

### Öğrenci paneli

- Yaklaşan ve geçmiş dersler
- Öğrenciye atanmış ödevler ve dosya teslimi
- Paylaşılan dokümanlar
- Duyurular ve bildirimler
- Deneme sonuçları ve gelişim grafikleri
- Profil fotoğrafı ve öğrenci yorumu
- Matematik oyunları

## Teknoloji

- .NET 8 ve ASP.NET Core MVC
- Razor Pages ve ASP.NET Core Identity
- Entity Framework Core 8
- SQLite
- Bootstrap, özel CSS ve JavaScript
- Chart.js
- SMTP tabanlı e-posta
- `BackgroundService` tabanlı otomatik hatırlatmalar

## Proje yapısı

```text
Areas/Admin/       Yönetici paneli
Areas/Student/     Öğrenci paneli
Controllers/       Herkese açık sayfalar
Data/              DbContext, seed ve migration dosyaları
Models/            Veritabanı ve ekran modelleri
Services/          E-posta, dosya, bildirim ve iş kuralları
Views/             Genel site Razor görünümleri
wwwroot/           CSS, JavaScript ve statik dosyalar
App_Data/uploads/  Korumalı yüklemelerin yerel klasörü
```

## Yerel kurulum

### Gereksinimler

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- macOS, Windows veya Linux
- Gerçek e-posta gönderimi için SMTP hesabı

```bash
cd "/Users/seda/Documents/ablam web"
dotnet restore
dotnet build
```

### İlk yönetici hesabı

Parolayı `appsettings.json` içine yazmayın. Yerel geliştirmede User Secrets kullanın:

```bash
dotnet user-secrets set "AdminSeed:Email" "ogretmen@example.com"
dotnet user-secrets set "AdminSeed:Password" "Guclu-Gecici-Parola1!"
```

Parola en az 10 karakter olmalı; büyük harf, küçük harf, rakam ve özel karakter içermelidir.

### Çalıştırma

```bash
dotnet run --launch-profile http
```

Varsayılan adresler:

- Ana site: `http://localhost:5080`
- Giriş: `http://localhost:5080/Identity/Account/Login`
- Yönetici: `http://localhost:5080/Admin`
- Öğrenci: `http://localhost:5080/Student`

İlk başlangıçta migration'lar uygulanır; roller, yönetici hesabı, başlangıç içerikleri ve oyun soruları hazırlanır.

## Yapılandırma

Üretimde gizli değerleri dosyaya yazmak yerine platformun environment variables/secrets bölümünde tanımlayın. ASP.NET Core iç içe ayarlar için çift alt çizgi (`__`) kullanır.

| Değişken | Açıklama | Örnek |
|---|---|---|
| `ASPNETCORE_ENVIRONMENT` | Çalışma ortamı | `Production` |
| `ASPNETCORE_URLS` | Uygulamanın dinleyeceği adres | `http://0.0.0.0:8080` |
| `ConnectionStrings__DefaultConnection` | SQLite bağlantısı | `Data Source=/data/app.db;Cache=Shared` |
| `Storage__RootPath` | Kalıcı yükleme dizini | `/data/uploads` |
| `AdminSeed__Email` | İlk yönetici e-postası | `admin@alanadiniz.com` |
| `AdminSeed__Password` | Güçlü ilk yönetici parolası | secret olarak girilmeli |
| `EmailSettings__SmtpServer` | SMTP sunucusu | sağlayıcınıza göre |
| `EmailSettings__SmtpPort` | SMTP portu | `587` |
| `EmailSettings__SenderName` | Gönderen adı | `Matematik Atölyesi` |
| `EmailSettings__SenderEmail` | Gönderen adresi | `noreply@alanadiniz.com` |
| `EmailSettings__Username` | SMTP kullanıcı adı | secret olarak girilmeli |
| `EmailSettings__Password` | SMTP parolası | secret olarak girilmeli |
| `EmailSettings__EnableSsl` | TLS/SSL kullanımı | `true` |
| `EmailSettings__SiteUrl` | Canlı sitenin kök adresi | `https://alanadiniz.com` |
| `Reminders__Enabled` | Otomatik hatırlatmalar | `true` veya `false` |

> [!WARNING]
> Yönetici parolası, SMTP parolası ve gerçek bağlantı bilgileri Git'e gönderilmemelidir.

## Veritabanı ve migration

Yerel veritabanı `app.db` dosyasıdır. Uygulama başlangıçta bekleyen migration'ları otomatik uygular.

Yeni migration oluşturmak için:

```bash
dotnet ef migrations add AciklayiciMigrationAdi
dotnet ef database update
```

Yedek almak için uygulamayı durdurduktan sonra `app.db` ve yükleme klasörlerini birlikte yedekleyin. Canlı veritabanını çalışan uygulama üzerinden doğrudan kopyalamak yerine SQLite'ın güvenli yedekleme yöntemini kullanın.

## Dosya yüklemeleri

Desteklenen türler:

- PDF
- JPG/JPEG
- PNG
- DOCX

Varsayılan üst sınır 20 MB'dir. Profil fotoğrafları en fazla 5 MB olabilir.

Yerel geliştirmede tüm dosyalar `App_Data/uploads` altında tutulur. Üretimde `Storage__RootPath` ile kalıcı bir dizin seçilmelidir. Ödev ekleri ve öğrenci teslimleri dahil hiçbir özel dosya `wwwroot` altından sunulmaz; indirmeler rol ve sahiplik kontrolü yapan endpoint'lerden geçer.

## Deploy öncesi zorunlu kontroller

Aşağıdaki maddeler tamamlanmadan canlıya çıkmayın:

- [x] `app.db` dosyası publish paketinden çıkarıldı.
- [x] Ödev ve öğrenci teslim dosyaları `wwwroot` dışına taşındı ve yetki kontrollü indiriliyor.
- [x] Tüm yeni yüklemeler `Storage__RootPath` ile tek bir kökten yönetiliyor.
- [ ] SQLite dosyası ve yükleme dizini için kalıcı disk/volume bağlayın.
- [ ] Migration snapshot dosyasını güncel modelle eşitleyin.
- [ ] `EmailSettings__SiteUrl` değerini HTTPS canlı alan adıyla değiştirin.
- [ ] SMTP ve yönetici bilgilerini platform secrets alanına girin.
- [ ] `AllowedHosts` değerini canlı alan adıyla sınırlandırın.
- [x] .NET 8 SDK `global.json` ile sabitlendi ve `/health` endpoint'i eklendi.
- [x] Yerel macOS ortamındaki başlangıç gecikmesi giderildi ve uygulamanın dinlemeye geçtiği doğrulandı.
- [ ] Admin, öğrenci ve yetkisiz ziyaretçi akışlarını uçtan uca test edin.
- [ ] Veritabanı ve yüklenen dosyalar için otomatik yedekleme kurun.

Release kontrolü:

```bash
dotnet build -c Release
dotnet publish -c Release -o ./publish
```

Publish klasöründe gerçek `app.db`, parola, kullanıcı yüklemesi veya geliştirme sırrı bulunmadığını elle doğrulayın.

## Nerede deploy edilmeli?

### 1. Önerilen üretim mimarisi: Azure App Service

Uzun süre kullanılacak gerçek öğrenci verileri için en sağlam seçenek:

- Uygulama: Azure App Service
- Veritabanı: Azure SQL veya PostgreSQL
- Dosyalar: Azure Blob Storage
- Gizli bilgiler: App Service Configuration veya Key Vault

Bu seçenekte SQLite yerine sunucu veritabanına ve yerel yüklemeler yerine Blob Storage'a geçiş gerekir. Buna karşılık yedekleme, ölçekleme ve veri kalıcılığı daha güvenli olur.

Genel akış:

1. Projeyi özel bir GitHub deposuna gönderin.
2. Azure'da Linux tabanlı .NET 8 App Service oluşturun.
3. Deployment Center'dan GitHub deposunu bağlayın.
4. Veritabanı ve Blob Storage kaynaklarını oluşturun.
5. Environment variable/secrets değerlerini Configuration ekranına girin.
6. Alan adını bağlayın ve HTTPS'i zorunlu yapın.
7. İlk deploy sonrasında migration ve giriş akışlarını doğrulayın.

### 2. Daha kolay tek sunucu seçeneği: Ubuntu VPS

Mevcut SQLite yapısını en az mimari değişiklikle kullanmak için küçük bir Ubuntu VPS uygundur:

- Uygulama Kestrel üzerinde çalışır.
- Nginx ters proxy ve HTTPS sağlar.
- `app.db` ile tüm yüklemeler sunucudaki kalıcı `/var/lib/matematik-atolyesi` dizininde tutulur.
- Uygulama `systemd` veya Docker Compose ile tek instance olarak çalıştırılır.

Bu yöntem SQLite için uygundur; ancak sunucu güncellemesi, firewall, SSL, yedekleme ve izleme sorumluluğu size aittir.

### 3. Kolay panel seçeneği: Render + Docker + Persistent Disk

Render .NET uygulamasını Docker ile çalıştırabilir. Fakat servislerin varsayılan dosya sistemi geçicidir; SQLite ve yüklemeler için ücretli persistent disk gerekir. Disk bağlı servis tek instance çalışır ve disk yalnızca mount edilen dizini korur.

Bu projede Render'a geçmeden önce:

1. Depodaki hazır `Dockerfile` ile Render'da Docker Web Service oluşturun.
2. Persistent Disk'i `/data` yoluna bağlayın.
3. `ConnectionStrings__DefaultConnection` ve `Storage__RootPath` değerlerini `/data` altında tutun.
4. Environment variables değerlerini girin.
5. Health Check Path değerini `/health` yapın ve özel alan adını ayarlayın.

Ücretsiz ve persistent disk bulunmayan bir serviste mevcut SQLite/yükleme yapısını kullanmayın; redeploy veya restart sonrasında veriler kaybolabilir.

Demo amacıyla depodaki `render.yaml` Blueprint'i kullanılabilir. Bu tanım ücretsiz instance, `/health` kontrolü ve geçici `/tmp` depolamasını otomatik ayarlar. Blueprint kurulurken `AdminSeed__Email` ve `AdminSeed__Password` değerleri Render ekranında secret olarak girilir. Ücretsiz demo servisindeki veriler restart veya deploy sonrasında sıfırlanabilir.

## Demo ders YouTube bağlantısı

- `Herkese Açık`: Arama ve kanal üzerinden bulunabilir.
- `Liste Dışı`: Yalnızca bağlantıyı bilenler izleyebilir; demo ders için genellikle en uygun seçenektir.
- `Özel`: Yalnızca YouTube'da izin verilen Google hesapları izleyebilir. Siteye bağlantı eklemek tek başına erişim sağlamaz.

## Güvenlik notları

- Admin ve Student alanları rol bazlı yetkilendirme kullanır.
- POST işlemlerinde antiforgery doğrulaması bulunur.
- Parolalar ASP.NET Core Identity tarafından hash'lenir.
- Başarısız girişlerde geçici hesap kilitleme uygulanır.
- Öğrenciye özel sorgular oturumdaki kullanıcıyla sınırlandırılır.
- Dosyanın uzantısı ve tarayıcıdan gelen MIME bilgisi kontrol edilmektedir; yüksek güvenlik gereken üretimde gerçek dosya imzası ve zararlı yazılım taraması da eklenmelidir.

## Sorun giderme

### Uygulama başlamıyor

```bash
dotnet restore
dotnet build
dotnet run --launch-profile http
```

Bağlantı dizesini, dosya yazma izinlerini, migration durumunu ve başlangıç seed loglarını kontrol edin.

### E-posta gitmiyor

- SMTP sunucusu ve portunu kontrol edin.
- Gönderen adresinin SMTP hesabıyla uyumlu olduğundan emin olun.
- Sağlayıcı iki aşamalı doğrulama kullanıyorsa uygulama parolası oluşturun.
- Yönetici panelindeki e-posta kayıtlarını kontrol edin.

### Yüklenen dosya deploy sonrasında kayboluyor

Platformun dosya sistemi geçicidir. Kalıcı disk bağlayın veya dosyaları nesne depolama servisine taşıyın.

### Migration çakışıyor

Migration dosyaları ile `ApplicationDbContextModelSnapshot.cs` dosyasının aynı modeli temsil ettiğini doğrulayın. Canlı veritabanını değiştirmeden önce yedek alın.

## Lisans ve gizlilik

Projede henüz açık kaynak lisansı tanımlanmamıştır. Öğrenci ve veli bilgileri kişisel veri içerdiği için depo özel tutulmalı; canlı veritabanı, yedekler ve yüklenen dosyalar herkese açık paylaşılmamalıdır.
