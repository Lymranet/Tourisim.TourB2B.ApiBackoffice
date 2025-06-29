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
using TourManagementApi.Services;

namespace TourManagementApi.Controllers
{
    public class TourCompanyController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<TourCompanyController> _logger;
        private readonly ExperienceBankService _experienceBankService;

        public TourCompanyController(
            ApplicationDbContext context,
            IWebHostEnvironment env,
            ILogger<TourCompanyController> logger, ExperienceBankService experienceBankService)
        {
            _context = context;
            _env = env;
            _logger = logger;
            _experienceBankService = experienceBankService;

        }

        public IActionResult Index()
        {
            var companies = _context.TourCompanies.ToList();
            return View(companies);
        }

        public IActionResult Create()
        {
            return View(new TourCompanyViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(TourCompanyViewModel model)
        {
            if (ModelState.IsValid)
            {
                var company = new TourCompany
                {
                    CompanyName = model.CompanyName,
                    AuthorizedPerson = model.AuthorizedPerson,
                    Email = model.Email,
                    Phone = model.Phone
                };

                company.LogoPath = await SaveFile(model.Logo, "logos");
                company.ImzaDocumentPath = await SaveFile(model.ImzaDocument, "docs");
                company.FaaliyetBelgesiPath = await SaveFile(model.FaaliyetBelgesi, "docs");
                company.OdaSicilKaydiPath = await SaveFile(model.OdaSicilKaydi, "docs");
                company.TicaretSicilGazetesiPath = await SaveFile(model.TicaretSicilGazetesi, "docs");
                company.VergiLevhasıPath = await SaveFile(model.VergiLevhası, "docs");
                company.SigortaBelgesiPath = await SaveFile(model.SigortaBelgesi, "docs");
                company.HizmetDetayiPath = await SaveFile(model.HizmetDetayi, "docs");
                company.AracD2belgesiPath = await SaveFile(model.AracD2Belgesi, "docs");
                company.SportifFaaliyetBelgesiPath = await SaveFile(model.SportifFaaliyetBelgesi, "docs");

                _context.Add(company);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var company = await _context.TourCompanies.FindAsync(id);
            if (company == null)
                return NotFound();

            var model = new TourCompanyViewModel
            {
                Id = company.Id,
                CompanyName = company.CompanyName,
                AuthorizedPerson = company.AuthorizedPerson,
                Email = company.Email,
                Phone = company.Phone,

                LogoPath = company.LogoPath,
                ImzaDocumentPath = company.ImzaDocumentPath,
                FaaliyetBelgesiPath = company.FaaliyetBelgesiPath,
                OdaSicilKaydiPath = company.OdaSicilKaydiPath,
                TicaretSicilGazetesiPath = company.TicaretSicilGazetesiPath,
                VergiLevhasıPath = company.VergiLevhasıPath,
                SigortaBelgesiPath = company.SigortaBelgesiPath,
                HizmetDetayiPath = company.HizmetDetayiPath,
                AracD2BelgesiPath = company.AracD2belgesiPath,
                SportifFaaliyetBelgesiPath = company.SportifFaaliyetBelgesiPath
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(TourCompanyViewModel model)
        {
            
                var company = await _context.TourCompanies.FindAsync(model.Id);
                if (company == null) return NotFound();

                company.CompanyName = model.CompanyName;
                company.AuthorizedPerson = model.AuthorizedPerson;
                company.Email = model.Email;
                company.Phone = model.Phone;

                // Sadece dosya gelirse güncelle
                company.LogoPath = model.Logo != null ? await SaveFile(model.Logo, "logos") : company.LogoPath;
                company.ImzaDocumentPath = model.ImzaDocument != null ? await SaveFile(model.ImzaDocument, "docs") : company.ImzaDocumentPath;
                company.FaaliyetBelgesiPath = model.FaaliyetBelgesi != null ? await SaveFile(model.FaaliyetBelgesi, "docs") : company.FaaliyetBelgesiPath;
                company.OdaSicilKaydiPath = model.OdaSicilKaydi != null ? await SaveFile(model.OdaSicilKaydi, "docs") : company.OdaSicilKaydiPath;
                company.TicaretSicilGazetesiPath = model.TicaretSicilGazetesi != null ? await SaveFile(model.TicaretSicilGazetesi, "docs") : company.TicaretSicilGazetesiPath;
                company.VergiLevhasıPath = model.VergiLevhası != null ? await SaveFile(model.VergiLevhası, "docs") : company.VergiLevhasıPath;
                company.SigortaBelgesiPath = model.SigortaBelgesi != null ? await SaveFile(model.SigortaBelgesi, "docs") : company.SigortaBelgesiPath;
                company.HizmetDetayiPath = model.HizmetDetayi != null ? await SaveFile(model.HizmetDetayi, "docs") : company.HizmetDetayiPath;
                company.AracD2belgesiPath = model.AracD2Belgesi != null ? await SaveFile(model.AracD2Belgesi, "docs") : company.AracD2belgesiPath;
                company.SportifFaaliyetBelgesiPath = model.SportifFaaliyetBelgesi != null ? await SaveFile(model.SportifFaaliyetBelgesi, "docs") : company.SportifFaaliyetBelgesiPath;

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
        }

        private async Task<string> SaveFile(IFormFile file, string folder)
        {
            if (file == null) return null;
            var uploadDir = Path.Combine(_env.WebRootPath, "uploads", folder);
            Directory.CreateDirectory(uploadDir);
            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(uploadDir, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return $"/uploads/{folder}/{fileName}";
        }
    }
}