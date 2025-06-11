using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace TourManagementApi.Models.Common;

[Owned]
public class Contact
{
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Role { get; set; }
    public bool IsEmpty => string.IsNullOrEmpty(Name) && string.IsNullOrEmpty(Email) && string.IsNullOrEmpty(Phone) && string.IsNullOrEmpty(Role);
} 