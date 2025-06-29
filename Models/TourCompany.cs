using System;
using System.Collections.Generic;

namespace TourManagementApi.Models;

public partial class TourCompany
{
    public int Id { get; set; }

    public string CompanyName { get; set; } = null!;

    public string AuthorizedPerson { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string? LogoPath { get; set; }

    public string? ImzaDocumentPath { get; set; }

    public string? FaaliyetBelgesiPath { get; set; }

    public string? OdaSicilKaydiPath { get; set; }

    public string? TicaretSicilGazetesiPath { get; set; }

    public string? VergiLevhasıPath { get; set; }

    public string? SigortaBelgesiPath { get; set; }

    public string? HizmetDetayiPath { get; set; }

    public string? AracD2belgesiPath { get; set; }

    public string? SportifFaaliyetBelgesiPath { get; set; }

    public virtual ICollection<Activity> Activities { get; set; } = new List<Activity>();
}
