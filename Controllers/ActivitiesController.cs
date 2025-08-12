using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic.FileIO;
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
using TourManagementApi.Services;
using Addon = TourManagementApi.Models.Addon;

namespace TourManagementApi.Controllers
{
    public class ActivitiesController : Controller
    {
        private readonly TourManagementDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<ActivitiesController> _logger;
        private readonly ExperienceBankService _experienceBankService;

        public ActivitiesController(
            TourManagementDbContext context,
            IWebHostEnvironment environment,
            ILogger<ActivitiesController> logger, ExperienceBankService experienceBankService)
        {
            _context = context;
            _environment = environment;
            _logger = logger;
            _experienceBankService = experienceBankService;

        }

        #region Activies
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteActivity(int id)
        {


            var activity = await _context.Activities
                .Include(a => a.Options)
                .Include(a => a.Addons)
                .Include(a => a.Availabilities)
                .Include(a => a.MeetingPoints)
                .Include(a => a.RoutePoints)
                .Include(a => a.TimeSlots)
                .Include(a => a.Translations)
                .Include(a => a.PriceCategories)
                .Include(a => a.CancellationPolicyConditions)
                .Include(a => a.ActivityLanguages)
                .Include(a => a.Reservations)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (activity == null)
                return NotFound();

            if (activity.Status == "active")
                return BadRequest("Aktif turlar silinemez.");
            // Alt ilişkili veriler siliniyor
            _context.Options.RemoveRange(activity.Options);
            _context.Addons.RemoveRange(activity.Addons);
            _context.Availabilities.RemoveRange(activity.Availabilities);
            _context.MeetingPoints.RemoveRange(activity.MeetingPoints);
            _context.RoutePoints.RemoveRange(activity.RoutePoints);
            _context.TimeSlots.RemoveRange(activity.TimeSlots);
            _context.Translations.RemoveRange(activity.Translations);
            _context.PriceCategories.RemoveRange(activity.PriceCategories);
            _context.CancellationPolicyConditions.RemoveRange(activity.CancellationPolicyConditions);
            _context.ActivityLanguages.RemoveRange(activity.ActivityLanguages);
            _context.Reservations.RemoveRange(activity.Reservations);

            // Ana activity silipduru
            _context.Activities.Remove(activity);
            await _context.SaveChangesAsync();

            try
            {
                // ExperienceBank notification
                await _experienceBankService.NotifyActivityUpdatedAsync(
                    activityId: activity.Id.ToString(),
                    partnerSupplierId: activity.PartnerSupplierId
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ExperienceBank notify failed after DeleteActivity");
            }

            return RedirectToAction("Index");
        }


        public IActionResult Index(string Id)
        {
            try
            {
#if DEBUG
                Id = "96d2940f09674b0f81d86bc821c69ff6";
#endif

                if (string.IsNullOrEmpty(Id))
                {
                    // Eğer Id boşsa Session'dan oku
                    Id = HttpContext.Session.GetString("B2BAgencyId");
                }
                else
                {
                    // Eğer Id geldiyse Session'a kaydet
                    HttpContext.Session.SetString("B2BAgencyId", Id);
                }
                var activities = _context.Activities.Where(a => a.B2BAgencyId == Id)
                    .Include(a => a.Availabilities)
                    .Include(a => a.Options)
                    .Include(a => a.TourCompany)
                   .Select(a => new ActivityBasicViewModel
                   {
                       ActivityId = a.Id,
                       Title = a.Title ?? string.Empty,
                       Description = a.Description ?? string.Empty,
                       Category = a.Category ?? string.Empty,
                       TourCompany = a.TourCompany.CompanyName,
                       Label = a.Label ?? string.Empty,
                       Status = a.Status ?? "draft",
                       Options = a.Options ?? new List<Option>(),
                       CountryCode = a.CountryCode ?? string.Empty,
                       DestinationCode = a.DestinationCode ?? string.Empty,
                       DestinationName = a.DestinationName ?? string.Empty,
                       CoverImageUrl = a.CoverImage,
                       CreatedAt = a.CreatedAt,
                       UpdatedAt = a.UpdatedAt,
                       Rating = a.Rating,
                       IsFreeCancellation = a.IsFreeCancellation,
                       ReservationsCount = a.Reservations.Count(),
                       AvailabilitiesCount = a.Availabilities.Count()
                   })

                    .ToList();

                return View(activities);
            }
            catch (Exception ex)
            {
                return View(new List<Activity>());
            }
        }
        public IActionResult Preview(int id)
        {
            var activity = _context.Activities
                .Include(a => a.Options).ThenInclude(o => o.TicketCategories)
                .Include(a => a.Options).ThenInclude(o => o.OpeningHours)
                .Include(a => a.Addons)
                .Include(a => a.MeetingPoints)
                .Include(a => a.RoutePoints)
                .Include(a => a.Translations)
                .Include(a => a.ActivityLanguages)
                .Include(a => a.Availabilities)
                .Include(a => a.CancellationPolicyConditions)
                .Include(a => a.TourCompany)
                .FirstOrDefault(a => a.Id == id);

            if (activity == null)
                return NotFound();

            var model = new ActivityPreviewViewModel
            {
                Id = activity.Id,
                Title = activity.Title,
                Description = activity.Description,
                Category = activity.Category,
                Duration = activity.Duration,
                Status = activity.Status,
                Rating = activity.Rating,
                IsFreeCancellation = activity.IsFreeCancellation,
                Label = activity.Label,
                CountryCode = activity.CountryCode,
                DestinationCode = activity.DestinationCode,
                DestinationName = activity.DestinationName,
                CoverImage = activity.CoverImage,
                PreviewImage = activity.PreviewImage,
                MediaVideos = activity.Media_Videos,
                ContactInfoName = activity.ContactInfo_Name,
                ContactInfoEmail = activity.ContactInfo_Email,
                ContactInfoPhone = activity.ContactInfo_Phone,
                ContactInfoRole = activity.ContactInfo_Role,
                Exclusions = activity.Exclusions,
                Inclusions = activity.Inclusions,
                ImportantInfo = activity.ImportantInfo,
                Highlights = activity.Highlights,
                Itinerary = activity.Itinerary,
                DetailsUrl = activity.DetailsUrl,
                ExclusionsJson = activity.ExclusionsJson,
                InclusionsJson = activity.InclusionsJson,
                ImportantInfoJson = activity.ImportantInfoJson,
                GuestFieldsJson = activity.GuestFieldsJson,
                GuestFields = activity.GuestFields,
                CreatedAt = activity.CreatedAt,
                UpdatedAt = activity.UpdatedAt,
                TourCompanyName = activity.TourCompany?.CompanyName,
                PartnerSupplierId = activity.PartnerSupplierId,
                B2BAgencyId = activity.B2BAgencyId,

                Options = activity.Options.ToList(),
                Addons = activity.Addons.ToList(),
                MeetingPoints = activity.MeetingPoints.ToList(),
                RoutePoints = activity.RoutePoints.ToList(),
                Translations = activity.Translations.ToList(),
                ActivityLanguages = activity.ActivityLanguages.ToList(),
                CancellationPolicies = activity.CancellationPolicyConditions.ToList(),
                Availabilities = activity.Availabilities.ToList()
            };

            // Galeri JSON parse et
            if (!string.IsNullOrEmpty(activity.GalleryImages))
            {
                try
                {
                    var images = System.Text.Json.JsonSerializer.Deserialize<List<string>>(activity.GalleryImages);
                    model.GalleryImages = images ?? new List<string>();
                }
                catch { }
            }

            return View("Preview", model);
        }

        public IActionResult Preview2(int id)
        {
            var activity = _context.Activities
                .Include(a => a.Options)
                .Include(a => a.ActivityLanguages)
                .Include(a => a.Addons)
                .Include(a => a.MeetingPoints)
                .Include(a => a.CancellationPolicyConditions)
                .Include(a => a.RoutePoints)
                .Include(a => a.Translations)
                .FirstOrDefault(a => a.Id == id);

            if (activity == null)
            {
                return NotFound();
            }

            var missingFields = new List<string>();

            if (string.IsNullOrWhiteSpace(activity.Description))
                missingFields.Add("Açıklama");

            if (string.IsNullOrWhiteSpace(activity.Category))
                missingFields.Add("Kategori");

            if (string.IsNullOrWhiteSpace(activity.DestinationName))
                missingFields.Add("Lokasyon adı");

            if (string.IsNullOrWhiteSpace(activity.CountryCode))
                missingFields.Add("Ülke Kodu");

            if (string.IsNullOrWhiteSpace(activity.CoverImage))
                missingFields.Add("Kapak Görseli");

            if (activity.Options == null || !activity.Options.Any())
                missingFields.Add("Seçenek (Option)");

            if (activity.Addons == null || !activity.Addons.Any())
                missingFields.Add("Ek Ürün (Addon)");

            if (activity.MeetingPoints == null || !activity.MeetingPoints.Any())
                missingFields.Add("Buluşma Noktası");

            if (activity.RoutePoints == null || !activity.RoutePoints.Any())
                missingFields.Add("Güzergah (Route Points)");

            if (activity.Translations == null || !activity.Translations.Any())
                missingFields.Add("Çeviri");

            if (activity.CancellationPolicyConditions == null || !activity.CancellationPolicyConditions.Any())
                missingFields.Add("İptal Politikası");

            if (activity.ActivityLanguages == null || !activity.ActivityLanguages.Any())
                missingFields.Add("Dil Tanımı");

            var viewModel = new ActivityPreviewViewModel
            {
                ActivityId = activity.Id,
                Title = activity.Title,
                MissingFields = missingFields
            };

            return View(viewModel);
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
            var tourCompanies = await _context.TourCompanies.OrderBy(tc => tc.CompanyName).Select(tc => new SelectListItem
            {
                Value = tc.Id.ToString(),
                Text = tc.CompanyName
            }).ToListAsync();
            try
            {

                if (id.HasValue)
                {
                    var activity = await _context.Activities.FindAsync(id.Value);
                    if (activity == null) return NotFound();

                    var cancellationPolicy = _context.CancellationPolicyConditions.Where(a => a.ActivityId == id.Value).ToList();
                    var vm = new ActivityBasicViewModel
                    {
                        ActivityId = activity.Id,
                        Title = activity.Title,
                        Categories = TxtJson.DeserializeStringList(activity.Category),
                        Description = activity.Description,
                        ContactInfo = activity.ContactInfo_Name != null ? new ContactInfoViewModel
                        {
                            Name = activity.ContactInfo_Name,
                            Role = activity.ContactInfo_Role,
                            Email = activity.ContactInfo_Email,
                            Phone = activity.ContactInfo_Phone
                        } : new ContactInfoViewModel(),
                        CoverImageUrl = activity.CoverImage,
                        PreviewImageUrl = activity.PreviewImage,
                        GalleryImageUrls = TxtJson.DeserializeStringList(activity.GalleryImages),
                        ExistingGalleryImages = TxtJson.DeserializeStringList(activity.GalleryImages),
                        VideoUrls = TxtJson.DeserializeStringList(activity.Media_Videos),
                        Highlights = activity.Highlights,
                        Inclusions = TxtJson.DeserializeStringList(activity.Inclusions),
                        Exclusions = TxtJson.DeserializeStringList(activity.Exclusions),
                        ImportantInfo = TxtJson.DeserializeStringList(activity.ImportantInfo),
                        Itinerary = activity.Itinerary,
                        CountryCode = activity.CountryCode,
                        DestinationCode = activity.DestinationCode,
                        DestinationName = activity.DestinationName,
                        TourCompanyId = activity.TourCompanyId,
                        TourCompanies = tourCompanies,
                        CancellationPolicies = cancellationPolicy.OrderByDescending(c => c.MinDurationBeforeStartTimeSec)
                        .Select(c =>
                        {
                            // kullanıcıya "day" veya "hour" olarak anlamlı döndür
                            // 24 saat ve katları gün olarak, diğerleri saat olarak gösterelim
                            if (c.MinDurationBeforeStartTimeSec % (24 * 3600) == 0)
                            {
                                return new CancellationPolicyConditionInput
                                {
                                    MinDurationUnit = "day",
                                    MinDurationValue = c.MinDurationBeforeStartTimeSec / (24 * 3600),
                                    RefundPercentage = c.RefundPercentage,
                                    IsFreeCancellation = c.IsFreeCancellation
                                };
                            }
                            else
                            {
                                return new CancellationPolicyConditionInput
                                {
                                    MinDurationUnit = "hour",
                                    MinDurationValue = c.MinDurationBeforeStartTimeSec / 3600,
                                    RefundPercentage = c.RefundPercentage,
                                    IsFreeCancellation = c.IsFreeCancellation
                                };
                            }
                        }).ToList(),
                        IsFreeCancellationSummary = activity.IsFreeCancellation ?? false
                    };
                    return View(vm);
                }
            }
            catch (Exception ex)
            {

                throw;
            }

            return View(new ActivityBasicViewModel
            {
                TourCompanies = tourCompanies,
                CancellationPolicies = new List<CancellationPolicyConditionInput>
        {
            // yeni kayıt için 1 örnek satır
            new CancellationPolicyConditionInput { MinDurationValue = 24, MinDurationUnit = "hour", RefundPercentage = 100, IsFreeCancellation = true }
        }
            });
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        [DisableRequestSizeLimit]
        [RequestFormLimits(MultipartBodyLengthLimit = 1024 * 1024 * 100)]
        public async Task<IActionResult> CreateBasic(ActivityBasicViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.TourCompanies = await _context.TourCompanies.OrderBy(tc => tc.CompanyName).Select(tc => new SelectListItem
                {
                    Value = tc.Id.ToString(),
                    Text = tc.CompanyName
                }).ToListAsync();

                return View(model);
            }

            var agencyId = HttpContext.Session.GetString("B2BAgencyId");
            if (string.IsNullOrEmpty(agencyId))
            {
                // Eğer Id boşsa Session'dan oku
                agencyId = "96d2940f09674b0f81d86bc821c69ff6";
            }
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


                //
                // 1) Önce mevcut koşulları temizle (update senaryosu)
                //
                var existingConds = await _context.CancellationPolicyConditions
                    .Where(c => c.ActivityId == activity.Id)
                    .ToListAsync();

                if (existingConds.Any())
                {
                    _context.CancellationPolicyConditions.RemoveRange(existingConds);
                    await _context.SaveChangesAsync();
                }
                var newConds = new List<CancellationPolicyCondition>();
                if (model.CancellationPolicies != null)
                {
                    foreach (var row in model.CancellationPolicies)
                    {
                        // basit validasyon
                        if (row.MinDurationValue < 0 || row.RefundPercentage < 0 || row.RefundPercentage > 100)
                            continue;

                        var seconds = row.MinDurationUnit == "day"
                            ? row.MinDurationValue * 24 * 3600
                            : row.MinDurationValue * 3600;

                        newConds.Add(new CancellationPolicyCondition
                        {
                            ActivityId = activity.Id,
                            MinDurationBeforeStartTimeSec = seconds,
                            RefundPercentage = row.RefundPercentage,
                            IsFreeCancellation = row.IsFreeCancellation
                        });
                    }
                }
                // 3) Activity.IsFreeCancellation özetini hesapla
                // Kural: herhangi bir satır free ise ya da Refund=100 ise (ve min süre > 0) "free cancellation" vardır.
                activity.IsFreeCancellation = newConds.Any(c => c.IsFreeCancellation || c.RefundPercentage == 100);

                if (newConds.Any())
                {
                    await _context.CancellationPolicyConditions.AddRangeAsync(newConds);
                }
                newConds = newConds
                .OrderByDescending(c => c.MinDurationBeforeStartTimeSec)
                .ToList();

                activity.B2BAgencyId = agencyId;
                activity.Title = model.Title ?? string.Empty;
                activity.Category = TxtJson.SerializeStringList(model.Categories?.Take(3).ToList() ?? new List<string>());
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

                activity.Media_Videos = JsonSerializer.Serialize(model.VideoUrls?.Where(url => !string.IsNullOrWhiteSpace(url)).ToList() ?? new List<string>());

                activity.ContactInfo_Name = model.ContactInfo.Name ?? string.Empty;
                activity.ContactInfo_Role = model.ContactInfo.Role ?? string.Empty;
                activity.ContactInfo_Email = model.ContactInfo.Email ?? string.Empty;
                activity.ContactInfo_Phone = model.ContactInfo.Phone ?? string.Empty;

                activity.Highlights = model.Highlights ?? string.Empty;
                activity.Inclusions = JsonSerializer.Serialize(model.Inclusions?.Where(x => !string.IsNullOrWhiteSpace(x)).ToList() ?? new List<string>());
                activity.Exclusions = JsonSerializer.Serialize(model.Exclusions?.Where(x => !string.IsNullOrWhiteSpace(x)).ToList() ?? new List<string>());
                activity.ImportantInfo = JsonSerializer.Serialize(model.ImportantInfo?.Where(x => !string.IsNullOrWhiteSpace(x)).ToList() ?? new List<string>());
                activity.Itinerary = model.Itinerary;

                activity.TourCompanyId = model.TourCompanyId;
                activity.CountryCode = model.CountryCode;
                activity.DestinationCode = model.DestinationCode;
                activity.DestinationName = model.DestinationName;
                activity.Status = "draft";
                if (!model.ActivityId.HasValue)
                {
                    _context.Activities.Add(activity);
                }

                await _context.SaveChangesAsync();

                try
                {
                    await _experienceBankService.NotifyActivityUpdatedAsync(
                        activityId: activity.Id.ToString(),
                        partnerSupplierId: activity.PartnerSupplierId
                    );
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "ExperienceBank notify failed after CreateBasic");
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Kayıt sırasında bir hata oluştu: " + ex.Message);
                return View(model);
            }
        }

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
        #endregion


