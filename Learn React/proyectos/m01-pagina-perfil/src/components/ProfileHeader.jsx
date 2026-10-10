import Avatar from "./Avatar.jsx";
import Badge from "./Badge.jsx";

// Cabecera del perfil: avatar + datos + estado de conexión
export default function ProfileHeader({ user }) {
  return (
    <header className="profile-header">
      <Avatar src={user.avatarUrl} alt={user.name} size={96} />
      <div>
        <h1 className="profile-header__name">{user.name}</h1>
        <p className="profile-header__role">{user.role}</p>
        <p className="profile-header__location">📍 {user.location}</p>
        <Badge online={user.isOnline} />
      </div>
    </header>
  );
}
