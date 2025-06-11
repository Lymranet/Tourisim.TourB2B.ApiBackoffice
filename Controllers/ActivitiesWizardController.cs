using Microsoft.AspNetCore.Mvc;
using TourManagementApi.Data;
using TourManagementApi.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TourManagementApi.Models.ViewModels;
using System.Linq;
using System;
using TourManagementApi.Models.Common;
using System.Collections.Generic;
using System.IO;

namespace TourManagementApi.Controllers
{
    public class ActivitiesWizardController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ActivitiesWizardController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. Adım: Temel Bilgiler (Basic Information)
        [HttpGet]
        public async Task<IActionResult> CreateBasic(int? id)
        {
            if (id.HasValue)
            {
                var activity = await _context.Activities.FindAsync(id.Value);
                if (activity == null) return NotFound();
                var vm = new ActivityBasicViewModel
                {
                    ActivityId = activity.Id,
                    Title = activity.Title,
                    Category = activity.Category,
                    Subcategory = activity.Subcategory,
                    Description = activity.Description,
                    Status = activity.Status,
                    Languages = activity.Languages,
                    ContactInfo = activity.ContactInfo != null ? new ContactInfoViewModel
                    {
                        Name = activity.ContactInfo.Name,
                        Role = activity.ContactInfo.Role,
                        Email = activity.ContactInfo.Email,
                        Phone = activity.ContactInfo.Phone
                    } : new ContactInfoViewModel(),
                    CoverImageUrl = activity.Media?.Images?.Header,
                    PreviewImageUrl = activity.Media?.Images?.Teaser,
                    GalleryImageUrls = activity.Media?.Images?.Gallery,
                    VideoUrls = activity.Media?.Videos ?? new List<string>()
                };
                return View(vm);
            }
            return View(new ActivityBasicViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBasic(ActivityBasicViewModel model, string VideoUrls)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .Select(x => new { x.Key, Errors = x.Value.Errors.Select(e => e.ErrorMessage).ToList() })
                    .ToList();
                Console.WriteLine("ModelState Errors: " + Newtonsoft.Json.JsonConvert.SerializeObject(errors));
                return View(model);
            }

            Activity activity;
            if (model.ActivityId.HasValue)
            {
                // Güncelleme
                activity = await _context.Activities.FindAsync(model.ActivityId.Value);
                if (activity == null) return NotFound();
            }
            else
            {
                // Yeni kayıt
                activity = new Activity();
                _context.Activities.Add(activity);
            }
            // Alanları güncelle
            activity.Title = model.Title;
            activity.Category = model.Category;
            activity.Subcategory = model.Subcategory;
            activity.Description = model.Description;
            activity.Status = model.Status;
            activity.Languages = model.Languages;
            activity.ContactInfo = new ContactInfo
            {
                Name = model.ContactInfo.Name,
                Role = model.ContactInfo.Role,
                Email = model.ContactInfo.Email,
                Phone = model.ContactInfo.Phone
            };

            // Medya yükleme
            var uploadsRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploadsRoot)) Directory.CreateDirectory(uploadsRoot);
            activity.Media ??= new Media();
            // Kapak
            if (model.CoverImage != null && model.CoverImage.Length > 0)
            {
                var fileName = $"cover_{Guid.NewGuid()}{Path.GetExtension(model.CoverImage.FileName)}";
                var filePath = Path.Combine(uploadsRoot, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.CoverImage.CopyToAsync(stream);
                }
                activity.Media.Images.Header = "/uploads/" + fileName;
            }
            // Önizleme
            if (model.PreviewImage != null && model.PreviewImage.Length > 0)
            {
                var fileName = $"preview_{Guid.NewGuid()}{Path.GetExtension(model.PreviewImage.FileName)}";
                var filePath = Path.Combine(uploadsRoot, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.PreviewImage.CopyToAsync(stream);
                }
                activity.Media.Images.Teaser = "/uploads/" + fileName;
            }
            // Galeri
            if (model.GalleryImages != null && model.GalleryImages.Count > 0)
            {
                activity.Media.Images.Gallery = new List<string>();
                foreach (var img in model.GalleryImages)
                {
                    if (img != null && img.Length > 0)
                    {
                        var fileName = $"gallery_{Guid.NewGuid()}{Path.GetExtension(img.FileName)}";
                        var filePath = Path.Combine(uploadsRoot, fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await img.CopyToAsync(stream);
                        }
                        activity.Media.Images.Gallery.Add("/uploads/" + fileName);
                    }
                }
            }
            // Video URL'ler
            if (!string.IsNullOrWhiteSpace(VideoUrls))
            {
                activity.Media.Videos = VideoUrls.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
            }
            else
            {
                activity.Media.Videos = new List<string>();
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // 2. Adım: Lokasyon
        [HttpGet]
        public async Task<IActionResult> CreateLocation(int id)
        {
            var activity = await _context.Activities.FindAsync(id);
            if (activity == null) return NotFound();
            var vm = new ActivityLocationViewModel
            {
                ActivityId = activity.Id,
                Address = activity.Location?.Address,
                City = activity.Location?.City,
                Country = activity.Location?.Country,
                Latitude = activity.Location?.Coordinates?.Latitude,
                Longitude = activity.Location?.Coordinates?.Longitude
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateLocation(Models.ViewModels.ActivityLocationViewModel model)
        {
            try
            {
                if (!ModelState.IsValid || !model.Latitude.HasValue || !model.Longitude.HasValue)
                {
                    if (!model.Latitude.HasValue || !model.Longitude.HasValue)
                        ModelState.AddModelError("", "Lütfen haritadan bir konum seçin.");
                    return View(model);
                }
                var activity = await _context.Activities
                    .Include(a => a.Location)
                    .ThenInclude(l => l.Coordinates)
                    .FirstOrDefaultAsync(a => a.Id == model.ActivityId);
                if (activity == null) return NotFound();
                if (activity.Location == null)
                {
                    activity.Location = new Location
                    {
                        Address = model.Address ?? string.Empty,
                        City = model.City ?? string.Empty,
                        Country = model.Country ?? string.Empty,
                        Coordinates = new Coordinates
                        {
                            Latitude = model.Latitude ?? 0,
                            Longitude = model.Longitude ?? 0
                        }
                    };
                }
                else
                {
                    activity.Location.Address = model.Address ?? string.Empty;
                    activity.Location.City = model.City ?? string.Empty;
                    activity.Location.Country = model.Country ?? string.Empty;
                    if (activity.Location.Coordinates == null)
                    {
                        activity.Location.Coordinates = new Coordinates();
                    }
                    activity.Location.Coordinates.Latitude = model.Latitude ?? 0;
                    activity.Location.Coordinates.Longitude = model.Longitude ?? 0;
                }
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine("CreateLocation Exception: " + ex.ToString());
                ModelState.AddModelError("", "Bir hata oluştu: " + ex.Message);
                return View(model);
            }
        }

        // 3. Adım: Fiyatlandırma
        [HttpGet]
        public async Task<IActionResult> CreatePricing(int id)
        {
            var activity = await _context.Activities.Include(a => a.Pricing).FirstOrDefaultAsync(a => a.Id == id);
            if (activity == null) return NotFound();
            var vm = new ActivityPricingViewModel
            {
                ActivityId = activity.Id,
                DefaultCurrency = activity.Pricing?.DefaultCurrency ?? "TRY",
                TaxRate = activity.Pricing?.TaxRate ?? 18,
                Categories = activity.Pricing?.Categories?.Select(c => new PriceCategoryViewModel
                {
                    Type = c.Type,
                    PriceType = c.PriceType,
                    Amount = c.Amount,
                    Currency = c.Currency,
                    MinAge = c.MinAge,
                    MaxAge = c.MaxAge,
                    Description = c.Description,
                    MinParticipants = c.MinParticipants,
                    MaxParticipants = c.MaxParticipants,
                    DiscountType = c.DiscountType,
                    DiscountValue = c.DiscountValue
                }).ToList() ?? new List<PriceCategoryViewModel>(),
                Included = activity.Included ?? new List<string>(),
                Excluded = activity.Excluded ?? new List<string>(),
                Requirements = activity.Requirements ?? new List<string>(),
                CancellationPolicy = activity.CancellationPolicy,
                AdditionalNotes = activity.AdditionalNotes
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePricing(ActivityPricingViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var activity = await _context.Activities.Include(a => a.Pricing).FirstOrDefaultAsync(a => a.Id == model.ActivityId);
            if (activity == null) return NotFound();
            // Fiyatlandırma
            if (activity.Pricing == null)
                activity.Pricing = new ActivityPricing();
            activity.Pricing.DefaultCurrency = model.DefaultCurrency;
            activity.Pricing.TaxRate = model.TaxRate;
            activity.Pricing.Categories = model.Categories?.Select(c => new PriceCategory
            {
                Type = c.Type,
                PriceType = c.PriceType,
                Amount = c.Amount,
                Currency = c.Currency,
                MinAge = c.MinAge,
                MaxAge = c.MaxAge,
                Description = c.Description,
                MinParticipants = c.MinParticipants,
                MaxParticipants = c.MaxParticipants,
                DiscountType = c.DiscountType,
                DiscountValue = c.DiscountValue
            }).ToList() ?? new List<PriceCategory>();
            // Servisler ve gereksinimler
            activity.Included = model.Included ?? new List<string>();
            activity.Excluded = model.Excluded ?? new List<string>();
            activity.Requirements = model.Requirements ?? new List<string>();
            // Ek bilgi
            activity.CancellationPolicy = model.CancellationPolicy;
            activity.AdditionalNotes = model.AdditionalNotes;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // 4. Adım: Zaman Yönetimi
        [HttpGet]
        public async Task<IActionResult> CreateTime(int id)
        {
            var activity = await _context.Activities.Include(a => a.TimeSlots).FirstOrDefaultAsync(a => a.Id == id);
            if (activity == null) return NotFound();
            var vm = new ActivityTimeViewModel
            {
                ActivityId = activity.Id,
                Duration = activity.Duration,
                SeasonalAvailability = new SeasonalAvailabilityViewModel
                {
                    StartDate = DateTime.TryParse(activity.SeasonalAvailability?.StartDate, out var sd) ? sd : (DateTime?)null,
                    EndDate = DateTime.TryParse(activity.SeasonalAvailability?.EndDate, out var ed) ? ed : (DateTime?)null
                },
                TimeSlots = activity.TimeSlots?.Select(ts => new TimeSlotViewModel
                {
                    StartTime = ts.StartTime,
                    EndTime = ts.EndTime,
                    DaysOfWeek = ts.DaysOfWeek?.ToList() ?? new List<string>()
                }).ToList() ?? new List<TimeSlotViewModel>()
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTime(ActivityTimeViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var activity = await _context.Activities.Include(a => a.TimeSlots).FirstOrDefaultAsync(a => a.Id == model.ActivityId);
            if (activity == null) return NotFound();
            // Süre
            activity.Duration = model.Duration;
            // Sezon
            if (model.SeasonalAvailability != null)
            {
                activity.SeasonalAvailability ??= new SeasonalAvailability();
                activity.SeasonalAvailability.StartDate = model.SeasonalAvailability.StartDate?.ToString("yyyy-MM-dd");
                activity.SeasonalAvailability.EndDate = model.SeasonalAvailability.EndDate?.ToString("yyyy-MM-dd");
            }
            // Zaman dilimleri
            activity.TimeSlots = model.TimeSlots?.Select(ts => new Models.Common.TimeSlot
            {
                StartTime = ts.StartTime,
                EndTime = ts.EndTime,
                DaysOfWeek = ts.DaysOfWeek?.ToList() ?? new List<string>()
            }).ToList() ?? new List<Models.Common.TimeSlot>();
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // 5. Adım: Buluşma Noktaları
        [HttpGet]
        public async Task<IActionResult> CreateMeetingPoints(int id)
        {
            var activity = await _context.Activities.Include(a => a.MeetingPoints).FirstOrDefaultAsync(a => a.Id == id);
            if (activity == null) return NotFound();
            var vm = new ActivityMeetingPointsViewModel
            {
                ActivityId = activity.Id,
                MeetingPoints = activity.MeetingPoints?.Select(mp => new MeetingPointViewModel
                {
                    Name = mp.Name,
                    Address = mp.Address,
                    Latitude = mp.Latitude,
                    Longitude = mp.Longitude
                }).ToList() ?? new List<MeetingPointViewModel>()
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateMeetingPoints(ActivityMeetingPointsViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var activity = await _context.Activities.Include(a => a.MeetingPoints).FirstOrDefaultAsync(a => a.Id == model.ActivityId);
            if (activity == null) return NotFound();
            activity.MeetingPoints = model.MeetingPoints?.Select(mp => new MeetingPoint
            {
                Name = mp.Name,
                Address = mp.Address,
                Latitude = mp.Latitude,
                Longitude = mp.Longitude
            }).ToList() ?? new List<MeetingPoint>();
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // Ek Ürünler (Addons) Adımı
        [HttpGet]
        public async Task<IActionResult> CreateAddons(int id)
        {
            var activity = await _context.Activities.Include(a => a.Addons).FirstOrDefaultAsync(a => a.Id == id);
            if (activity == null) return NotFound();
            var vm = new ActivityAddonsViewModel
            {
                ActivityId = activity.Id,
                Addons = activity.Addons?.Select(a => new AddonViewModel
                {
                    Id = a.Id,
                    Title = a.Title,
                    Type = a.Type,
                    Description = a.Description,
                    Price = new AddonPriceViewModel
                    {
                        Amount = a.Price?.Amount ?? "0",
                        Currency = a.Price?.Currency ?? "TRY"
                    },
                    Translations = a.Translations?.Select(t => new AddonTranslationViewModel
                    {
                        Language = t.Language,
                        Title = t.Title,
                        Description = t.Description
                    }).ToList() ?? new List<AddonTranslationViewModel>()
                }).ToList() ?? new List<AddonViewModel>()
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAddons(ActivityAddonsViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);
            var activity = await _context.Activities.Include(a => a.Addons).FirstOrDefaultAsync(a => a.Id == model.ActivityId);
            if (activity == null) return NotFound();
            activity.Addons = model.Addons?.Select(a => new Addon
            {
                Id = a.Id ?? Guid.NewGuid().ToString(),
                Title = a.Title,
                Type = a.Type,
                Description = a.Description,
                Price = new AddonPrice
                {
                    Amount = a.Price?.Amount ?? "0",
                    Currency = a.Price?.Currency ?? "TRY"
                },
                Translations = a.Translations?.Select(t => new AddonTranslation
                {
                    Language = t.Language,
                    Title = t.Title,
                    Description = t.Description
                }).ToList() ?? new List<AddonTranslation>()
            }).ToList() ?? new List<Addon>();
            await _context.SaveChangesAsync();
            return RedirectToAction("CreateLocation", new { id = model.ActivityId });
        }

        // Tur Listesi (Index)
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var activities = await _context.Activities.ToListAsync();
            return View(activities);
        }

        // Diğer adımlar için şablonlar...
    }
} 