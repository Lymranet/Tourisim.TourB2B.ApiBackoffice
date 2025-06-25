using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using TourManagementApi.Data;
using TourManagementApi.Helper;
using TourManagementApi.Models;
using TourManagementApi.Models.Api;
using TourManagementApi.Models.Common;
using TourManagementApi.Models.ViewModels;
using Addon = TourManagementApi.Models.Addon;

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
                return View(new List<Activity>());
            }
        }

        [HttpPatch]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] StatusUpdateDto dto)
        {
            var activity = await _context.Activities.FindAsync(id);
            if (activity == null)
            {
                return NotFound();
            }

            activity.Status = dto.Status;
            _context.Update(activity);
            await _context.SaveChangesAsync();
            return Ok();
        }


        // 1. Adım: Temel Bilgiler (Basic Information)
        [HttpGet]
        [IgnoreAntiforgeryToken]

        public async Task<IActionResult> CreateBasic(int? id)
        {
            try
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
                        ContactInfo = activity.ContactInfoName != null ? new ContactInfoViewModel
                        {
                            Name = activity.ContactInfoName,
                            Role = activity.ContactInfoRole,
                            Email = activity.ContactInfoEmail,
                            Phone = activity.ContactInfoPhone
                        } : new ContactInfoViewModel(),
                        CoverImageUrl = activity.CoverImage,
                        PreviewImageUrl = activity.PreviewImage,
                        GalleryImageUrls = TxtJson.DeserializeStringList(activity.GalleryImages),
                        ExistingGalleryImages = TxtJson.DeserializeStringList(activity.GalleryImages),
                        VideoUrls = TxtJson.DeserializeStringList(activity.MediaVideos),
                        Highlights = activity.Highlights,
                        Inclusions = TxtJson.DeserializeStringList(activity.Inclusions),
                        Exclusions = TxtJson.DeserializeStringList(activity.Exclusions),
                        ImportantInfo = TxtJson.DeserializeStringList(activity.ImportantInfo),
                        Itinerary = activity.Itinerary,
                        CountryCode = activity.CountryCode,
                        DestinationCode = activity.DestinationCode,
                        DestinationName = activity.DestinationName
                    };
                    return View(vm);
                }
            }
            catch (Exception ex)
            {

                throw;
            }

            return View(new ActivityBasicViewModel());
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        [DisableRequestSizeLimit]
        [RequestFormLimits(MultipartBodyLengthLimit = 1024 * 1024 * 100)]
        public async Task<IActionResult> CreateBasic(ActivityBasicViewModel model)
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
                var finalGalleryList = new List<string>();
                if (model.ExistingGalleryImages != null)
                {
                    finalGalleryList.AddRange(model.ExistingGalleryImages);
                }
                activity.Title = model.Title ?? string.Empty;
                activity.Category = model.Category ?? string.Empty;
                activity.Subcategory = model.Subcategory ?? string.Empty;
                activity.Description = model.Description ?? string.Empty;
                activity.DetailsUrl = "";
                activity.PartnerSupplierId = "";
                if (model.CoverImage != null)
                {
                    activity.CoverImage = await FileHelper.SaveImage(model.CoverImage, "cover", _environment, _logger);
                }

                if (model.PreviewImage != null)
                {
                    activity.PreviewImage = await FileHelper.SaveImage(model.PreviewImage, "preview", _environment, _logger);
                }

                if (model.GalleryImages != null && model.GalleryImages.Any())
                {
                    foreach (var image in model.GalleryImages.Take(10))
                    {

                        //var imageUrl = await SaveResizedImageAsync(file, Path.Combine(_environment.WebRootPath, "uploads", "gallery"));
                        var imagePath = await FileHelper.SaveImage(image, "gallery", _environment, _logger);
                        if (!string.IsNullOrEmpty(imagePath))
                        {
                            finalGalleryList.Add(imagePath);
                        }
                    }
                    activity.GalleryImages = JsonSerializer.Serialize(finalGalleryList);
                }

                activity.MediaVideos = JsonSerializer.Serialize(model.VideoUrls?.Where(url => !string.IsNullOrWhiteSpace(url)).ToList() ?? new List<string>());

                activity.ContactInfoName = model.ContactInfo.Name ?? string.Empty;
                activity.ContactInfoRole = model.ContactInfo.Role ?? string.Empty;
                activity.ContactInfoEmail = model.ContactInfo.Email ?? string.Empty;
                activity.ContactInfoPhone = model.ContactInfo.Phone ?? string.Empty;

                activity.Highlights = model.Highlights ?? string.Empty;
                activity.Inclusions = JsonSerializer.Serialize(model.Inclusions?.Where(x => !string.IsNullOrWhiteSpace(x)).ToList() ?? new List<string>());
                activity.Exclusions = JsonSerializer.Serialize(model.Exclusions?.Where(x => !string.IsNullOrWhiteSpace(x)).ToList() ?? new List<string>());
                activity.ImportantInfo = JsonSerializer.Serialize(model.ImportantInfo?.Where(x => !string.IsNullOrWhiteSpace(x)).ToList() ?? new List<string>());
                activity.Itinerary = model.Itinerary;

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

        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> DeleteGalleryImage([FromBody] DeleteGalleryImageRequest request)
        {
            var activity = await _context.Activities.FindAsync(request.ActivityId);
            if (activity == null || string.IsNullOrWhiteSpace(activity.GalleryImages))
                return Json(new { success = false, message = "Aktivite bulunamadı." });

            var galleryList = TxtJson.DeserializeStringList(activity.GalleryImages);

            var filename = Path.GetFileName(request.ImagePath);
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "gallery", filename);

            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }

            galleryList.RemoveAll(img => img.EndsWith(filename));
            activity.GalleryImages = TxtJson.SerializeStringList(galleryList);

            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }



        [HttpPost]
        public async Task<IActionResult> DeleteCoverOrPreviewImage([FromBody] DeleteImageRequest request)
        {
            var activity = await _context.Activities.FindAsync(request.ActivityId);
            if (activity == null)
                return Json(new { success = false, message = "Aktivite bulunamadı." });

            string? imagePath = null;
            string? folder = null;

            if (request.ImageType == "cover" && !string.IsNullOrEmpty(activity.CoverImage))
            {
                imagePath = activity.CoverImage;
                folder = "cover";
                activity.CoverImage = null;
            }
            else if (request.ImageType == "preview" && !string.IsNullOrEmpty(activity.PreviewImage))
            {
                imagePath = activity.PreviewImage;
                folder = "preview";
                activity.PreviewImage = null;
            }

            if (imagePath != null)
            {
                var filename = Path.GetFileName(imagePath);
                var fullPath = Path.Combine(_environment.WebRootPath, "uploads", folder, filename);
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }

                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }

            return Json(new { success = false, message = "Silinecek görsel bulunamadı." });
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
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> CreateLocation(ActivityLocationViewModel model)
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
                Pricing = new ActivityPricing
                {
                    DefaultCurrency = activity.PricingDefaultCurrency,
                    TaxIncluded = activity.PricingTaxIncluded ?? false,
                    TaxRate = activity.PricingTaxRate ?? 0
                }
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

                activity.PricingDefaultCurrency = model.Pricing.DefaultCurrency;
                activity.PricingTaxIncluded = model.Pricing.TaxIncluded;
                activity.PricingTaxRate = model.Pricing.TaxRate;

                _context.PriceCategories.RemoveRange(activity.PriceCategories);

                foreach (var category in model.Categories)
                {
                    activity.PriceCategories.Add(new PriceCategory
                    {
                        Type = category.Type,
                        PriceType = category.PriceType,
                        Amount = category.Amount,
                        Currency = category.Currency,
                        Description = category.Description,
                        MinAge = category.MinAge,
                        MaxAge = category.MaxAge,
                        MinParticipants = category.MinParticipants,
                        MaxParticipants = category.MaxParticipants,
                        DiscountType = category.DiscountType,
                        DiscountValue = category.DiscountValue,
                        ActivityPricingActivityId = activity.Id
                    });
                }

                await _context.SaveChangesAsync();

                return RedirectToAction("CreateTime", new { id = activity.Id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Kayıt sırasında bir hata oluştu: " + ex.Message);
                return View(model);
            }
        }

        public async Task<IActionResult> AddonsIndex()
        {
            var activitiesWithAddons = await _context.Activities
                .Include(a => a.Addons)
                .Where(a => a.Addons != null && a.Addons.Any())
                .ToListAsync();

            return View(activitiesWithAddons);
        }

        [HttpGet]
        public async Task<IActionResult> CreateAddons(int id)
        {
            var addOns = await _context.Addons.Where(a=>a.ActivityId==id).ToListAsync();
            if (addOns == null) return NotFound();
            var vm = new ActivityAddonsViewModel
            {
                ActivityId = id,
                Addons = addOns?.Select(a => new AddonViewModel
                {
                    Id = a.Id,
                    Title = a.Title,
                    Type = a.Type,
                    Description = a.Description,
                    PriceAmount = a.PriceAmount,
                    Currency = a.Currency ?? "TRY",
                    Translations = a.AddonTranslations?.Select(t => new AddonTranslationViewModel
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

            var activity = await _context.Activities
                .Include(a => a.Addons)
                .FirstOrDefaultAsync(a => a.Id == model.ActivityId);

            if (activity == null) return NotFound();

            activity.Addons = model.Addons?.Select(a => new Addon
            {
                Id = a.Id,
                Title = a.Title,
                Type = a.Type,
                Description = a.Description,
                PriceAmount = a.PriceAmount ?? 0, // varsayılan
                Currency = a.Currency ?? "TRY",
                AddonTranslations = a.Translations?.Select(t => new AddonTranslation
                {
                    Language = t.Language,
                    Title = t.Title,
                    Description = t.Description
                }).ToList() ?? new()
            }).ToList() ?? new();

            await _context.SaveChangesAsync();

            return RedirectToAction("CreateLocation", new { id = model.ActivityId });
        }


        #region Translation Alanı
        // GET: translation yönetimi sayfası
        [HttpGet]
        public async Task<IActionResult> ManageLanguages(int id)
        {
            var activity = await _context.Activities
                .Include(a => a.ActivityLanguages)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (activity == null)
                return NotFound();

            var existingLangs = await _context.ActivityLanguages
                .Where(x => x.ActivityId == id)
                .Select(x => x.LanguageCode)
                .ToListAsync();

            var model = new TourTranslationViewModel
            {
                ActivityId = id,
                Title = activity.Title,
                ExistingLanguages = existingLangs
            };

            return View(model);
        }

        // GET: Yeni dil ekleme formu
        [HttpGet]
        public async Task<IActionResult> AddLanguage(int activityId, string? languageCode = null)
        {
            var activity = await _context.Activities.FindAsync(activityId);
            if (activity == null) return NotFound();

            var existingLanguages = await _context.ActivityLanguages
                .Where(x => x.ActivityId == activityId)
                .Select(x => x.LanguageCode)
                .ToListAsync();

            var allLanguages = new Dictionary<string, string>
                {
                    { "en", "İngilizce" },
                    { "de", "Almanca" },
                    { "fr", "Fransızca" },
                    { "es", "İspanyolca" },
                    { "it", "İtalyanca" },
                    { "ru", "Rusça" },
                    { "ar", "Arapça" },
                    { "zh", "Çince" }
                };

            // Eğer düzenleme için geldiyse dil zaten eklenmiştir
            Dictionary<string, string> languageOptions = (languageCode != null)
                ? new Dictionary<string, string> { { languageCode, allLanguages[languageCode] } }
                : allLanguages
                    .Where(x => !existingLanguages.Contains(x.Key))
                    .ToDictionary(x => x.Key, x => x.Value);

            ViewBag.LanguageOptions = languageOptions;

            var translation = await _context.Translations
                .FirstOrDefaultAsync(t => t.ActivityId == activityId && t.Language == languageCode);

            var model = new AddLanguageViewModel
            {
                ActivityId = activityId,
                LanguageCode = languageCode ?? string.Empty,
                Original = new ActivityTranslationDTO
                {
                    Title = activity.Title,
                    Description = activity.Description,
                    Highlights = activity.Highlights,
                    Itinerary = activity.Itinerary,
                    Inclusions = string.IsNullOrWhiteSpace(activity.Inclusions)
        ? new List<string>()
        : JsonSerializer.Deserialize<List<string>>(activity.Inclusions) ?? new List<string>(),

                    Exclusions = string.IsNullOrWhiteSpace(activity.Exclusions)
        ? new List<string>()
        : JsonSerializer.Deserialize<List<string>>(activity.Exclusions) ?? new List<string>(),

                    ImportantInfo = string.IsNullOrWhiteSpace(activity.ImportantInfo)
        ? new List<string>()
        : JsonSerializer.Deserialize<List<string>>(activity.ImportantInfo) ?? new List<string>()
                },
                Translated = translation != null
                    ? new ActivityTranslationDTO
                    {
                        Title = translation.Title,
                        Description = translation.Description,
                        Highlights = translation.Highlights,
                        Itinerary = translation.Itinerary,
                        Inclusions = string.IsNullOrWhiteSpace(translation.InclusionsJson)
                            ? new List<string>()
                            : JsonSerializer.Deserialize<List<string>>(translation.InclusionsJson) ?? new List<string>(),

                        Exclusions = string.IsNullOrWhiteSpace(translation.ExclusionsJson)
                            ? new List<string>()
                            : JsonSerializer.Deserialize<List<string>>(translation.ExclusionsJson) ?? new List<string>(),

                        ImportantInfo = string.IsNullOrWhiteSpace(translation.ImportantInfoJson)
                            ? new List<string>()
                            : JsonSerializer.Deserialize<List<string>>(translation.ImportantInfoJson) ?? new List<string>()
                    }
                    : new ActivityTranslationDTO()
            };

            return View(model);
        }



        // POST: Yeni dil ve çeviri kaydetme
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> AddLanguage(AddLanguageViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var translation = await _context.Translations
                .FirstOrDefaultAsync(t => t.ActivityId == model.ActivityId && t.Language == model.LanguageCode);

            if (translation == null)
            {
                // Yeni çeviri
                _context.Translations.Add(new Translation
                {
                    ActivityId = model.ActivityId,
                    Language = model.LanguageCode,
                    Title = model.Translated.Title,
                    Description = model.Translated.Description,
                    Highlights = model.Translated.Highlights,
                    Itinerary = model.Translated.Itinerary,
                    Label = "",
                    InclusionsJson = JsonSerializer.Serialize(model.Translated.Inclusions.Where(x => !string.IsNullOrWhiteSpace(x))),
                    ExclusionsJson = JsonSerializer.Serialize(model.Translated.Exclusions.Where(x => !string.IsNullOrWhiteSpace(x))),
                    ImportantInfoJson = JsonSerializer.Serialize(model.Translated.ImportantInfo.Where(x => !string.IsNullOrWhiteSpace(x)))
                });

                // ActivityLanguage varsa ekleme
                var exists = await _context.ActivityLanguages
                    .AnyAsync(x => x.ActivityId == model.ActivityId && x.LanguageCode == model.LanguageCode);

                if (!exists)
                {
                    _context.ActivityLanguages.Add(new ActivityLanguage
                    {
                        ActivityId = model.ActivityId,
                        LanguageCode = model.LanguageCode
                    });
                }
            }
            else
            {
                // Var olan çeviriyi güncelle
                translation.Title = model.Translated.Title;
                translation.Description = model.Translated.Description;
                translation.Highlights = model.Translated.Highlights;
                translation.Itinerary = model.Translated.Itinerary;
                translation.InclusionsJson = JsonSerializer.Serialize(model.Translated.Inclusions.Where(x => !string.IsNullOrWhiteSpace(x)));
                translation.ExclusionsJson = JsonSerializer.Serialize(model.Translated.Exclusions.Where(x => !string.IsNullOrWhiteSpace(x)));
                translation.ImportantInfoJson = JsonSerializer.Serialize(model.Translated.ImportantInfo.Where(x => !string.IsNullOrWhiteSpace(x)));
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("ManageLanguages", new { id = model.ActivityId });
        }


        // POST: Dil silme
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> DeleteLanguage(int activityId, string languageCode)
        {
            var langEntry = await _context.ActivityLanguages
                .FirstOrDefaultAsync(x => x.ActivityId == activityId && x.LanguageCode == languageCode);

            if (langEntry != null)
            {
                _context.ActivityLanguages.Remove(langEntry);

                // İlgili Translation kaydını da sil
                var translation = await _context.Translations
                    .FirstOrDefaultAsync(t => t.Language == languageCode);

                if (translation != null)
                {
                    _context.Translations.Remove(translation);
                }

                await _context.SaveChangesAsync();
            }

            return RedirectToAction("ManageLanguages", new { id = activityId });
        }



        #endregion
    }
}