using Microsoft.EntityFrameworkCore;
using MudBlazor;
using MudBlazor.Services;
using System.Globalization;
using Ti.Pm.PmDb;
using Ti.Pm.Web.Data.Service;

namespace Ti.Pm.Web
{
    public class Startup
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _hostingEnvironment = env;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            /*var builder = WebApplication.CreateBuilder();*/

            services.AddControllersWithViews();
            services.AddMvc(options => options.EnableEndpointRouting = false);
            services.AddMudServices();
            services.AddScoped<MudThemeProvider>();           
            services.AddRazorPages();
            services.AddServerSideBlazor();

            Thread.CurrentThread.CurrentCulture = new CultureInfo("ru-RU");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru-RU");

            //services.AddAppConfig(Configuration);
            //services.AddLocalStorage();
            //Blazored.LocalStorage.ServiceCollectionExtensions.AddBlazoredLocalStorage(services);
            services.AddServerSideBlazor().AddCircuitOptions(options => { options.DetailedErrors = true; });
           
            services.AddControllersWithViews();
            services.AddHttpContextAccessor();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddHttpClient();
            services.AddScoped(r =>
            {
                var client = new HttpClient(new HttpClientHandler()
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator //()=> { true}
                });
                return client;
            });
           

            services.AddDbContext<TiPmDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("TiPmDbContext")));           
            services.AddLogging();
            //services.AddServerLogger();
            services.AddScoped<LogApplicationService>();
            services.AddScoped<ProjectPmService>();
            services.AddScoped<TaskTypePmService>();
            services.AddScoped<StatusPmService>();
            services.AddScoped<TaskPmService>();
        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }
            app.UsePathBase("/backend");

            app.UseStaticFiles();

            app.UseRequestLocalization();

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {


                endpoints.MapControllers();
                endpoints.MapControllerRoute(
            name: "default",
            pattern: "api/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });

        }
    }
}