        #region Yield


        [HttpPost]
        public IActionResult SavePricing([FromBody] FiyatlandirmaViewModel1 model)
        {
            try
            {
                if (model == null)
                    return BadRequest(new { success = false, message = "Model boş." });

                var ticketCategories = _context.TicketCategories
                    .Where(a => a.OptionId == model.OptionId)
                    .ToList();

                if (!ticketCategories.Any())
                    return NotFound(new { success = false, message = "İlgili OptionId için kayıt bulunamadı." });

                foreach (var a in ticketCategories)
                {
                    a.SalePrice = a.Amount + (a.Amount * model.Percentage / 100);
                    a.SalePercentage = model.Percentage;
                }

                _context.SaveChanges();

                return Json(new { success = true, message = $"OptionId {model.OptionId} güncellendi." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message,
                    stack = ex.StackTrace
                });
            }
        }









        public async Task<IActionResult> PricingYield(int id)
        {
            var activity = await _context.Activities
                .Include(a => a.Options)
                .ThenInclude(o => o.TicketCategories)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (activity == null) return NotFound();

            var vm = new FiyatlandirmaViewModel
            {
                ActivityId = activity.Id,
                ActivityTitle = activity.Title,
                Options = activity.Options.Select(o =>
                {
                    var platformKomOrani = 0.3m;

                    var ticketCategories = o.TicketCategories.Select(tc => new TicketCategoryPricingViewModel
                    {
                        TicketCategoryId = tc.Id,
                        TicketCategoryName = tc.Name,
                        Amount = (tc.Amount * 150 / 100), // Satış fiyatı örneği
                        Currency = tc.Currency,
                        SupplierCost = tc.Amount
                    }).ToList();

                    var toplamSatis = ticketCategories.Sum(tc => tc.Amount);
                    var toplamTaseronMaliyeti = ticketCategories.Sum(tc => tc.SupplierCost);
                    var komisyonMaliyeti = toplamSatis * platformKomOrani;
                    var kalanTutar = toplamSatis - komisyonMaliyeti - toplamTaseronMaliyeti;
                    var toplamMaliyet = ticketCategories.Sum(tc => tc.SupplierCost) + komisyonMaliyeti;
                    return new OptionPricingViewModel
                    {
                        OptionId = o.Id,
                        OptionName = o.Name,
                        TicketCategories = ticketCategories,
                        AracMaliyeti = 0,
                        TopMaliyeti = 0,
                        GelirVergisi = (kalanTutar * 20 / 100),
                        RehberBonus = 0,
                        PlatformKomisyonTutari = komisyonMaliyeti + 0,
                        KomisyonMaliyeti = komisyonMaliyeti,
                        PlatformKomOrani = platformKomOrani,
                        Karlilik = (toplamSatis - toplamMaliyet) / toplamSatis
                    };
                }).ToList()
            };

            return View(vm);
        }

