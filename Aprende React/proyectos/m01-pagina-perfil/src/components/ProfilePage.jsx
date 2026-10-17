import Card from "./Card.jsx";
import ProfileHeader from "./ProfileHeader.jsx";
import StatsBar from "./StatsBar.jsx";
import SkillList from "./SkillList.jsx";

// Datos de ejemplo: UN solo objeto, fuente de verdad de la página.
// En el Módulo 9 vendrá de una API; la estructura ya está preparada.
const user = {
  name: "Ana García",
  role: "Frontend Developer",
  location: "Madrid, España",
  avatarUrl: "https://i.pravatar.cc/150?img=47",
  isOnline: true,
  stats: [
    { value: 24, label: "repos" },
    { value: "1.2k", label: "seguidores" },
    { value: 312, label: "contribuciones" },
  ],
  skills: [
    { name: "JavaScript", level: 3 },
    { name: "React", level: 3 },
    { name: "CSS", level: 2 },
    { name: "Node", level: 1 },
  ],
};

export default function ProfilePage() {
  return (
    <main className="profile-page">
      {/* Card es agnóstica: solo aporta la caja visual */}
      <Card>
        <ProfileHeader user={user} />
      </Card>

      <Card title="Estadísticas">
        <StatsBar stats={user.stats} />
      </Card>

      <Card title="Habilidades">
        <SkillList skills={user.skills} />
      </Card>
    </main>
  );
}
