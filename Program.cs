using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Globalization;
using TourManagementApi.Data;
using TourManagementApi.Helper;
using TourManagementApi.Middleware;
using TourManagementApi.Models;
using TourManagementApi.Services;

var builder = WebApplication.CreateBuilder(args);
var cultureInfo = new CultureInfo("en-US");
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation();

builder.Services.AddEndpointsApiExplorer();

// Configure Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Tour Management API", 
        Version = "v1",
        Description = "API for managing tours and activities"
    });
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN";
});
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 1024 * 1024 * 100; // 100 MB
});
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = 1024 * 1024 * 100; // 100 MB
});

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.File("Logs/log-.txt",
                  rollingInterval: RollingInterval.Day,
                  outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.Logger(lc => lc
        .Filter.ByIncludingOnly(le =>
            le.Properties.ContainsKey("SourceContext") &&
            le.Properties["SourceContext"].ToString().Contains("GlobalExceptionMiddleware"))
        .WriteTo.File("Logs/hatalar-.log",
                      rollingInterval: RollingInterval.Day,
                      outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    )
    .CreateLogger();


builder.Host.UseSerilog();
// Add SQL Server Configuration
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure())
    .EnableSensitiveDataLogging()
    .LogTo(Console.WriteLine, LogLevel.Information));

builder.Services.AddScoped<ExperienceBankService>();
builder.Services.Configure<ExperienceBankSettings>(builder.Configuration.GetSection("ExperienceBank"));
builder.Services.AddHttpClient();
builder.Services.AddScoped<ExperienceBankService>();
builder.Services.AddScoped<AuthorizationHeaderHelper>();

var app = builder.Build();

// Otomatik veritabanı oluşturma ve migration'ları uygulama
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        // Sadece migration'ları uygula, veritabanını silme
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Veritabanı güncellenirken bir hata oluştu.");
    }
}

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseDeveloperExceptionPage();
//    app.UseSwagger();
//    app.UseSwaggerUI(c =>
//    {
//        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Tour Management API V1");
//        c.RoutePrefix = "swagger";
//    });
//}
//else
//{
//    app.UseExceptionHandler("/Home/Error");
//    app.UseHsts();
//}

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// SWAGGER HER ORTAMDA AKTİF
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Tour Management API V1");
    c.RoutePrefix = "swagger";
});


app.UseStaticFiles();
app.UseRouting();

// Frame Policy Middleware
app.Use(async (context, next) =>
{
    var allowedOrigins = new[]
    {
        "https://b2b.hotelwidget.com",
        "http://localhost:44392"
    };

    context.Response.Headers.Remove("X-Frame-Options");

    // Frame-ancestors policy'yi oluştur
    var policy = $"frame-ancestors 'self' {string.Join(" ", allowedOrigins)}";
    context.Response.Headers["Content-Security-Policy"] = policy;

    await next();
});
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseStatusCodePages("text/plain", "Status Code: {0}");
app.UseAuthorization();
app.UseMiddleware<RequestHeaderValidationMiddleware>();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=ActivitiesWizard}/{action=Index}/{id?}");

// Configure custom URLs
builder.WebHost.UseUrls("http://localhost:5050");

app.Run(); 