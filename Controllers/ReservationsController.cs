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


        public IActionResult Detail(int id)
        {
            var reservation = _context.Reservations
                .Include(r => r.Activity)
                .Include(r => r.Option)
                .Include(r => r.ReservationGuests)
                .FirstOrDefault(r => r.Id == id);

            if (reservation == null)
                return NotFound();

            return View("Detail", reservation);
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

        public IActionResult Index(ReservationFilterViewModel filters)
        {
            var query = _context.Reservations
                .Include(r => r.Activity)
                .Include(r => r.Option)
                .AsQueryable();

            if (filters.ActivityId.HasValue)
                query = query.Where(r => r.ActivityId == filters.ActivityId.Value);

            if (!string.IsNullOrEmpty(filters.Status))
                query = query.Where(r => r.Status == filters.Status);

            if (filters.FromDate.HasValue)
                query = query.Where(r => r.ScheduledDate >= filters.FromDate.Value);

            if (filters.ToDate.HasValue)
                query = query.Where(r => r.ScheduledDate <= filters.ToDate.Value);

            filters.Reservations = query
                .OrderByDescending(r => r.ScheduledDate)
                .ToList();

            filters.Activities = _context.Activities
                .Select(a => new Activity { Id = a.Id, Title = a.Title })
                .ToList();

            return View("Index", filters);
        }

    }

}
