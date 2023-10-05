using Microsoft.EntityFrameworkCore;
using MudBlazor;
using MudBlazor.Services;
using System.Globalization;
using Ti.Pm.PmDb;
using Ti.Pm.Web.Data.Service;
using Ti.Pm.Web.Data.Services;

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
            services.AddControllersWithViews();
            services.AddMvc(options => options.EnableEndpointRouting = false);
            services.AddMudServices();
            services.AddScoped<MudThemeProvider>();           
            services.AddRazorPages();
            services.AddServerSideBlazor();

            Thread.CurrentThread.CurrentCulture = new CultureInfo("ru-RU");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru-RU");
            
            services.AddServerSideBlazor().AddCircuitOptions(options => { options.DetailedErrors = true; });         
            services.AddControllersWithViews();
            services.AddHttpContextAccessor();
            services.AddLogging();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddHttpClient();
            services.AddScoped(r =>
            {
                var client = new HttpClient(new HttpClientHandler()
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator 
                });
                return client;
            });


            services.AddDbContext<TiPmDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("TiPmDbContext")));           
            services.AddScoped<LogApplicationService>();
            services.AddScoped<ProjectPmService>();
            services.AddScoped<TaskTypePmService>();
            services.AddScoped<StatusPmService>();
            services.AddScoped<TaskPmService>();
            services.AddScoped<CreateDialogOptionService>();
            services.AddScoped<UserService>();
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
