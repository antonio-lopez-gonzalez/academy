import PriceTag from "./PriceTag.jsx";
import StockNotice from "./StockNotice.jsx";

// Diccionario valor-dato → etiqueta visible (mejor que ternarios en cadena)
const CATEGORY_LABELS = {
  peripherals: "Periféricos",
  monitors: "Monitores",
  cameras: "Cámaras",
  audio: "Audio",
  accessories: "Accesorios",
};

export default function ProductCard({ product }) {
  // La tarjeta entera cambia de aspecto si está agotada (clase BEM condicional)
  const cardClass =
    product.stock === 0 ? "product-card product-card--soldout" : "product-card";

  return (
    <article className={cardClass}>
      <span className="product-card__category">
        {/* ?? por si el backend manda una categoría no mapeada */}
        {CATEGORY_LABELS[product.category] ?? product.category}
      </span>

      <h3 className="product-card__name">{product.name}</h3>
      <PriceTag price={product.price} onSale={product.onSale} />
      <StockNotice stock={product.stock} />
    </article>
  );
}
