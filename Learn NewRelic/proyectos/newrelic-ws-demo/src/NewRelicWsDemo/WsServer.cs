// ============================================================================
// Comunicación con el socket: recibe JSON, construye MessageEnvelope y
// delega en MessageDispatcher. La ESPERA del siguiente frame queda FUERA de
// la transacción (M4-02): la transacción mide el procesado, no la espera.
// ============================================================================

using System.Net.WebSockets;
using System.Text;
using NewRelicWsDemo.Handlers;
using NewRelicWsDemo.Messages;

namespace NewRelicWsDemo;

public static class WsServer
{
    public static async Task HandleAsync(WebSocket socket, MessageDispatcher dispatcher)
    {
        var buffer = new byte[4 * 1024];

        while (socket.State == WebSocketState.Open)
        {
            // 1) Espera BLOQUEANTE del siguiente frame (fuera de la transacción)
            var result = await socket.ReceiveAsync(buffer, CancellationToken.None);
            if (result.MessageType == WebSocketMessageType.Close) break;

            // 2) Convierte los bytes que llegaron en texto y a envelope
            var json = Encoding.UTF8.GetString(buffer, 0, result.Count);

            try
            {
                var env = MessageEnvelope.Parse(json);
                // 3) Transacción del mensaje aquí (M4-02)
                dispatcher.OnMessage(env);
            }
            catch (Exception ex) when (ex is System.Text.Json.JsonException or InvalidDataException)
            {
                // Un JSON roto no debe tirar abajo el loop
                dispatcher.OnMalformed(ex);
            }
        }
    }
}