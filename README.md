# Ticket Sistemi

Taner Saydam Web API Eğitimi kapsamında hazırlanan ödev projesi.

## Ödev Şartları

Bu proje, **destek paneli (ticket sistemi)** geliştirilmesini isteyen ödevin gereksinimlerine göre hazırlanmıştır.

### Gereksinimler

- **Destek talebi açma:** Kullanıcı destek talebi (ticket) açabilmelidir.
- **Cevap yazma:** Yetkili kişi, açılan taleplere cevap yazabilmelidir.
- **Talebi kapatma:** İşlem tamamlandıktan sonra yetkili kişi talebi kapatabilmelidir.
- **Geçmişi görüntüleme:** Talep geçmişi okunaklı biçimde, istenirse en yeni en üstte veya en eski en üstte olacak şekilde görüntülenebilmelidir.
- **Talebin sonucu:** Talebi açan kişi, talebinin nasıl sonuçlandığını görebilmelidir.
- **Kullanıcı sistemi:** Sonradan kayıt olup destek talebi açan kullanıcılar ile bu işi yöneten yetkililer birbirinden ayrılmalıdır.
- **Giriş ve yetki:** Kullanıcı giriş yaptıktan sonra talep açabilmeli ve yalnızca kendi taleplerini görebilmelidir.
- **Authentication:** Kimlik doğrulama araştırma ödevi olarak bırakılmıştır.

### Kullanılan teknolojiler

.NET 10, EF Core, ASP.NET Core Identity, JWT, Carter, FluentValidation, Scalar

### Klasör yapısı

```text
TicketSistemi/
    Context/
        ApplicationDbContext.cs
        DbSeeder.cs
    DTOs/
        Auth/
            AuthResponse.cs
            LoginRequestDto.cs
            RegisterRequestDto.cs
        Tickets/
            CreateMessageRequest.cs
            CreateTicketRequest.cs
            TicketDetailResponse.cs
            TicketMessageResponse.cs
            TicketSummaryResponse.cs
    Extensions/
        BearerSecuritySchemeTransformer.cs
        ResultExtensions.cs
        UserClaimsExtensions.cs
    Migrations/
    Models/
        Common/
            Result.cs
        Enums/
            ErrorType.cs
            SortOrder.cs
            TicketStatus.cs
        ApplicationUser.cs
        Ticket.cs
        TicketMessage.cs
        UserRoles.cs
    Modules/
        AdminModule.cs
        AuthModule.cs
        TicketModule.cs
    Services/
        Abstractions/
            IAuthService.cs
            ITicketService.cs
            ITokenService.cs
        Implementations/
            AuthService.cs
            TicketService.cs
            TokenService.cs
    Validator/
        CreateMessageRequestValidator.cs
        CreateTicketRequestValidator.cs
        LoginRequestValidator.cs
        RegisterRequestValidator.cs
    appsettings.json
    Program.cs
```
## Roller ve Yetkiler

**Kullanıcı (User)**
- Kayıt olabilir ve giriş yapabilir.
- Destek talebi (ticket) açabilir.
- Yalnızca kendi taleplerini görebilir, başkasının talebine erişemez.
- Kendi talebine mesaj yazabilir (talep kapalıysa yazamaz).

**Yönetici (Admin)**
- Tüm talepleri görebilir ve duruma göre filtreleyebilir.
- Taleplere cevap yazabilir.
- Talebi kapatabilir.
- Yalnızca kapatılmış talepleri silebilir.
- Admin hesabı kayıt ile oluşmaz, uygulama ilk çalıştığında ayarlardaki bilgilerle otomatik oluşturulur.

## Endpoint'ler

### Auth
- POST /auth/register : Yeni kullanıcı kaydı (User rolü verilir)
- POST /auth/login : Giriş yapar, JWT token döner

### Tickets (User)
- POST /tickets : Yeni talep açar
- GET /tickets/my : Kendi taleplerini listeler (order: Asc/Desc)
- GET /tickets/{id} : Kendi talebinin detayını ve mesajlarını getirir
- POST /tickets/{id}/messages : Kendi talebine mesaj yazar

