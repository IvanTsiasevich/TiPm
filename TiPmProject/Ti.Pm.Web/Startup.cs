using Microsoft.EntityFrameworkCore;
using MudBlazor;
using MudBlazor.Services;
using System.Globalization;
using Ti.Pm.PmDb;
using Ti.Pm.Web.Data;
using Ti.Pm.Web.Data.Service;
using Ti.Pm.Web.Data.Services;
using Ti.Pm.Web.Data.SharedServices;

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
            services.AddControllersWithViews();

            services.AddAuthentication(
                        ConstField.CookieScheme)
                        .AddCookie(ConstField.CookieScheme,
                        op => { op.LoginPath = "/login"; });

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

            services.AddAuthorization(config => 
            {
                config.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
                config.AddPolicy("AuthUserRoles", policy => policy.RequireRole("Admin", "User"));
            });


            services.AddScoped<LogApplicationService>();
            services.AddScoped<ProjectPmService>();
            services.AddScoped<TaskTypePmService>();
            services.AddScoped<StatusPmService>();
            services.AddScoped<TaskPmService>();
            services.AddScoped<CreateDialogOptionService>();
            services.AddScoped<UserService>();
            services.AddScoped<SecurityService>();

            services.AddOptions();
            services.AddAuthorizationCore();
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

            app.UseAuthorization();
            app.UseAuthentication();
    
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapControllerRoute("default","api/{controller=Account}/{action=Index}/{id?}");
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });

        }
    }
}
