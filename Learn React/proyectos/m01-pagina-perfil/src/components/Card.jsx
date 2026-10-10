// Contenedor visual agnóstico: no sabe qué contiene.
// La prop "title" es opcional (cortocircuito && para no pintar h3 vacío).
export default function Card({ title, children }) {
  return (
    <section className="card">
      {title && <h3 className="card__title">{title}</h3>}
      {children}
    </section>
  );
}
