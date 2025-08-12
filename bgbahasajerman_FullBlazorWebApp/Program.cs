using bgbahasajerman_FullBlazorWebApp.Client.Pages;
using bgbahasajerman_FullBlazorWebApp.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

// API URL configuration - works for both Server and WebAssembly modes
string apiBaseUrl;
if (Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true")
{
    Console.WriteLine("Running in Docker");
    // For Server components: use internal Docker network
    apiBaseUrl = "http://bgbj-dataaccess-api:80/";
}
else
{
    Console.WriteLine("Running locally from Visual Studio");
    // Local development: use Tailscale IP
    apiBaseUrl = "http://100.117.149.44:8090/";
}

// HttpClient for Server-side components
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(apiBaseUrl),
    Timeout = TimeSpan.FromSeconds(30)
});

// Additional HttpClient specifically for WebAssembly components
builder.Services.AddHttpClient("WebAssemblyAPI", client =>
{
    client.BaseAddress = new Uri("http://100.117.149.44:8090/");
    client.Timeout = TimeSpan.FromSeconds(30);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(bgbahasajerman_FullBlazorWebApp.Client._Imports).Assembly);

app.Run();