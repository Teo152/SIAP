using lib_dominio.Entidades;

namespace lib_aplicaciones.Interfaces;

public interface IMensajeriaAplicacion
{
    void Configurar(string StringConexion);

    Mensajes? Enviar(Mensajes? entidad, int reservaId);

    List<Mensajes> ListarConversacion(int usuario1Id, int usuario2Id, int reservaId);

    Mensajes? Modificar(Mensajes? entidad);

    Mensajes? Borrar(Mensajes? entidad);

    int ContarNoLeidos(int usuarioDestinoId, int otroUsuarioId, int reservaId);

    void MarcarComoLeidos(int usuarioDestinoId, int otroUsuarioId, int reservaId);

    bool AdminPuedeIngresar(int reservaId);

    List<Mensajes> AdminListarConversacion(int reservaId);

    Mensajes AdminEnviarMensaje(int reservaId, int adminId, string texto);
}