using BookingWeb.Components;
using BookingWeb.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.Configure<SalonInfo>(builder.Configuration.GetSection(SalonInfo.SectionName));

// Typed HttpClients for the Booking API. Blazor Server calls the API from the server side,
// so the API does not need CORS and its address is never exposed to the browser.
var apiBaseUrl = builder.Configuration["BookingApi:BaseUrl"]
    ?? throw new InvalidOperationException("BookingApi:BaseUrl is not configured.");

void ConfigureApiClient(HttpClient client)
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(10);
}

builder.Services.AddHttpClient<BookingApiClient>(ConfigureApiClient);
builder.Services.AddHttpClient<AdminApiClient>(ConfigureApiClient);

// Admin token storage: one instance per browser tab (Blazor circuit)
builder.Services.AddScoped<AdminSession>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