### Admin
- GET /admin/tickets : Tüm talepleri listeler (status ve order filtresi)
- GET /admin/tickets/{id} : Talep detayını ve mesajlarını getirir
- POST /admin/tickets/{id}/messages : Talebe cevap yazar
- POST /admin/tickets/{id}/close : Talebi kapatır
- DELETE /admin/tickets/{id} : Kapatılmış talebi siler

## Talep Durumları

Open -> InProgress -> Closed

- Talep Open olarak açılır.
- Admin ilk cevabı yazınca InProgress olur.
- Admin kapatınca Closed olur ve yeni mesaj yazılamaz.

## İstek Sınırı (Rate Limit)

- Auth endpoint'leri: dakikada 5 istek (IP başına)
- Ticket endpoint'leri: dakikada 20 istek (IP başına)

## Kararlar ve Nedenleri

1. **Karar:** Ayrı projeler yerine tek projede klasörler
   
   **Neden:** Proje küçük, ayrı projeler ek yapılandırma getirirdi. Sorumluluklar klasörlerle ayrılır, gerekirse sonradan bölünebilir.
2. **Karar:** Identity + JWT
   
   **Neden:** Parola hash'leme, roller, lockout hazır gelir, kendin yazmak tehlikelidir. API için AddIdentityCore (AddIdentity cookie ile çakışır).
3. **Karar:** Id'ler Guid
   
   **Neden:** /tickets/5 gibi ardışık ID'yi tahmin etmek kolaydır. Ama asıl koruma ownership kontrolüdür, Guid yalnızca ek bir katmandır.
4. **Karar:** Entity yerine DTO kullanımı
   
   **Neden:** Client Status veya CreatedByUserId göndererek sistemi kandırabilir (over-posting).
5. **Karar:** FluentValidation, yalnızca Request DTO'lar için
    
   **Neden:** Validator client'tan gelen veriyi doğrular, Response ve entity'ler için gerekli değil.
6. **Karar:** Admin register ile oluşmaz, seed ile oluşur
    
   **Neden:** Register'da rol seçimi olsa herkes kendini admin yapabilir. Admin e-postası ve parolası user secrets'tan okunur, repoda yok.
7. **Karar:** Rol adları const string
    
   **Neden:** AddToRoleAsync string bekler.
8. **Karar:** Başkasının ticket'ı için 404 (403 değil)
    
   **Neden:** 403 "böyle bir ticket var, ama senin değil" demektir ve varlığını ifşa eder.
9. **Karar:** İş kuralları serviste, modül ince
    
   **Neden:** Kapalı ticket'a mesaj yazılamaz (409), admin ilk cevabı yazınca durum InProgress olur.
10. **Karar:** Result sınıfı (exception yerine)
    
    **Neden:** Yanlış parola istisna değil, normal bir durumdur. ErrorType servisteki hatayı 404/409/400'e çevirir.
11. **Karar:** Login'de genel hata mesajı + lockout
 
    **Neden:** Saldırgan hangi username'in var olduğunu öğrenmesin. 5 yanlış denemeden sonra hesap kilitlenir.
12. **Karar:** Yalnızca kapalı ticket silinir, cascade delete
    
    **Neden:** Açık ticket'ı silmek geçmişi kaybettirir. Ticket silinince mesajları da veritabanından silinir.
12. **Karar:** Rate limit: auth 5/dk, ticket 20/dk, IP bazında
    
    **Neden:** Login/register brute-force saldırısına açıktır. Admin güvenilir bir roldür, ona limit koymadım.
13. **Karar:** CancellationToken
    
    **Neden:** Client isteği kestiğinde sunucu veritabanına gönderilen artık gereksiz sorguyu durdurur.
14. **Karar:** Tarihler DateTimeOffset ve UTC
    
    **Neden:** Saat dilimi karışıklığı olmasın.

