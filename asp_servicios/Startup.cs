using asp_servicios.Controllers;
using lib_aplicaciones.Implementaciones;
using lib_aplicaciones.Interfaces;
using lib_repositorios;
using lib_repositorios.Implementaciones;
using lib_repositorios.Interfaces;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore; // <� IMPORTANTE

namespace asp_servicios
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public static IConfiguration? Configuration { set; get; }

        // ? Firma correcta: solo IServiceCollection
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<KestrelServerOptions>(x => { x.AllowSynchronousIO = true; });
            services.Configure<IISServerOptions>(x => { x.AllowSynchronousIO = true; });

            services.AddControllers();
            services.AddEndpointsApiExplorer();
            // services.AddSwaggerGen();

            // ? Lee la cadena y registra tu DbContext REAL: Conexion
            var cs = Configuration!.GetConnectionString("DefaultConnection")
                     ?? throw new InvalidOperationException("Falta ConnectionStrings:DefaultConnection en appsettings*.json");

            services.AddDbContext<Conexion>(opt => opt.UseSqlServer(cs)); // <� aqu� va tu contexto

            // Resto de servicios
            services.AddScoped<IConexion, Conexion>();
            services.AddScoped<IPropiedadesAplicacion, PropiedadesAplicacion>();
            services.AddScoped<IUsuariosAplicacion, UsuariosAplicacion>();
            services.AddScoped<TokenController, TokenController>();
            services.AddScoped<IBusquedasAplicacion, BusquedasAplicacion>();

            services.AddCors(o => o.AddDefaultPolicy(b => b.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                // app.UseSwagger();
                // app.UseSwaggerUI();
            }

            // ? Orden correcto del pipeline
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors();
            // app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}