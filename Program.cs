using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Globalization;
using System.Net.Http.Headers;
using TourManagementApi.Data;
using TourManagementApi.Helper;
using TourManagementApi.Middleware;
using TourManagementApi.Models;
using TourManagementApi.Services;
// <<< Rezdy Integration <<<
using TourManagementApi.Services.Rezdy; // (3) ProductService ve BookingService
using static System.Net.WebRequestMethods;
// >>> Rezdy Integration >>>


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
    c.SwaggerDoc("rezdy", new OpenApiInfo
    {
        Title = "RezdyConnect API",
        Version = "v1",
        Description = "Rezdy Channel Manager entegrasyonu için endpointler"
    });

    c.SwaggerDoc("experiencebank", new OpenApiInfo
    {
        Title = "ExperienceBank API",
        Version = "v1",
        Description = "ExperienceBank entegrasyonu için endpointler"
    });

    c.DocInclusionPredicate((documentName, apiDesc) =>
    {
        if (!apiDesc.TryGetMethodInfo(out var methodInfo))
            return false;

        var groupName = methodInfo.DeclaringType?
            .GetCustomAttributes(typeof(ApiExplorerSettingsAttribute), true)
            .Cast<ApiExplorerSettingsAttribute>()
            .FirstOrDefault()?.GroupName;

        return groupName == documentName;
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
builder.Services.AddSession();

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()

    // Genel log
    .WriteTo.File("Logs/general-.log", rollingInterval: RollingInterval.Day)

    // Sadece API requestleri
    .WriteTo.Logger(lc => lc
        .Filter.ByIncludingOnly(le =>
            le.Properties.ContainsKey("RequestType") &&
            le.Properties["RequestType"].ToString().Contains("API"))
        .WriteTo.File("Logs/api-.log", rollingInterval: RollingInterval.Day))

    // Sadece MVC requestleri
    .WriteTo.Logger(lc => lc
        .Filter.ByIncludingOnly(le =>
            le.Properties.ContainsKey("RequestType") &&
            le.Properties["RequestType"].ToString().Contains("MVC"))
        .WriteTo.File("Logs/mvc-.log", rollingInterval: RollingInterval.Day))

    // Exception logları
    .WriteTo.Logger(lc => lc
        .Filter.ByIncludingOnly(Matching.WithProperty<string>("SourceContext", sc => sc.Contains("GlobalExceptionMiddleware")))
        .WriteTo.File("Logs/errors-.log", rollingInterval: RollingInterval.Day))

    .CreateLogger();



builder.Host.UseSerilog();
// Add SQL Server Configuration
builder.Services.AddDbContext<TourManagementDbContext>(options =>
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

// REZDY CONNECTION
builder.Services.AddScoped<IProductService, TourManagementApi.Services.ProductService>();
builder.Services.AddScoped<AvailabilityService>();
builder.Services.AddScoped<TourManagementApi.Services.BookingService>();

// <<< Rezdy Integration <<<
// 1. Rezdy ayarlarını appsettings.json’dan okumaca:
builder.Services.Configure<RezdySettings>(builder.Configuration.GetSection("Rezdy"));

// 2. Rezdy API client’ını HttpClientFactory üzerinden kaydedelim:
builder.Services.AddHttpClient<IRezdyApiClient, RezdyApiClient>(client =>
{
    var rezdy = builder.Configuration.GetSection("Rezdy");
    var baseUrl = rezdy["BaseUrl"]!;
    var apiKey = rezdy["ApiKey"]!;

    if (!baseUrl.StartsWith("https://"))
        throw new InvalidOperationException("Rezdy BaseUrl must use HTTPS");

    client.BaseAddress = new Uri(baseUrl);
    client.DefaultRequestHeaders.Add("apiKey", apiKey);

    // 🔑 Payload format headers
    client.DefaultRequestHeaders.Accept.Clear();
    client.DefaultRequestHeaders.Accept
          .Add(new MediaTypeWithQualityHeaderValue("application/json"));
});
// 3. Rezdy’ye özel servislerimizi ekleyelim:
builder.Services.AddScoped<TourManagementApi.Services.Rezdy.ProductService>();
builder.Services.AddScoped<TourManagementApi.Services.Rezdy.BookingService>();
// >>> Rezdy Integration >>>

var app = builder.Build();

// Otomatik veritabanı oluşturma ve migration'ları uygulama
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<TourManagementDbContext>();
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
    c.SwaggerEndpoint("/swagger/rezdy/swagger.json", "RezdyConnect API");
    c.SwaggerEndpoint("/swagger/experiencebank/swagger.json", "ExperienceBank API");
    c.RoutePrefix = "swagger";
});


app.UseStaticFiles();
app.UseRouting();
app.UseSession();
// Frame Policy Middleware
//app.Use(async (context, next) =>
//{
//    var allowedOrigins = new[]
//    {
//        "https://b2b.hotelwidget.com",
//        "http://localhost:44392",
//        "https://tour.hotelwidget.com/"
//    };

//    context.Response.Headers.Remove("X-Frame-Options");

//    // Frame-ancestors policy'yi oluştur
//    var policy = $"frame-ancestors 'self' {string.Join(" ", allowedOrigins)}";
//    context.Response.Headers["Content-Security-Policy"] = policy;

//    await next();
//});
app.UseMiddleware<RequestResponseLoggingMiddleware>();
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseStatusCodePages("text/plain", "Status Code: {0}");
app.UseAuthorization();
//app.UseMiddleware<RequestHeaderValidationMiddleware>();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Activities}/{action=Index}/{id?}");

// <<< Rezdy Integration <<<
// Webhook endpoint’inizi buraya yönlendirin:
app.MapControllerRoute(
    name: "rezdy-webhook",
    pattern: "api/webhooks/rezdy",
    defaults: new { controller = "RezdyWebhook", action = "Handle" });
//
// Mevcut default route’unuz:
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Activities}/{action=Index}/{id?}");
// >>> Rezdy Integration >>>

// Configure custom URLs
builder.WebHost.UseUrls("http://localhost:5050");

app.Run(); 