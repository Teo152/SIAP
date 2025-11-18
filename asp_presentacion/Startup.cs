using lib_aplicaciones.Implementaciones;
using lib_aplicaciones.Interfaces;
using lib_presentaciones.Implementaciones;
using lib_presentaciones.Interfaces;

namespace asp_presentacion
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // ✔ SOLO IServiceCollection aquí
        public void ConfigureServices(IServiceCollection services)
        {
            // Servicios Presentación
            services.AddScoped<IUsuariosPresentacion, UsuariosPresentacion>();
            services.AddScoped<IMunicipiosPresentacion, MunicipiosPresentacion>();
            services.AddScoped<IBusquedasPresentacion, BusquedasPresentacion>();
            services.AddScoped<IPropiedadesPresentacion, PropiedadesPresentacion>();
            services.AddScoped<IReservasPresentacion, ReservasPresentacion>();
            services.AddScoped<IMensajeriaPresentacion, MensajeriaPresentacion>();
            services.AddScoped<IPagosPresentacion, PagosPresentacion>();
            services.AddScoped<IReportesChatPresentacion, ReportesChatPresentacion>();

            // Servicios ASP.NET
            services.AddRazorPages();
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddHttpContextAccessor();

            // Sesión
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
            });
        }

        // ✔ Este Configure es el correcto para .NET 6+
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (!env.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
            });
        }
    }
}