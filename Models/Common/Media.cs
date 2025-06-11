using Microsoft.EntityFrameworkCore;

namespace TourManagementApi.Models.Common;

[Owned]
public class Media
{
    public MediaImages Images { get; set; } = new();
    public List<string> Videos { get; set; } = new();
}

[Owned]
public class MediaImages
{
    public string? Header { get; set; }
    public string? Teaser { get; set; }
    public List<string> Gallery { get; set; } = new();
} 