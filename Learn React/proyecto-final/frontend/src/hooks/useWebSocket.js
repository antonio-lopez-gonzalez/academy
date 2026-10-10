import { useEffect, useRef, useCallback } from "react";
import * as signalR from "@microsoft/signalr";

const HUB_URL = import.meta.env.VITE_HUB_URL || "/hubs/tasks";

export function useWebSocket(projectId, onMessage) {
  const connectionRef = useRef(null);

  useEffect(() => {
    const connection = new signalR.HubConnectionBuilder()
      .withUrl(HUB_URL)
      .withAutomaticReconnect()
      .build();

    connectionRef.current = connection;

    connection.on("TaskCreated", (task) =>
      onMessage({ type: "created", task })
    );
    connection.on("TaskUpdated", (task) =>
      onMessage({ type: "updated", task })
    );
    connection.on("TaskMoved", (task) => onMessage({ type: "moved", task }));
    connection.on("TaskDeleted", (taskId) =>
      onMessage({ type: "deleted", taskId })
    );

    async function start() {
      try {
        await connection.start();
        await connection.invoke("JoinProject", projectId);
      } catch (err) {
        console.error("Error conectando SignalR:", err);
      }
    }

    start();

    return () => {
      connection.invoke("LeaveProject", projectId).catch(() => {});
      connection.stop();
    };
  }, [projectId, onMessage]);

  const send = useCallback((methodName, ...args) => {
    connectionRef.current?.invoke(methodName, ...args);
  }, []);

  return { send };
}
