using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace asp_presentacion.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            // Aquí puedes agregar la validación real del usuario
            if (Email == "admin" && Password == "1234")
            {
                // Ejemplo: redirigir al index si login correcto
                return RedirectToPage("/Anfitrion");
            }

            // Si es incorrecto, mostrar error (por ahora opcional)
            ModelState.AddModelError(string.Empty, "Credenciales incorrectas");
            return Page();
        }
    }
}
