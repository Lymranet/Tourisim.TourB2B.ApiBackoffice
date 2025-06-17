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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace TourManagementApi.Controllers
{
    public class ActivitiesWizardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<ActivitiesWizardController> _logger;

        public ActivitiesWizardController(
            ApplicationDbContext context, 
            IWebHostEnvironment environment,
            ILogger<ActivitiesWizardController> logger)
        {
            _context = context;
            _environment = environment;
            _logger = logger;
        }

        public IActionResult Index()
        {
            try
            {
                var activities = _context.Activities
                    .Include(a => a.Options)
                    .Select(a => new Activity
                    {
                        Id = a.Id,
                        Title = a.Title ?? string.Empty,
                        Description = a.Description ?? string.Empty,
                        Category = a.Category ?? string.Empty,
                        Subcategory = a.Subcategory ?? string.Empty,
                        Language = a.Language ?? string.Empty,
                        Label = a.Label ?? string.Empty,
                        Status = a.Status ?? "draft",
                        Options = a.Options ?? new List<Option>(),
                        CountryCode = a.CountryCode ?? string.Empty,
                        DestinationCode = a.DestinationCode ?? string.Empty,
                        DestinationName = a.DestinationName ?? string.Empty
                    })
                    .ToList();

                return View(activities);
            }
            catch (Exception ex)
            {
                // Hata durumunda boş liste döndür
                return View(new List<Activity>());
            }
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
                    Languages = activity.Languages,
                    ContactInfo = activity.ContactInfo != null ? new ContactInfoViewModel
                    {
                        Name = activity.ContactInfo.Name,
                        Role = activity.ContactInfo.Role,
                        Email = activity.ContactInfo.Email,
                        Phone = activity.ContactInfo.Phone
                    } : new ContactInfoViewModel(),
                    CoverImageUrl = activity.CoverImage,
                    PreviewImageUrl = activity.PreviewImage,
                    GalleryImageUrls = activity.GalleryImages,
                    VideoUrls = activity.VideoUrls,
                    Highlights = activity.Highlights,
                    Inclusions = activity.Inclusions,
                    Exclusions = activity.Exclusions,
                    ImportantInfo = activity.ImportantInfo,
                    Itinerary = activity.Itinerary,
                    CountryCode = activity.CountryCode,
                    DestinationCode = activity.DestinationCode,
                    DestinationName = activity.DestinationName
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
                return View(model);

            try
            {
                var activity = model.ActivityId.HasValue
                    ? await _context.Activities.FindAsync(model.ActivityId.Value)
                    : new Activity();

                if (activity == null)
                    return NotFound();

                // Temel bilgileri güncelle
                activity.Title = model.Title ?? string.Empty;
                activity.Category = model.Category ?? string.Empty;
                activity.Subcategory = model.Subcategory ?? string.Empty;
                activity.Description = model.Description ?? string.Empty;
                activity.Languages = model.Languages ?? new List<string>();

                // Medya işlemleri
                if (model.CoverImage != null)
                {
                    activity.CoverImage = await SaveImage(model.CoverImage, "cover");
                }

                if (model.PreviewImage != null)
                {
                    activity.PreviewImage = await SaveImage(model.PreviewImage, "preview");
                }

                if (model.GalleryImages != null && model.GalleryImages.Any())
                {
                    activity.GalleryImages = new List<string>();
                    foreach (var image in model.GalleryImages.Take(10)) // En fazla 10 galeri görseli
                    {
                        var imagePath = await SaveImage(image, "gallery");
                        if (!string.IsNullOrEmpty(imagePath))
                        {
                            activity.GalleryImages.Add(imagePath);
                        }
                    }
                }

                activity.VideoUrls = model.VideoUrls?.Where(url => !string.IsNullOrEmpty(url)).ToList() ?? new List<string>();

                // İletişim bilgileri
                activity.ContactInfo = new ContactInfo
                {
                    Name = model.ContactInfo.Name ?? string.Empty,
                    Role = model.ContactInfo.Role ?? string.Empty,
                    Email = model.ContactInfo.Email ?? string.Empty,
                    Phone = model.ContactInfo.Phone ?? string.Empty
                };

                // Trekksoft uyumlu yeni alanlar
                activity.Highlights = model.Highlights;
                activity.Inclusions = model.Inclusions?.Where(x => !string.IsNullOrEmpty(x)).ToList() ?? new List<string>();
                activity.Exclusions = model.Exclusions?.Where(x => !string.IsNullOrEmpty(x)).ToList() ?? new List<string>();
                activity.ImportantInfo = model.ImportantInfo?.Where(x => !string.IsNullOrEmpty(x)).ToList() ?? new List<string>();
                activity.Itinerary = model.Itinerary;

                // Destinasyon bilgileri
                activity.CountryCode = model.CountryCode;
                activity.DestinationCode = model.DestinationCode;
                activity.DestinationName = model.DestinationName;

                if (!model.ActivityId.HasValue)
                {
                    _context.Activities.Add(activity);
                }

                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Kayıt sırasında bir hata oluştu: " + ex.Message);
                return View(model);
            }
        }

        // 2. Adım: Lokasyon
        [HttpGet]
        public async Task<IActionResult> CreateLocation(int? id)
        {
            if (!id.HasValue)
            {
                return RedirectToAction(nameof(Index));
            }

            var activity = await _context.Activities
                .Include(a => a.MeetingPoints)
                .Include(a => a.RoutePoints)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (activity == null)
            {
                return NotFound();
            }

            var viewModel = new ActivityLocationViewModel
            {
                ActivityId = activity.Id,
                MeetingPoints = activity.MeetingPoints?.Select(mp => new MeetingPointViewModel
                {
                    Name = mp.Name,
                    Address = mp.Address,
                    Latitude = mp.Latitude,
                    Longitude = mp.Longitude
                }).ToList() ?? new List<MeetingPointViewModel>(),
                RoutePoints = activity.RoutePoints?.Select(rp => new RoutePointViewModel
                {
                    Name = rp.Name,
                    Latitude = rp.Latitude,
                    Longitude = rp.Longitude
                }).ToList() ?? new List<RoutePointViewModel>()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateLocation(Models.ViewModels.ActivityLocationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var activity = await _context.Activities
                    .Include(a => a.MeetingPoints)
                    .Include(a => a.RoutePoints)
                    .FirstOrDefaultAsync(a => a.Id == model.ActivityId);

                if (activity == null)
                {
                    return NotFound();
                }

                // Buluşma noktalarını güncelle
                activity.MeetingPoints.Clear();
                if (model.MeetingPoints != null)
                {
                    foreach (var mp in model.MeetingPoints)
                    {
                        activity.MeetingPoints.Add(new MeetingPoint
                        {
                            Name = mp.Name,
                            Address = mp.Address,
                            Latitude = mp.Latitude,
                            Longitude = mp.Longitude
                        });
                    }
                }

                // Rota noktalarını güncelle
                activity.RoutePoints.Clear();
                if (model.RoutePoints != null)
                {
                    foreach (var rp in model.RoutePoints)
                    {
                        activity.RoutePoints.Add(new RoutePoint
                        {
                            Name = rp.Name,
                            Latitude = rp.Latitude,
                            Longitude = rp.Longitude
                        });
                    }
                }

                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Kayıt sırasında bir hata oluştu: " + ex.Message);
                return View(model);
            }
        }

        // 3. Adım: Fiyatlandırma
        [HttpGet]
        public async Task<IActionResult> CreatePricing(int id)
        {
            var activity = await _context.Activities.FindAsync(id);
            if (activity == null)
            {
                return NotFound();
            }

            var viewModel = new ActivityPricingViewModel
            {
                ActivityId = activity.Id,
                Pricing = activity.Pricing ?? new ActivityPricing()
            };

            return View(viewModel);
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> CreatePricing(ActivityPricingViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var activity = await _context.Activities.FindAsync(model.ActivityId);
                if (activity == null)
                {
                    return NotFound();
                }

                activity.Pricing = model.Pricing;
                await _context.SaveChangesAsync();

                return RedirectToAction("CreateTime", new { id = activity.Id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Kayıt sırasında bir hata oluştu: " + ex.Message);
                return View(model);
            }
        }

        // 4. Adım: Zaman Yönetimi
        [HttpGet]
        public async Task<IActionResult> CreateTime(int? id)
        {
            if (id.HasValue)
            {
                var activity = await _context.Activities.FindAsync(id.Value);
                if (activity == null) return NotFound();
                var vm = new ActivityTimeViewModel
                {
                    ActivityId = activity.Id,
                    Duration = activity.Duration,
                    SeasonalAvailability = activity.SeasonalAvailability != null ? new SeasonalAvailabilityViewModel
                    {
                        StartDate = DateTime.TryParse(activity.SeasonalAvailability.StartDate, out var sd) ? sd : (DateTime?)null,
                        EndDate = DateTime.TryParse(activity.SeasonalAvailability.EndDate, out var ed) ? ed : (DateTime?)null
                    } : new SeasonalAvailabilityViewModel(),
                    TimeSlots = activity.TimeSlots.Select(ts => new TimeSlotViewModel
                    {
                        StartTime = ts.StartTime,
                        EndTime = ts.EndTime,
                        DaysOfWeek = ts.DaysOfWeek
                    }).ToList()
                };
                return View(vm);
            }
            return View(new ActivityTimeViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTime(ActivityTimeViewModel model)
        {
            if (!ModelState.IsValid)
                return View(vm);

            try
            {
                var activity = await _context.Activities.FindAsync(vm.ActivityId);
                if (activity == null)
                    return NotFound();

                activity.Duration = vm.Duration;
                activity.SeasonalAvailability = new SeasonalAvailability
                {
                    StartDate = vm.SeasonalAvailability.StartDate?.ToString("yyyy-MM-dd"),
                    EndDate = vm.SeasonalAvailability.EndDate?.ToString("yyyy-MM-dd")
                };

                activity.TimeSlots = vm.TimeSlots.Select(ts => new TimeSlot
                {
                    StartTime = ts.StartTime,
                    EndTime = ts.EndTime,
                    DaysOfWeek = ts.DaysOfWeek
                }).ToList();

                await _context.SaveChangesAsync();
                return RedirectToAction("Index", new { id = activity.Id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Zaman bilgileri kaydedilirken bir hata oluştu: " + ex.Message);
                return View(vm);
            }
        }

        // 5. Adım: Misafir Tanımlamaları
        // GET: ActivitiesWizard/CreateGuestFields/5
        /*
        [HttpGet]
        public async Task<IActionResult> CreateGuestFields(int? id)
        {
            if (id == null)
                return NotFound();

            var activity = await _context.Activities.FindAsync(id.Value);
            if (activity == null)
                return NotFound();

            var vm = new GuestFieldsViewModel
            {
                ActivityId = activity.Id,
                MinParticipants = activity.MinParticipants,
                MaxParticipants = activity.MaxParticipants,
                GuestFields = activity.GuestFields
            };

            return View(vm);
        }

        // POST: ActivitiesWizard/CreateGuestFields
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
            if (file == null || file.Length == 0)
                return null;

            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"{prefix}_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return uniqueFileName;
        }

        private async Task<string> SaveImage(IFormFile file, string folder)
        {
            if (file == null || file.Length == 0)
                return null;

            try
            {
                // Dosya uzantısını kontrol et
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (extension != ".jpg" && extension != ".jpeg" && extension != ".png" && extension != ".gif")
                {
                    throw new Exception("Geçersiz dosya formatı. Sadece JPG, JPEG, PNG ve GIF dosyaları yüklenebilir.");
                }

                // Klasör yolunu oluştur
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", folder);
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Benzersiz dosya adı oluştur
                var uniqueFileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Dosyayı kaydet
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // URL yolunu döndür
                return $"/uploads/{folder}/{uniqueFileName}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Dosya yüklenirken hata oluştu");
                throw new Exception("Dosya yüklenirken bir hata oluştu: " + ex.Message);
            }
        }
    }
} 