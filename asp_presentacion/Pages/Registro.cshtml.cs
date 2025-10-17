using System;
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

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            // Guardar usuario en la BD
            await _usuariosPresentacion.Guardar(Usuario);

            // Redirigir al login después del registro
            return RedirectToPage("/Login");
        }
    }
}
