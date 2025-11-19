using Microsoft.AspNetCore.SignalR;

public class UserIdProvider : IUserIdProvider
{
    public string GetUserId(HubConnectionContext connection)
    {
        var httpContext = connection.GetHttpContext();
        var usuarioId = httpContext?.Session.GetInt32("UsuarioId");
        return usuarioId?.ToString();
    }
}