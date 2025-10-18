using System.Threading.Tasks;
using lib_dominio.Entidades;
using lib_presentaciones.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace asp_presentacion.Pages
{
    public class RegistroModel : PageModel
    {
        private readonly IUsuariosPresentacion _usuariosPresentacion;

        public RegistroModel(IUsuariosPresentacion usuariosPresentacion)
        {
            _usuariosPresentacion = usuariosPresentacion;
        }

        [BindProperty]
        public Usuarios Usuario { get; set; } = new();

        public void OnGet(int? rol)
        {
            // Si viene de la página de rol, asigna el valor (1 = anfitrión, 2 = huésped)
            if (rol.HasValue)
            {
                Usuario.Rol = (RolUsuario)rol.Value;
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            try
            {
                await _usuariosPresentacion.Guardar(Usuario);
                return RedirectToPage("/Anfitrion");
            }
            catch (Exception ex)
            {
                // Puedes mostrar el error en la página
                ModelState.AddModelError(string.Empty, $"Error: {ex.Message}");
                return Page();
            }
        }
    }
}