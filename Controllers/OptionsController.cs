using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using TourManagementApi.Data;
using TourManagementApi.Helper;
using TourManagementApi.Models;
using TourManagementApi.Models.ViewModels;

namespace TourManagementApi.Controllers
{
    public class OptionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OptionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? activityId)
        {
            if (!activityId.HasValue)
            {
                return RedirectToAction("Index", "Activities");
            }

            var activity = await _context.Activities
                .FirstOrDefaultAsync(a => a.Id == activityId);

            if (activity == null)
            {
                return NotFound();
            }

            ViewBag.ActivityId = activityId;
            ViewBag.ActivityTitle = activity.Title;

            var options = await _context.Options
                .Where(o => o.ActivityId == activityId)
                .ToListAsync();

            return View(options);
        }

        public IActionResult Create(int activityId)
        {
            var model = new OptionViewModel
            {
                ActivityId = activityId
            };

            return View(model);
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Create(OptionViewModel model)
        {
            if (ModelState.IsValid)
            {


                var option = new Option
                {
                    ActivityId = model.ActivityId,
                    Name = model.Name,
                    StartTime = model.StartTime,
                    EndTime=model.EndTime,
                    FromDate = model.FromDate,
                    UntilDate = model.UntilDate,
                    CutOff = model.CutOff,
                    CanBeBookedAfterStartTime = model.CanBeBookedAfterStartTime,
                    Weekdays = TxtJson.SerializeStringList(model.Weekdays),
                    OpeningHours = model.OpeningHours?.Select(oh => new OpeningHour
                    {
                        FromTime = oh.FromTime,
                        ToTime = oh.ToTime
                    }).ToList() ?? new List<OpeningHour>(),
                    TicketCategories = model.TicketCategories?.Select(tc => new TicketCategory
                    {
                        Name = tc.Name,
                        MinSeats = tc.MinSeats,
                        MaxSeats = tc.MaxSeats,
                        Amount = tc.Amount,
                        Currency = tc.Currency,
                        PriceType = tc.PriceType,
                        Type = tc.Type,
                        MinAge = tc.MinAge,
                        MaxAge = tc.MaxAge
                    }).ToList() ?? new List<TicketCategory>()
                };

                _context.Options.Add(option);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index), new { activityId = model.ActivityId });
            }

            // Hataları logla
            foreach (var key in ModelState.Keys)
            {
                var errors = ModelState[key]?.Errors;
                if (errors != null && errors.Count > 0)
                {
                    Console.WriteLine($"ModelState Error - {key}: {string.Join(", ", errors.Select(e => e.ErrorMessage))}");
                }
            }

            return View(model);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var option = await _context.Options
                .Include(o => o.OpeningHours)
                .Include(o => o.TicketCategories)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (option == null)
            {
                return NotFound();
            }

            var model = new OptionViewModel
            {
                Id = option.Id,
                ActivityId = option.ActivityId,
                Name = option.Name,
                StartTime = option.StartTime,
                EndTime = option.EndTime,
                FromDate = option.FromDate,
                UntilDate = option.UntilDate,
                CutOff = option.CutOff,
                CanBeBookedAfterStartTime = option.CanBeBookedAfterStartTime,
                Weekdays = TxtJson.DeserializeStringList(option.Weekdays),
                OpeningHours = option.OpeningHours.Select(oh => new OpeningHourViewModel
                {
                    Id = oh.Id,
                    FromTime = oh.FromTime,
                    ToTime = oh.ToTime
                }).ToList(),
                TicketCategories = option.TicketCategories.Select(tc => new TicketCategoryViewModel
                {
                    Id = tc.Id,
                    Name = tc.Name,
                    MinSeats = tc.MinSeats,
                    MaxSeats = tc.MaxSeats,
                    Amount = tc.Amount,
                    Currency = tc.Currency,
                    PriceType = tc.PriceType,
                    Type = tc.Type,
                    MinAge = tc.MinAge,
                    MaxAge = tc.MaxAge
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Edit(int id, OptionViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {

                try
                {
                    var option = await _context.Options
                        .Include(o => o.OpeningHours)
                        .Include(o => o.TicketCategories)
                        .FirstOrDefaultAsync(o => o.Id == id);

                    if (option == null)
                    {
                        return NotFound();
                    }

                    option.Name = model.Name;
                    option.StartTime = model.StartTime;
                    option.EndTime = model.EndTime;
                    option.FromDate = model.FromDate;
                    option.UntilDate = model.UntilDate;
                    option.CutOff = model.CutOff;
                    option.CanBeBookedAfterStartTime = model.CanBeBookedAfterStartTime;
                    option.Weekdays = TxtJson.SerializeStringList(model.Weekdays);

                    // TicketCategories güncelleme

                    // OpeningHours güncelleme
                    _context.OpeningHours.RemoveRange(option.OpeningHours);
                    option.OpeningHours = model.OpeningHours?.Select(oh => new OpeningHour
                    {
                        FromTime = oh.FromTime,
                        ToTime = oh.ToTime,
                        OptionId = option.Id
                    }).ToList() ?? new List<OpeningHour>();

                    // Önce TicketCategoryCapacities silincek dayı
                    foreach (var tc in option.TicketCategories)
                    {
                        var capacities = _context.TicketCategoryCapacities.Where(c => c.TicketCategoryId == tc.Id);
                        _context.TicketCategoryCapacities.RemoveRange(capacities);
                    }

                    // Sonra TicketCategories sikilir
                    _context.TicketCategories.RemoveRange(option.TicketCategories);

                    // TicketCategories güncelleme
                    option.TicketCategories = model.TicketCategories?.Select(tc => new TicketCategory
                    {
                        Name = tc.Name,
                        MinSeats = tc.MinSeats,
                        MaxSeats = tc.MaxSeats,
                        Amount = tc.Amount,
                        Currency = tc.Currency,
                        PriceType = tc.PriceType,
                        Type = tc.Type,
                        MinAge = tc.MinAge,
                        MaxAge = tc.MaxAge,
                        OptionId = option.Id
                    }).ToList() ?? new List<TicketCategory>();

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index), new { activityId = model.ActivityId });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OptionExists(model.Id ?? 0))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            return View(model);
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var option = await _context.Options
                .Include(o => o.OpeningHours)
                .Include(o => o.TicketCategories)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (option == null)
            {
                return NotFound();
            }

            var activityId = option.ActivityId;

            _context.Options.Remove(option);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index), new { activityId });
        }

        private bool OptionExists(int id)
        {
            return _context.Options.Any(e => e.Id == id);
        }
    }
} 