using Microsoft.AspNetCore.Mvc;
using TourManagementApi.Data;
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
using TourManagementApi.Models.ReportModels;

namespace TourManagementApi.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly TourManagementDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<ReservationsController> _logger;

        public ReservationsController(
            TourManagementDbContext context,
            IWebHostEnvironment environment,
            ILogger<ReservationsController> logger)
        {
            _context = context;
            _environment = environment;
            _logger = logger;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(long id)
        {
            var strategy = _context.Database.CreateExecutionStrategy();

            try
            {
                await strategy.ExecuteAsync(async () =>
                {
                    await using var tx = await _context.Database.BeginTransactionAsync();

                    try
                    {
                        // --- 1) ITEM ALT TABLOLARI (JOIN ile, Contains YOK) ---

                        // ParticipantFields
                        var partFields = await (
                            from f in _context.BookingParticipantFields
                            join p in _context.BookingParticipants on f.ParticipantId equals p.ParticipantId
                            join i in _context.BookingItems on p.ItemId equals i.ItemId
                            where i.BookingId == id
                            select f
                        ).ToListAsync();
                        _context.BookingParticipantFields.RemoveRange(partFields);

                        // Participants
                        var participants = await (
                            from p in _context.BookingParticipants
                            join i in _context.BookingItems on p.ItemId equals i.ItemId
                            where i.BookingId == id
                            select p
                        ).ToListAsync();
                        _context.BookingParticipants.RemoveRange(participants);

                        // Quantities
                        var quantities = await (
                            from q in _context.BookingQuantities
                            join i in _context.BookingItems on q.ItemId equals i.ItemId
                            where i.BookingId == id
                            select q
                        ).ToListAsync();
                        _context.BookingQuantities.RemoveRange(quantities);

                        // Extras
                        var extras = await (
                            from e in _context.BookingExtras
                            join i in _context.BookingItems on e.ItemId equals i.ItemId
                            where i.BookingId == id
                            select e
                        ).ToListAsync();
                        _context.BookingExtras.RemoveRange(extras);

                        // Vouchers
                        var vouchers = await (
                            from v in _context.BookingVouchers
                            join i in _context.BookingItems on v.ItemId equals i.ItemId
                            where i.BookingId == id
                            select v
                        ).ToListAsync();
                        _context.BookingVouchers.RemoveRange(vouchers);

                        // PickupLocations
                        var pickups = await (
                            from pu in _context.BookingPickupLocations
                            join i in _context.BookingItems on pu.ItemId equals i.ItemId
                            where i.BookingId == id
                            select pu
                        ).ToListAsync();
                        _context.BookingPickupLocations.RemoveRange(pickups);

                        // Items (en sonda)
                        var items = await _context.BookingItems
                            .Where(i => i.BookingId == id)
                            .ToListAsync();
                        _context.BookingItems.RemoveRange(items);

                        // --- 2) BOOKING ÜST TABLOLARI (BookingId ile, Contains YOK) ---

                        _context.BookingCreatedBies.RemoveRange(
                            await _context.BookingCreatedBies.Where(c => c.BookingId == id).ToListAsync()
                        );
                        _context.BookingCreditCards.RemoveRange(
                            await _context.BookingCreditCards.Where(c => c.BookingId == id).ToListAsync()
                        );
                        _context.BookingCustomers.RemoveRange(
                            await _context.BookingCustomers.Where(c => c.BookingId == id).ToListAsync()
                        );
                        _context.BookingFields.RemoveRange(
                            await _context.BookingFields.Where(f => f.BookingId == id).ToListAsync()
                        );
                        _context.BookingPayments.RemoveRange(
                            await _context.BookingPayments.Where(p => p.BookingId == id).ToListAsync()
                        );
                        _context.BookingResellerUsers.RemoveRange(
                            await _context.BookingResellerUsers.Where(r => r.BookingId == id).ToListAsync()
                        );

                        // --- 3) BOOKING (en sonda) ---
                        var booking = await _context.Bookings.FirstOrDefaultAsync(b => b.BookingId == id);
                        if (booking != null) _context.Bookings.Remove(booking);

                        await _context.SaveChangesAsync();
                        await tx.CommitAsync();
                    }
                    catch
                    {
                        await tx.RollbackAsync();
                        throw;
                    }
                });

                TempData["Success"] = "Rezervasyon ve tüm alt detayları silindi.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Booking delete failed for {BookingId}", id);
                TempData["Error"] = "Silme işlemi sırasında bir hata oluştu.";
                return RedirectToAction(nameof(Detail), new { id });
            }
        }


        private static (int? activityId, int? optionId) ParseActivityOption(string? externalProductCode)
        {
            if (string.IsNullOrWhiteSpace(externalProductCode)) return (null, null);
            var idx = externalProductCode.IndexOf('-');
            if (idx <= 0 || idx >= externalProductCode.Length - 1) return (null, null);

            var aStr = externalProductCode.Substring(0, idx);
            var oStr = externalProductCode.Substring(idx + 1);

            if (!int.TryParse(aStr, out var a)) return (null, null);
            if (!int.TryParse(oStr, out var o)) return (a, null);

            return (a, o);
        }

        // LIST
        public async Task<IActionResult> Index(BookingListFilterVM filters)
        {
            // booking × item
            var q = _context.Bookings.AsNoTracking()
                .SelectMany(b => b.BookingItems.DefaultIfEmpty(), (b, i) => new { b, i })
                .Where(x => x.i != null && x.b.Status != "ABANDONED_CART");

            // (varsa) tarih/status filtrelerin aynen…

            var proj = q.Select(x => new BookingListItemVM
            {
                BookingId = (int)x.b.BookingId,
                OrderNumber = x.b.OrderNumber!,
                Code = x.b.Code!,
                ProductName = x.i.ProductName!,
                ExternalProductCode = x.i.ExternalProductCode,   // <— parse edeceğiz
                ReservationDate = x.b.DateCreated,
                StartDate = x.b.DateCreated,              // StartTimeLocal string; listede sıralama için DateCreated
                CustomerName = x.b.BookingCustomers
                    .Select(c => c.Name ?? (c.FirstName + " " + c.LastName))
                    .FirstOrDefault() ?? "",
                GuestCount = (x.i.BookingParticipants.Any()
                    ? x.i.BookingParticipants.Count()
                    : (x.i.BookingQuantities.Select(q => (int?)q.Value).Sum() ?? 0)),
                TotalAmount = x.b.TotalAmount ?? 0,
                TotalCurrency = x.b.TotalCurrency!,
                Status = x.b.Status!,
                // ActivityId / ActivityTitle şimdilik null; parse sonrası dolduracağız
                ActivityId = null,
                ActivityTitle = null
            });

            // DB'den çek
            var page = Math.Max(filters.Page, 1);
            var size = Math.Clamp(filters.PageSize, 10, 100);
            filters.TotalCount = await proj.CountAsync();
            filters.Items = await proj.OrderByDescending(v => v.ReservationDate)
                                 .Skip((page - 1) * size)
                                 .Take(size)
                                 .ToListAsync();

            // --- Parse ile ActivityId/OptionId doldur ---
            foreach (var item in filters.Items)
            {
                var (aid, oid) = ParseActivityOption(item.ExternalProductCode);
                item.ActivityId = aid;
                // OptionId’yi de istersen VM’ine ekleyebilirsin:
                // item.OptionId = oid;
            }

            // İstersen ActivityTitle’ı tek seferde doldur:
            var activityIds = filters.Items.Where(i => i.ActivityId.HasValue)
                                           .Select(i => i.ActivityId!.Value)
                                           .Distinct()
                                           .ToList();
            // Boşsa sorgu atma
            Dictionary<int, string> actDict = new();
            if (activityIds.Count > 0)
            {
                // 1) Tüm aktiviteleri hafif projeksiyonla çek (Id, Title)
                var actPairs = await _context.Activities
                    .AsNoTracking()
                    .Select(a => new { a.Id, a.Title })
                    .ToListAsync();                 // <-- sadece SELECT; CTE/WITH tetiklemez

                // 2) Bellekte sözlüğe çevir
                actDict = actPairs.ToDictionary(x => x.Id, x => x.Title);
            }

            // 3) Eşle
            foreach (var item in filters.Items)
            {
                if (item.ActivityId.HasValue)
                {
                    string title;
                    if (actDict.TryGetValue(item.ActivityId.Value, out title))
                    {
                        item.ActivityTitle = title;
                    }
                }
            }


            // Aktivite dropdown: basitçe tüm aktiviteler (istenirse filtreli de yapabilirsin)
            filters.Activities = await _context.Activities.AsNoTracking()
                .OrderBy(a => a.Title)
                .Select(a => new ActivityLookupVM { Id = a.Id, Title = a.Title })
                .ToListAsync();

            return View(filters);

        }

        // DETAIL
        public async Task<IActionResult> Detail(long id)
        {
            var b = await _context.Bookings.AsNoTracking()
                .Where(x => x.BookingId == id)
                .Select(x => new BookingDetailVM
                {
                    Header = new BookingHeaderVM
                    {
                        BookingId = x.BookingId,
                        OrderNumber = x.OrderNumber!,
                        Code = x.Code!,
                        Status = x.Status!,
                        DateCreated = x.DateCreated,
                        DateConfirmed = x.DateConfirmed,
                        TotalAmount = x.TotalAmount ?? 0,
                        TotalCurrency = x.TotalCurrency!,
                        TotalPaid = x.TotalPaid ?? 0,
                        TotalDue = x.TotalDue ?? 0,
                        BarcodeValue = x.BookingFields.Where(f => f.Label == "Barcode").Select(f => f.Value).FirstOrDefault(),
                        BarcodeType = x.BookingFields.Where(f => f.Label == "Barcode").Select(f => f.BarcodeType).FirstOrDefault()
                    },

                    // TÜM MÜŞTERİLER
                    Customers = x.BookingCustomers
                        .Select(c => new BookingCustomerVM
                        {
                            Name = c.Name ?? (c.FirstName + " " + c.LastName),
                            Email = c.Email,
                            Phone = c.Phone ?? c.Mobile,
                            CountryCode = c.CountryCode
                            // İstersen AddressLine, City, PostCode vb. de ekleyebilirsin
                        })
                        .ToList(),

                    Payments = x.BookingPayments.Select(p => new BookingPaymentVM
                    {
                        Date = null,
                        Type = p.Type,
                        Amount = (decimal)(p.Amount ?? 0),
                        Currency = p.Currency!
                    }).OrderByDescending(p => p.Date).ToList(),

                    BookingFields = x.BookingFields.Select(f => new BookingFieldVM
                    {
                        Label = f.Label,
                        Value = f.Value,
                        BarcodeType = f.BarcodeType
                    }).ToList(),

                    InternalNotes = x.InternalNotes,
                    ResellerComments = x.ResellerComments,
                    Source = x.Source,
                    SourceChannel = x.SourceChannel,
                    SupplierName = x.SupplierName,
                    ResellerName = x.ResellerName,

                    Items = x.BookingItems.Select(i => new BookingItemDetailVM
                    {
                        ItemId = (int)i.ItemId,
                        ProductName = i.ProductName!,
                        ExternalProductCode = i.ExternalProductCode,
                        StartTimeLocal = null,
                        EndTimeLocal = null,
                        Amount = i.Amount ?? 0,
                        Subtotal = i.Subtotal ?? 0,
                        TotalItemTax = i.TotalItemTax ?? 0,
                        QuantitySum = (i.BookingQuantities.Select(q => (int?)q.Value).Sum() ?? 0),

                        Quantities = i.BookingQuantities.Select(q => new BookingQuantityVM
                        {
                            OptionLabel = q.OptionLabel!,
                            Value = q.Value ?? 0,
                            OptionPrice = (decimal?)q.OptionPrice
                        }).ToList(),

                        Extras = i.BookingExtras.Select(xe => new BookingExtraVM
                        {
                            Name = xe.Name!,
                            Description = xe.Description,
                            ExtraPriceType = xe.ExtraPriceType,
                            Price = (decimal)(xe.Price ?? 0),
                            Quantity = xe.Quantity ?? 0
                        }).ToList(),

                        Vouchers = i.BookingVouchers.Select(v => new BookingVoucherVM
                        {
                            Code = v.Code!,
                            Status = v.Status,
                            Value = (decimal)(v.Value ?? 0),
                            ValueType = v.ValueType
                        }).ToList(),

                        Pickup = i.BookingPickupLocations.Select(pu => new BookingPickupLocationVM
                        {
                            LocationName = pu.LocationName,
                            Address = pu.Address,
                            PickupInstructions = pu.PickupInstructions,
                            PickupTime = pu.PickupTime
                        }).FirstOrDefault(),

                        Participants = i.BookingParticipants.Select(p => new BookingParticipantVM
                        {
                            ParticipantId = (int)p.ParticipantId,
                            Fields = p.BookingParticipantFields.Select(pf => new BookingFieldVM
                            {
                                Label = pf.Label,
                                Value = pf.Value,
                                BarcodeType = pf.BarcodeType
                            }).ToList()
                        }).ToList()
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (b == null) return NotFound();

            // ExternalProductCode parse + ActivityTitle resolve (daha önceki gibi)
            var activityIds = new HashSet<int>();
            foreach (var it in b.Items)
            {
                var (aid, _) = ParseActivityOption(it.ExternalProductCode);
                it.ActivityId = aid;
                if (aid.HasValue) activityIds.Add(aid.Value);
            }

            if (activityIds.Count > 0)
            {
                var actPairs = await _context.Activities.AsNoTracking()
                    .Select(a => new { a.Id, a.Title })
                    .ToListAsync();
                var actDict = actPairs.ToDictionary(x => x.Id, x => x.Title);

                foreach (var it in b.Items)
                {
                    if (it.ActivityId.HasValue)
                    {
                        string title;
                        if (actDict.TryGetValue(it.ActivityId.Value, out title))
                            it.ActivityTitle = title;
                    }
                }
            }

            return View(b);
        }

    }
}