        #endregion

        #region Location

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

        #endregion

        #region Addon 

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
            var addOns = await _context.Addons.Where(a => a.ActivityId == id).ToListAsync();
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

        #endregion

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
                { "en", "en_us" }, // İngilizce - ABD İngilizcesi
                { "de", "de_de" }, // Almanca - Almanya
                { "fr", "fr_fr" }, // Fransızca - Fransa
                { "es", "es_es" }, // İspanyolca - İspanya
                { "it", "it_it" }, // İtalyanca - İtalya
                { "ru", "ru_ru" }, // Rusça - Rusya
                { "ar", "ar_ae" }, // Arapça - BAE (genel kullanım için)
                { "zh", "zh_cn" }  // Çince - Çin (Basitleştirilmiş Çince)
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

        #region Availability Alanı

        [HttpGet]
        public async Task<IActionResult> CreateAvailability(int id)
        {
            var activity = await _context.Activities
                .Include(a => a.Options)
                .ThenInclude(o => o.TicketCategories)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (activity == null) return NotFound();

            var viewModel = new CreateAvailabilityViewModel
            {
                ActivityId = activity.Id,
                OptionList = activity.Options.Select(o => new SelectListItem
                {
                    Value = o.Id.ToString(),
                    Text = o.Name
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetTicketCategories(int optionId)
        {
            var option = await _context.Options
                .Include(o => o.TicketCategories)
                .FirstOrDefaultAsync(o => o.Id == optionId);

            if (option == null)
                return Json(new List<object>());

            var categories = option.TicketCategories.Select(tc => new
            {
                id = tc.Id,
                name = tc.Name
            }).ToList();

            return Json(categories);
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> CreateAvailability(CreateAvailabilityViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var existingDates = await _context.Availabilities
                .Where(a =>
                    a.ActivityId == model.ActivityId &&
                    a.OptionId == model.OptionId &&
                    a.Date >= model.StartDate && a.Date <= model.EndDate)
                .Select(a => a.Date)
                .ToListAsync();

            var newAvailabilities = new List<Availability>();

            for (var date = model.StartDate; date <= model.EndDate; date = date.AddDays(1))
            {
                if (existingDates.Contains(date))
                    continue; //  kayıt var geçççç

                var availability = new Availability
                {
                    ActivityId = model.ActivityId,
                    OptionId = model.OptionId,
                    Date = date,
                    StartTime = new DateTimeOffset(
                        new DateTime(date.Year, date.Month, date.Day, model.StartTime.Hour, model.StartTime.Minute, model.StartTime.Second),
                        TimeSpan.FromHours(3) // veya: TimeZoneInfo.Local.GetUtcOffset(...)
                    ),
                    AvailableCapacity = model.AvailableCapacity,
                    MaximumCapacity = model.MaximumCapacity,
                    PartnerSupplierId = "12004",
                    TicketCategoryCapacities = model.TicketCategories
                        .Where(tc => tc.TicketCategoryId > 0)
                        .Select(tc => new TicketCategoryCapacity
                        {
                            TicketCategoryId = tc.TicketCategoryId,
                            Capacity = tc.Capacity
                        }).ToList()
                };

                newAvailabilities.Add(availability);
            }

            if (!newAvailabilities.Any())
            {
                ModelState.AddModelError("", "Tüm günler için zaten kapasite tanımlanmış.");
                model.OptionList = await _context.Options
                    .Where(o => o.ActivityId == model.ActivityId)
                    .Select(o => new SelectListItem
                    {
                        Value = o.Id.ToString(),
                        Text = o.Name
                    }).ToListAsync();

                model.TicketCategories = await _context.TicketCategories
                    .Where(tc => tc.OptionId == model.OptionId)
                    .Select(tc => new TicketCategoryInputModel
                    {
                        TicketCategoryId = tc.Id,
                        Name = tc.Name
                    }).ToListAsync();

                return View(model);
            }

            _context.Availabilities.AddRange(newAvailabilities);
            await _context.SaveChangesAsync();

            // ExperienceBank notification
            try
            {
                foreach (var availability in newAvailabilities)
                {
                    // Örneğin availability availableCapacity < 30 ise veya oldCapacity = 0 -> current > 0 ise trigger et
                    if (availability.AvailableCapacity < 30 || availability.AvailableCapacity > 0)
                    {
                        await _experienceBankService.NotifyAvailabilityUpdatedAsync(
                            supplierId: availability.PartnerSupplierId,
                            activityId: availability.ActivityId.ToString(),
                            optionId: availability.OptionId.ToString(),
                            localDateTime: availability.Date.ToString("yyyy-MM-ddTHH:mm:sszzz"),
                            availableCapacity: availability.AvailableCapacity,
                            oldCapacity: 0 // ilk create olduğunda eski kapasite yok
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ExperienceBank availability notify failed (CreateAvailability)");
            }


            return RedirectToAction("Index");
        }

        public async Task<IActionResult> EditAvailability(int id)
        {
            var availability = await _context.Availabilities
                .Include(a => a.TicketCategoryCapacities)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (availability == null)
                return NotFound();

            var option = await _context.Options.FindAsync(availability.OptionId);

            var model = new CreateAvailabilityViewModel
            {
                Id = availability.Id,
                ActivityId = availability.ActivityId,
                OptionId = availability.OptionId,
                StartDate = availability.Date,
                EndDate = availability.Date,
                StartTime = TimeOnly.FromDateTime(availability.StartTime?.UtcDateTime ?? DateTime.UtcNow),
                AvailableCapacity = availability.AvailableCapacity,
                MaximumCapacity = availability.MaximumCapacity,
                OptionList = await _context.Options
                    .Where(o => o.ActivityId == availability.ActivityId)
                    .Select(o => new SelectListItem
                    {
                        Value = o.Id.ToString(),
                        Text = o.Name
                    }).ToListAsync(),
                TicketCategories = availability.TicketCategoryCapacities.Select(tc => new TicketCategoryInputModel
                {
                    TicketCategoryId = tc.TicketCategoryId,
                    Capacity = tc.Capacity,
                    Name = "" // dilersen isme göre DB'den çekebilirsin
                }).ToList()
            };

            return View("CreateAvailability", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAvailability(CreateAvailabilityViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.OptionList = await _context.Options
                    .Where(o => o.ActivityId == model.ActivityId)
                    .Select(o => new SelectListItem
                    {
                        Value = o.Id.ToString(),
                        Text = o.Name
                    }).ToListAsync();

                return View("CreateAvailability", model);
            }

            var availability = await _context.Availabilities
                .Include(a => a.TicketCategoryCapacities)
                .FirstOrDefaultAsync(a => a.Id == model.Id);

            if (availability == null)
                return NotFound();

            availability.OptionId = model.OptionId;
            availability.Date = model.StartDate;
            availability.StartTime = new DateTimeOffset(DateTime.Today.Add(model.StartTime.ToTimeSpan()));
            availability.AvailableCapacity = model.AvailableCapacity;
            availability.MaximumCapacity = model.MaximumCapacity;

            // Kategori kapasite güncelle
            _context.TicketCategoryCapacities.RemoveRange(availability.TicketCategoryCapacities);

            availability.TicketCategoryCapacities = model.TicketCategories
                .Where(tc => tc.TicketCategoryId > 0)
                .Select(tc => new TicketCategoryCapacity
                {
                    TicketCategoryId = tc.TicketCategoryId,
                    Capacity = tc.Capacity
                }).ToList();

            await _context.SaveChangesAsync();

            try
            {
                if (availability.AvailableCapacity < 30 || availability.AvailableCapacity > 0)
                {
                    await _experienceBankService.NotifyAvailabilityUpdatedAsync(
                        supplierId: availability.PartnerSupplierId,
                        activityId: availability.ActivityId.ToString(),
                        optionId: availability.OptionId.ToString(),
                        localDateTime: availability.Date.ToString("yyyy-MM-ddTHH:mm:sszzz"),
                        availableCapacity: availability.AvailableCapacity,
                        oldCapacity: availability.MaximumCapacity // örnek olarak eski kapasiteyi gönder
                    );
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ExperienceBank availability notify failed (EditAvailability)");
            }



            return RedirectToAction("Availabilities", new { activityId = model.ActivityId });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAvailability(int id)
        {
            var availability = await _context.Availabilities
                .Include(a => a.TicketCategoryCapacities)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (availability == null)
                return NotFound();

            _context.TicketCategoryCapacities.RemoveRange(availability.TicketCategoryCapacities);
            _context.Availabilities.Remove(availability);
            await _context.SaveChangesAsync();

            try
            {
                await _experienceBankService.NotifyAvailabilityUpdatedAsync(
                    supplierId: availability.PartnerSupplierId,
                    activityId: availability.ActivityId.ToString(),
                    optionId: availability.OptionId.ToString(),
                    localDateTime: availability.Date.ToString("yyyy-MM-ddTHH:mm:sszzz"),
                    availableCapacity: 0,
                    oldCapacity: availability.AvailableCapacity
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ExperienceBank availability notify failed (DeleteAvailability)");
            }


            return RedirectToAction("Availabilities", new { activityId = availability.ActivityId });
        }

        public async Task<IActionResult> Availabilities(int activityId)
        {
            var activity = await _context.Activities
        .Include(a => a.Availabilities)
            .ThenInclude(av => av.Option)
        .Include(a => a.Availabilities)
            .ThenInclude(av => av.TicketCategoryCapacities)
                .ThenInclude(tcc => tcc.TicketCategory)
        .FirstOrDefaultAsync(a => a.Id == activityId);

            if (activity == null) return NotFound();

            return View(activity);
        }


        [HttpGet]
        public async Task<IActionResult> CheckAvailabilityExists(int activityId, int optionId, string date)
        {
            if (!DateOnly.TryParse(date, out var parsedDate))
                return Json(new { exists = false });

            var exists = await _context.Availabilities
                .AnyAsync(a =>
                    a.ActivityId == activityId &&
                    a.OptionId == optionId &&
                    a.Date == parsedDate);

            return Json(new { exists });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAllAvailabilities(int activityId)
        {
            var availabilities = await _context.Availabilities
                .Where(a => a.ActivityId == activityId)
                .Include(a => a.TicketCategoryCapacities)
                .ToListAsync();

            if (!availabilities.Any())
                return RedirectToAction("Availabilities", new { activityId });

            foreach (var availability in availabilities)
            {
                // Önce TicketCategoryCapacities sil
                _context.TicketCategoryCapacities.RemoveRange(availability.TicketCategoryCapacities);

                // ExperienceBank notify (try-catch içinde)
                try
                {
                    await _experienceBankService.NotifyAvailabilityUpdatedAsync(
                        supplierId: availability.PartnerSupplierId,
                        activityId: availability.ActivityId.ToString(),
                        optionId: availability.OptionId.ToString(),
                        localDateTime: availability.Date.ToString("yyyy-MM-ddTHH:mm:sszzz"),
                        availableCapacity: 0,
                        oldCapacity: availability.AvailableCapacity
                    );
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "ExperienceBank availability notify failed (DeleteAllAvailabilities)");
                }
            }

            // Availabilities sil
            _context.Availabilities.RemoveRange(availabilities);
            await _context.SaveChangesAsync();

            return RedirectToAction("Availabilities", new { activityId });
        }


        #endregion
    }
}