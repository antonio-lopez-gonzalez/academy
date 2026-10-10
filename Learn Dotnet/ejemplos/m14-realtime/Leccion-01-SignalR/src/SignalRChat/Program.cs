// ─── MÓDULO 14 ─── Tiempo Real ─── SignalR ───
// SignalR: abstracción sobre WebSockets con fallback automático
// Soporta: WebSockets → Server-Sent Events → Long Polling (en orden de preferencia)
// Hubs: punto central de comunicación bidireccional servidor↔cliente
// Groups: agrupar conexiones (salas de chat, notificaciones por rol)
// Redis Backplane: escalar SignalR horizontalmente entre múltiples servidores

using Microsoft.AspNetCore.SignalR;

// ─── Configuración del WebApplication ───
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Registrar SignalR
builder.Services.AddSignalR();

// Servir archivos estáticos (para el HTML del cliente)
WebApplication app = builder.Build();

// Servir archivos estáticos (wwwroot/)
app.UseStaticFiles();

// ─── Mapear el Hub de SignalR ───
// El cliente se conectará a wss://localhost:5000/chat
app.MapHub<ChatHub>("/chat");

// ─── Endpoint para servir el cliente HTML ───
app.MapGet("/", () => Results.Content(
    """
    <!DOCTYPE html>
    <html lang="es">
    <head>
        <meta charset="UTF-8">
        <title>SignalR Chat - Módulo 14</title>
        <style>
            body{font-family:monospace;max-width:600px;margin:2rem auto;background:#111;color:#0f0}
            input,button{padding:.5rem;margin:.25rem;font-family:monospace;background:#222;color:#0f0;border:1px solid #0f0}
            #messages{height:300px;overflow-y:auto;border:1px solid #0f0;padding:.5rem;margin-bottom:1rem}
        </style>
    </head>
    <body>
        <h1>SignalR Chat</h1>
        <input id="userInput" placeholder="Tu nombre..." value="Alumno">
        <input id="messageInput" placeholder="Escribe un mensaje...">
        <button onclick="send()">Enviar</button>
        <div id="messages"></div>

        <!-- CDN de SignalR JavaScript client -->
        <script src="https://cdnjs.cloudflare.com/ajax/libs/microsoft-signalr/8.0.0/signalr.min.js"></script>
        <script>
            // Construir conexión al Hub "/chat"
            const connection = new signalR.HubConnectionBuilder()
                .withUrl("/chat")
                .configureLogging(signalR.LogLevel.Information)
                .build();

            // Escuchar mensajes del servidor
            connection.on("ReceiveMessage", (user, message) => {
                const div = document.createElement("div");
                div.textContent = `[${new Date().toLocaleTimeString()}] ${user}: ${message}`;
                document.getElementById("messages").appendChild(div);
            });

            // Notificación cuando alguien se une
            connection.on("UserJoined", (user) => {
                const div = document.createElement("div");
                div.style.color = "#ff0";
                div.textContent = `→ ${user} se ha unido al chat`;
                document.getElementById("messages").appendChild(div);
            });

            // Notificación cuando alguien sale
            connection.on("UserLeft", (user) => {
                const div = document.createElement("div");
                div.style.color = "#f66";
                div.textContent = `← ${user} ha salido del chat`;
                document.getElementById("messages").appendChild(div);
            });

            // Iniciar conexión
            connection.start().then(() => {
                console.log("Conectado a SignalR");
            }).catch(err => console.error(err));

            // Enviar mensaje
            function send() {
                const user = document.getElementById("userInput").value;
                const message = document.getElementById("messageInput").value;
                if (message) {
                    connection.invoke("SendMessage", user, message);
                    document.getElementById("messageInput").value = "";
                }
            }

            // Enviar con Enter
            document.getElementById("messageInput").addEventListener("keypress", (e) => {
                if (e.key === "Enter") send();
            });
        </script>
    </body>
    </html>
    """, "text/html"));

app.Run();


