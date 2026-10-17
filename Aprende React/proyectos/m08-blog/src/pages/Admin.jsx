import { Navigate } from "react-router-dom";

export default function Admin() {
  const isAuth = false; // Simulación

  if (!isAuth) {
    return <Navigate to="/login" replace />;
  }

  return (
    <div>
      <h1>Panel de Administración</h1>
      <p>Bienvenido al panel de administración</p>
    </div>
  );
}
