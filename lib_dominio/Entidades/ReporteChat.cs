namespace lib_dominio.Entidades
{
    public class ReporteChat
    {
        public int Id { get; set; }
        public int ReservaId { get; set; }
        public int UsuarioReportanteId { get; set; }
        public string? Motivo { get; set; }
        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        // Navegación opcional
        public Reservas? Reserva { get; set; }

        public Usuarios? UsuarioReportante { get; set; }
    }
}