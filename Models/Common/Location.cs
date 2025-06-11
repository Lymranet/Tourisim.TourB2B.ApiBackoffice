using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace TourManagementApi.Models.Common;

[Owned]
public class Location
{
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public Coordinates? Coordinates { get; set; }
    public bool IsEmpty => string.IsNullOrEmpty(Address) && string.IsNullOrEmpty(City) && string.IsNullOrEmpty(Country);
}

[Owned]
public class Coordinates
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
} 