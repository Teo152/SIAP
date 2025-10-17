using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;
using lib_dominio.Entidades;
using lib_presentaciones.Interfaces;

namespace SkyBnB.Pages.Anfitrion
{
    public class IndexModel : PageModel
    {
        private readonly IPropiedadesPresentacion _propiedadesPresentacion;

        public IndexModel(IPropiedadesPresentacion propiedadesPresentacion)
        {
            _propiedadesPresentacion = propiedadesPresentacion;
        }

        public List<Propiedades> Propiedades { get; set; } = new();

        public async Task OnGetAsync()
        {
            // Carga las propiedades desde la BD
            Propiedades = await _propiedadesPresentacion.Listar();

            // Si tu capa usa un método con filtro por usuario, puedes usar algo así:
            // var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
            // Propiedades = await _propiedadesPresentacion.ObtenerPorAnfitrion(usuarioId);
        }
    }
}
