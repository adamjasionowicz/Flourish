using Ardalis.ListStartupServices;
using Autofac;
using Autofac.Core;
using Autofac.Extensions.DependencyInjection;
using Flourish.Core;
using Flourish.Infrastructure;
using Flourish.Infrastructure.Data;
using Flourish.Web;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.FeatureManagement;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

//Retrieve the Connection String from the secrets manager 
//Secret Manager is used only to test the web app locally.
//When the app is deployed to Azure App Service, use the Connection Strings application setting in App Service instead of Secret Manager to store the connection string.
var appConfigConnectionString = builder.Configuration.GetConnectionString("AppConfig");

builder.Host.ConfigureAppConfiguration(builder =>
{
  // connect to App Config Store using connection string
  builder.AddAzureAppConfiguration(options =>
    options.Connect(appConfigConnectionString)
            .ConfigureRefresh(refresh =>
            {
              refresh.Register("TestApp:Settings:Sentinel", refreshAll: true)
                     .SetCacheExpiration(new TimeSpan(0, 0, 5)); // AJ change to minutes in prod
            })
           .UseFeatureFlags(flagOptions =>
           {
             flagOptions.CacheExpirationInterval = TimeSpan.FromSeconds(5);
           }));
});

builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

builder.Host.UseSerilog((_, config) => config.ReadFrom.Configuration(builder.Configuration));

builder.Services.Configure<CookiePolicyOptions>(options =>
{
  options.CheckConsentNeeded = context => true;
  options.MinimumSameSitePolicy = SameSiteMode.None;
});

var dbContextConnectionString = builder.Configuration.GetConnectionString("SqliteConnection");  //Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext(dbContextConnectionString);

builder.Services.AddControllersWithViews().AddNewtonsoftJson();
builder.Services.AddRazorPages();

//allows feature flag values to be refreshed at a recurring interval while app continues to receive requests.
builder.Services.AddAzureAppConfiguration();
builder.Services.AddFeatureManagement(); // register IFeatureManager in IoC container

builder.Services.AddSwaggerGen(c =>
{
  c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
  c.EnableAnnotations();
});

// add list services for diagnostic purposes - see https://github.com/ardalis/AspNetCoreStartupServices
builder.Services.Configure<ServiceConfig>(config =>
{
  config.Services = new List<ServiceDescriptor>(builder.Services);

  // optional - default path to view services is /listallservices - recommended to choose your own path
  config.Path = "/listservices";
});

//Autofac
// Call UseServiceProviderFactory on the Host sub property 
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

// Call ConfigureContainer on the Host sub property 
builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
{
  containerBuilder.RegisterModule(new DefaultCoreModule());
  containerBuilder.RegisterModule(new DefaultInfrastructureModule(builder.Environment.EnvironmentName == "Development"));
});

builder.Logging.AddAzureWebAppDiagnostics();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.UseDeveloperExceptionPage();
  app.UseShowAllServicesMiddleware();
}
else
{
  app.UseExceptionHandler("/Home/Error");
  app.UseHsts();
}
app.UseRouting();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCookiePolicy();

//allows feature flag values to be refreshed at a recurring interval while app continues to receive requests.
app.UseAzureAppConfiguration();

// Enable middleware to serve generated Swagger as a JSON endpoint.
app.UseSwagger();

// Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"));

app.UseEndpoints(endpoints =>
{
  endpoints.MapDefaultControllerRoute();
  endpoints.MapRazorPages();
});

// Seed Database
using (var scope = app.Services.CreateScope())
{
  var services = scope.ServiceProvider;

  try
  {
    var context = services.GetRequiredService<AppDbContext>();
    //                    context.Database.Migrate();
    context.Database.EnsureCreated();
    SeedData.Initialize(services);
  }
  catch (Exception ex)
  {
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred seeding the DB. {exceptionMessage}", ex.Message);
  }
}

app.Run();
