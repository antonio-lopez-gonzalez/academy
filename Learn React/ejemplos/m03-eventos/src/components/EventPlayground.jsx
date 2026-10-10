// Laboratorio de eventos: burbuja, stopPropagation y parámetros.
// Abre la consola (F12) y juega con los botones.
export default function EventPlayground() {
  // Un manejador por acción, compartido; el dato viaja en la arrow
  function handleMenu(action, event) {
    event.stopPropagation(); // el div padre NO se entera de este clic
    console.log(`Acción: ${action} · elemento:`, event.target.textContent);
  }

  return (
    <div
      className="playground"
      onClick={() => console.log("🔼 clic en el contenedor (burbuja)")}
    >
      <p>Haz clic en los botones y mira la consola:</p>
      {["Perfil", "Ajustes", "Salir"].map((action) => (
        <button
          key={action}
          className="btn"
          // Arrow con parámetro Y evento reenviado (lección 03)
          onClick={(e) => handleMenu(action, e)}
        >
          {action}
        </button>
      ))}
    </div>
  );
}