// ─── HUB de SignalR ───
public class ChatHub : Hub
{
    // Diccionario estático para trackear usuarios conectados
    private static readonly Dictionary<string, string> ConnectedUsers = new();

    // ─── Enviar mensaje a TODOS los clientes ───
    // Invocado desde el cliente: connection.invoke("SendMessage", user, message)
    public async Task SendMessage(string user, string message)
    {
        // Clients.All: envía el mensaje a TODOS los clientes conectados
        // SendAsync("ReceiveMessage", ...): invoca el método "ReceiveMessage" en el cliente
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }

    // ─── Enviar mensaje a un usuario específico ───
    public async Task SendPrivateMessage(string targetConnectionId, string message)
    {
        string sender = ConnectedUsers.GetValueOrDefault(Context.ConnectionId, "Anónimo");

        // Clients.Client(connectionId): envía SOLO a esa conexión específica
        await Clients.Client(targetConnectionId).SendAsync("ReceiveMessage", $"{sender} (privado)", message);
    }

    // ─── Unirse a un grupo (salas de chat, canales) ───
    public async Task JoinGroup(string groupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

        // Clients.Group(groupName): envía a todos los miembros del grupo
        await Clients.Group(groupName).SendAsync("ReceiveMessage", "Sistema", $"{Context.ConnectionId} se unió al grupo {groupName}");

        // ─── ESCALAR CON REDIS BACKPLANE ───
        // Cuando tienes múltiples instancias de tu API, SignalR usa Redis para sincronizar:
        //
        // Program.cs:
        // builder.Services.AddSignalR().AddStackExchangeRedis("localhost:6379", options =>
        // {
        //     options.Configuration.ChannelPrefix = "SignalR_";
        // });
        //
        // Cada servidor publica los mensajes en Redis, y los demás servidores
        // los reciben y reenvían a sus clientes locales. Así todos los clientes
        // reciben los mensajes sin importar a qué servidor están conectados.
    }

    public async Task LeaveGroup(string groupName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        await Clients.Group(groupName).SendAsync("ReceiveMessage", "Sistema", $"{Context.ConnectionId} salió del grupo {groupName}");
    }

    // ─── OnConnectedAsync: se ejecuta cuando un cliente se conecta ───
    public override async Task OnConnectedAsync()
    {
        string? userName = Context.GetHttpContext()?.Request.Query["username"];
        ConnectedUsers[Context.ConnectionId] = userName ?? "Anónimo";

        // Notificar a todos EXCEPTO al que se conectó
        await Clients.Others.SendAsync("UserJoined", userName ?? "Anónimo");

        await base.OnConnectedAsync();
    }

    // ─── OnDisconnectedAsync: se ejecuta cuando un cliente se desconecta ───
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        string userName = ConnectedUsers.GetValueOrDefault(Context.ConnectionId, "Anónimo");
        ConnectedUsers.Remove(Context.ConnectionId);

        await Clients.Others.SendAsync("UserLeft", userName);
        await base.OnDisconnectedAsync(exception);
    }

    /*
    ─── COMPARACIÓN: WebSockets vs SignalR vs SSE ───

    | Característica          | WebSockets vanilla | SignalR           | SSE                  |
    |-------------------------|---------------------|--------------------|----------------------|
    | Comunicación            | Bidireccional       | Bidireccional      | Servidor → Cliente   |
    | Protocolo               | ws:// / wss://      | ws:// (con fallback)| HTTP                 |
    | Fallback automático     | ❌                  | ✅ (SSE, LP)       | ❌                   |
    | Reconexión automática   | ❌                  | ✅                  | ✅ (nativa)          |
    | Soporte de grupos       | ❌                  | ✅                  | ❌                   |
    | Escalado horizontal     | Manual              | Redis Backplane    | Manual               |
    | Manejo de mensajes      | Manual (buffers)    | Automático (JSON)  | EventSource API      |
    | Cuándo usar             | Máximo control      | 90% de los casos   | Solo notificaciones  |
    |                         | Juegos, IoT         | Apps empresariales  | Unidireccionales     |
    */
}
