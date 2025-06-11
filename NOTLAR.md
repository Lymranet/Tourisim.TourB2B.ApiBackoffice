# Aktivite Oluşturma Formu - Adım Adım Akış ve Inputlar

Bu dosya, Tour Management projesindeki klasik ASP.NET MVC uyumlu step-by-step (çok adımlı) aktivite oluşturma formunun tüm adımlarını, inputlarını ve akışını özetler. Amaç, ileride referans almak ve yapılanları kaybetmektir.

---

## Genel Akış
Form 6 ana adımdan oluşur. Her adımda farklı bilgiler toplanır ve kullanıcı adımlar arasında ileri/geri gidebilir. Son adımda tüm bilgiler özetlenir ve kayıt işlemi yapılır.

### 1. Adım: Temel Bilgiler (Basic Information)
- **Başlık (title)**: Metin kutusu
- **Kategori (category)**: Dropdown
- **Alt Kategori (subcategory)**: Dropdown (kategoriye göre dinamik)
- **Açıklama (description)**: Çok satırlı metin
- **Diller (languages[])**: Birden fazla dil seçilebilir (dinamik ekleme/çıkarma)
- **İletişim Bilgileri (contactInfo)**
  - İsim (contactInfo.name)
  - Rol (contactInfo.role)
  - E-posta (contactInfo.email)
  - Telefon (contactInfo.phone)

### 2. Adım: Lokasyon & İletişim (Location & Contact)
- **Adres arama**: Adres arama ve harita üzerinde gösterme
- **Adres (location.address)**
- **Şehir (location.city)**
- **Ülke (location.country)**
- **Enlem/Boylam (location.latitude, location.longitude)**: Harita üzerinden otomatik alınır

### 3. Adım: Fiyatlandırma (Pricing)
- **Varsayılan Para Birimi (pricing.defaultCurrency)**
- **KDV Oranı (pricing.taxRate)**
- **Fiyat Kategorileri (priceCategories[])**: Birden fazla kategori eklenebilir
  - Kategori Tipi (type)
  - Fiyat Tipi (priceType)
  - Tutar (amount)
  - Açıklama (description)
- **Dahil/Haric Servisler (included[], excluded[])**: Dinamik eklenebilir
- **Gereksinimler (requirements[])**: Dinamik eklenebilir
- **İptal Politikası (cancellationPolicy)**
- **Ek Notlar (additionalNotes)**

### 4. Adım: Zaman Yönetimi (Time Management)
- **Süre (duration)**: Dakika cinsinden
- **Sezon Bilgileri (seasonalAvailability)**
  - Başlangıç Tarihi (startDate)
  - Bitiş Tarihi (endDate)
- **Zaman Dilimleri (timeSlots[])**: Birden fazla zaman dilimi eklenebilir (dinamik)

### 5. Adım: Buluşma Noktaları (Meeting Points)
- **Buluşma Noktası Ekleme**: Modal ile harita üzerinden adres seçimi
- **Buluşma Noktası Bilgileri**
  - Adı (meetingPointName)
  - Adres (meetingPointAddress)
  - Enlem/Boylam (meetingPointLat, meetingPointLng)
- Birden fazla buluşma noktası eklenebilir

### 6. Adım: Dil & Önizleme (Language & Preview)
- **Durum (status)**: Taslak, Yayında, Pasif
- **Önizleme**: Girilen tüm bilgilerin özetlenmiş hali

---

## Teknik Notlar
- Form klasik ASP.NET MVC form yapısı ile uyumludur.
- Dinamik alanlar (diller, fiyat kategorileri, zaman dilimleri, buluşma noktaları) JavaScript ile eklenip çıkarılır.
- Her adım bir div içinde, sadece aktif adım görünür.
- Son adımda tüm bilgiler özetlenir ve submit edilir.
- API controller ileride dış sistemlerle entegrasyon için kullanılacaktır.

---

## Geliştirme Planı (Özet)
1. Adım adım formun her adımını klasik MVC ile çalışacak şekilde düzenle.
2. Dinamik alanların backend'e doğru şekilde model bind edilmesini sağla.
3. Kayıt işlemi tamamlandığında kullanıcıyı bilgilendir.
4. API controller'ı şimdilik koru, ileride dış sistemlerle entegrasyon için kullan.

---

> Bu dosya, yapılan işleri ve formun akışını unutmamak için referans olarak tutulacaktır. 