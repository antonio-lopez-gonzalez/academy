// Tarjeta de perfil: el componente estrella del Módulo 1.
// Recibe UN objeto "user" por props (lección 05) y lo pinta.
export default function UserCard({ user }) {
  // Dato derivado, calculado FUERA del JSX para mantenerlo limpio
  const statusText = user.isOnline ? "En línea" : "Desconectada";

  return (
    <article className="user-card">
      {/* Las llaves insertan JavaScript dentro del JSX */}
      <img
        className="user-card__avatar"
        src={user.avatarUrl}
        alt={"Avatar de " + user.name}
      />
      <h2 className="user-card__name">{user.name}</h2>
      <p className="user-card__role">{user.role}</p>

      {/* Estilo inline calculado: objeto JS con propiedades camelCase */}
      <span
        className="user-card__status"
        style={{ color: user.isOnline ? "#3fb950" : "#8b949e" }}
      >
        ● {statusText}
      </span>
    </article>
  );
}
