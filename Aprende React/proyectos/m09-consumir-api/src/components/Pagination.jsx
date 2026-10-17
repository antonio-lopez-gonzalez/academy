export default function Pagination({ info, currentPage, onPageChange }) {
  if (!info || info.pages <= 1) return null;

  return (
    <div className="pagination">
      <button
        onClick={() => onPageChange(currentPage - 1)}
        disabled={currentPage === 1}
      >
        Anterior
      </button>
      <span>
        Página {currentPage} de {info.pages}
      </span>
      <button
        onClick={() => onPageChange(currentPage + 1)}
        disabled={currentPage === info.pages}
      >
        Siguiente
      </button>
    </div>
  );
}
