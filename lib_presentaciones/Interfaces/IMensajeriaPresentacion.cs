using lib_dominio.Entidades;

namespace lib_presentaciones.Interfaces;

public interface IMensajeriaPresentacion
{
    Task<Mensajes> Enviar(int reservaId, Mensajes entidad);

    Task<List<Mensajes>> ListarConversacion(int usuario1Id, int usuario2Id, int reservaId);

    Task<int> ContarNoLeidos(int usuarioDestinoId, int otroUsuarioId, int reservaId);

    Task MarcarComoLeidos(int usuarioDestinoId, int otroUsuarioId, int reservaId);

    Task<bool> AdminPuedeIngresar(int reservaId);

    Task<List<Mensajes>> AdminListarConversacion(int reservaId);

    Task<Mensajes> AdminEnviar(int reservaId, int adminId, string texto);
}