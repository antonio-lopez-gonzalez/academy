export type EstadoTarea = "pendiente" | "en-progreso" | "completada";

export interface Tarea {
  id: string;
  titulo: string;
  descripcion: string;
  estado: EstadoTarea;
  creadaEn: string;
}