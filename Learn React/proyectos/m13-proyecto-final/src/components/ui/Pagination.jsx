import styles from "./Pagination.module.css";

export default function Pagination({ page, totalPages, onNext, onPrev }) {
  return (
    <div className={styles.pagination}>
      <button onClick={onPrev} disabled={page <= 1}>
        Anterior
      </button>
      <span>
        Página {page} de {totalPages}
      </span>
      <button onClick={onNext} disabled={page >= totalPages}>
        Siguiente
      </button>
    </div>
  );
}
