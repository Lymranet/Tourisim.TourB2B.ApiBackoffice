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
    public class ReportsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<ReportsController> _logger;

        public ReportsController(
            ApplicationDbContext context,
            IWebHostEnvironment environment,
            ILogger<ReportsController> logger)
        {
            _context = context;
            _environment = environment;
            _logger = logger;
        }


        public IActionResult TourReservationReport()
        {
            var data = _context.Reservations
                .Include(r => r.Activity)
                .GroupBy(r => r.Activity.Title)
                .Select(g => new TourReservationReportDto
                {
                    TourTitle = g.Key,
                    ReservationCount = g.Count(),
                    TotalGuests = g.Sum(x => x.GuestCount)
                })
                .OrderByDescending(x => x.TotalGuests)
                .ToList();

            return View("TourReservationReport", data);
        }

        public IActionResult DailyOperationalList()
        {
            var today = DateTime.Today;

            var data = _context.Reservations
                .Include(r => r.Activity)
                .Include(r => r.Option)
                .Include(r => r.Guests)
                .Where(r => r.ScheduledDate.Date == today)
                .Select(r => new DailyOperationalItemDto
                {
                    TourTitle = r.Activity.Title,
                    OptionName = r.Option.Name,
                    ScheduledDate = r.ScheduledDate,
                    GuestCount = r.GuestCount,
                    GuestNames = r.Guests.Select(g => g.GuestName).ToList()
                })
                .ToList();

            return View("DailyOperationalList", data);
        }

        public IActionResult RevenueAndGuestsPerTour()
        {
            var data = _context.Reservations
                .Include(r => r.Activity)
                .GroupBy(r => r.Activity.Title)
                .Select(g => new TourRevenueSummaryDto
                {
                    TourTitle = g.Key,
                    TotalAmount = g.Sum(r => r.TotalAmount),
                    TotalGuests = g.Sum(r => r.GuestCount)
                })
                .OrderByDescending(x => x.TotalAmount)
                .ToList();

            return View("RevenueAndGuestsPerTour", data);
        }

        public IActionResult AverageGuestsPerTour()
        {
            var data = _context.Reservations
                .Include(r => r.Activity)
                .GroupBy(r => r.Activity.Title)
                .Select(g => new TourAverageGuestsDto
                {
                    TourTitle = g.Key,
                    AverageGuests = g.Average(r => r.GuestCount)
                })
                .OrderByDescending(x => x.AverageGuests)
                .ToList();

            return View("AverageGuestsPerTour", data);
        }

        public IActionResult MonthlyRevenue()
        {
            var data = _context.Reservations
                .AsNoTracking()
                .ToList() // veriyi belleğe al
                .GroupBy(r => r.ScheduledDate.ToString("yyyy-MM")) // bellekte grup yap
                .Select(g => new MonthlyRevenueReportDto
                {
                    Month = g.Key,
                    TotalAmount = g.Sum(r => r.TotalAmount)
                })
                .OrderBy(x => x.Month)
                .ToList();

            return View("MonthlyRevenue", data);
        }


        public IActionResult StatusDistributionReport()
        {
            var data = _context.Reservations
                .GroupBy(r => r.Status)
                .Select(g => new ReservationStatusReportDto
                {
                    Status = g.Key,
                    ReservationCount = g.Count(),
                    TotalAmount = g.Sum(x => x.TotalAmount)
                })
                .OrderByDescending(x => x.ReservationCount)
                .ToList();

            return View("StatusDistributionReport", data);
        }
        public IActionResult Index()
        {
            return View("Index");
        }
    }
}
