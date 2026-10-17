// Botón de confirmación de borrado: el componente NO borra nada,
// solo AVISA al padre mediante la prop callback onDelete.
export default function DeleteButton({ itemName, onDelete }) {
  function handleConfirm(e) {
    // El SyntheticEvent tiene la misma API que el evento nativo
    console.log("Clic en:", e.target.textContent);

    const confirmed = confirm(`¿Eliminar "${itemName}"?`);
    if (confirmed) {
      onDelete(); // el padre decide qué significa "borrar"
    }
  }

  return (
    <button className="btn btn--danger" onClick={handleConfirm}>
      🗑 Eliminar
    </button>
  );
}
