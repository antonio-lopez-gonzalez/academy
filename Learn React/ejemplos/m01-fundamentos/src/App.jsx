import UserCard from "./components/UserCard.jsx";

// Datos de ejemplo: en el Módulo 9 vendrán de una API.
const users = [
  {
    id: 1,
    name: "Ana García",
    role: "Frontend Developer",
    avatarUrl: "https://i.pravatar.cc/150?img=47",
    isOnline: true,
  },
  {
    id: 2,
    name: "Carlos Ruiz",
    role: "Backend Developer (.NET)",
    avatarUrl: "https://i.pravatar.cc/150?img=12",
    isOnline: false,
  },
  {
    id: 3,
    name: "Lucía Fernández",
    role: "UX Designer",
    avatarUrl: "https://i.pravatar.cc/150?img=32",
    isOnline: true,
  },
];

export default function App() {
  return (
    <main className="page">
      <h1>Fundamentos de React — Equipo</h1>
      <p className="page__subtitle">
        Componentes, JSX, props y composición en acción.
      </p>

      <section className="team-grid">
        {/* Renderizamos un UserCard por cada usuario (map: Módulo 2) */}
        {users.map((user) => (
          <UserCard key={user.id} user={user} />
        ))}
      </section>
    </main>
  );
}