## Sorunlar ve Çözümleri

### Sorun 1: Migration/DbContext hatası: tip uyumsuzluğu

**Sebep:** ApplicationUser, string Id'li IdentityUser idi, foreign key'ler ise Guid

**Çözüm:** IdentityUser<Guid>, IdentityRole<Guid>, IdentityDbContext<..., Guid>

### Sorun 2: Register 500: "Role USER does not exist"

**Sebep:** Rol veritabanında yoktu. AddToRoleAsync exception atar, user ise zaten veritabanına yazılmış ve rolsüz kalmıştı

**Çözüm:** DbSeeder ile rollerin oluşturulması, rolsüz kalan user'ların silinmesi

### Sorun 3: Login 500: "Expires must be after NotBefore"

**Sebep:** ExpireMinutes okunmuyordu, değer 0 çıkıyordu

**Çözüm:** appsettings.json'da anahtarın adını düzelttim

### Sorun 4: Add-Migration konsolda durdu

**Sebep:** NuGet güvenlik uyarısı (NU1902) konsolda hata olarak algılandı

**Çözüm:** System.Security.Cryptography.Pkcs paketini güncelledim

### Sorun 5: Program açılmadı: "validator UNKNOWN"

**Sebep:** Validator'lar DI'a kaydedilmemişti

**Çözüm:** AddValidatorsFromAssemblyContaining<Program>()

### Sorun 6: Rate limit herkes için ortak sayaçtı, status 503 dönüyordu

**Sebep:** Normal policy tek bir ortak sayaçtır, RejectionStatusCode verilmemişti

**Çözüm:** IP bazında AddPolicy ve 429 kodu

## Kurulum

### Gereksinimler
- .NET 10 SDK
- Visual Studio (.NET 10'u destekleyen güncel sürüm)
- SQL Server LocalDB (Visual Studio ile birlikte gelir, yalnızca Windows)

### Adımlar

**1. Projeyi klonlayın** ve `TicketSistemi.slnx` dosyasını Visual Studio'da açın:

```powershell
git clone https://github.com/tbabayev37-t/Ticket-System.git
```

**2.** **Tools > NuGet Package Manager > Package Manager Console** menüsünü açın ve **Default project** olarak `TicketSistemi` seçili olduğundan emin olun.

**3. Gizli ayarları kaydedin**:

```powershell
dotnet user-secrets set "Jwt:Key" "<en az 32 karakterlik rastgele bir metin>"
dotnet user-secrets set "AdminSeed:Email" "admin@example.com"
dotnet user-secrets set "AdminSeed:Password" "<güçlü bir parola, örn. Admin123!>"
dotnet user-secrets set "ConnectionStrings:SqlServer" "Server=(localdb)\MSSQLLocalDB;Database=TicketSystemDB;Trusted_Connection=True;TrustServerCertificate=True"
```

Admin parolası en az 6 karakter olmalı ve büyük harf, küçük harf, rakam ile özel karakter içermelidir.

**4. Veritabanını oluşturun:**

```powershell
Update-Database
```

**5. Uygulamayı çalıştırın:** F5 (veya Ctrl+F5).

### Çalıştırdıktan Sonra

- API dokümantasyonu (Scalar): `http://localhost:7070/scalar/v1` (adres farklıysa Visual Studio'nun açtığı adrese `/scalar/v1` ekleyin)
- İlk admin hesabı uygulama ilk açıldığında otomatik oluşturulur. Kullanıcı adı `admin`, parola ise 3. adımda girdiğiniz parolanızdır.
- `/auth/login` ile giriş yapın, dönen token'ı Scalar'daki **Bearer Token** alanına yapıştırın.
- Normal kullanıcı için `/auth/register` ile kayıt olup aynı şekilde giriş yapabilirsiniz.

Visual Studio kullanmıyorsanız aynı işlemler terminalde `dotnet ef database update` ve `dotnet run` komutlarıyla yapılabilir.

