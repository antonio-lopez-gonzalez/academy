import { useState } from "react";

// Lección 3 · Estado con OBJETOS.
// Para cambiar UNA propiedad hay que crear un objeto NUEVO copiando
// todas las demás con spread y sobrescribiendo solo la que cambia.
export default function ObjectStateDemo() {
  const [profile, setProfile] = useState({
    name: "Ada",
    email: "ada@example.com",
    plan: "pro",
  });

  function handleNameChange(e) {
    // ✔ Objeto NUEVO: "{ ...profile }" copia todo,
    // "name:" sobrescribe solo esa clave
    setProfile({ ...profile, name: e.target.value });
  }

  function togglePlan() {
    setProfile({
      ...profile,
      plan: profile.plan === "pro" ? "free" : "pro",
    });
  }

  return (
    <div className="demo-box">
      <h3>Ficha de usuario (objeto en estado)</h3>
      <label>
        Nombre: <input value={profile.name} onChange={handleNameChange} />
      </label>{" "}
      <button onClick={togglePlan}>Cambiar plan</button>
      {/* La UI siempre proyecta el objeto COMPLETO actual */}
      <p>
        {profile.name} · {profile.email} · plan {profile.plan}
      </p>
    </div>
  );
}
