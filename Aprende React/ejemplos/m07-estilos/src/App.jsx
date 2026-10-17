import ThemeDemo from "./components/ThemeDemo";
import ResponsiveDemo from "./components/ResponsiveDemo";

export default function App() {
  return (
    <div className="app">
      <h1>Módulo 7 · Estilos</h1>
      <section>
        <h2>1. Tema claro/oscuro con variables CSS</h2>
        <ThemeDemo />
      </section>
      <section>
        <h2>2. Layout responsive</h2>
        <ResponsiveDemo />
      </section>
    </div>
  );
}
