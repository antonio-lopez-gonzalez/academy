export default function PriceTag({ price, onSale }) {
  // Formato de moneda profesional: nunca price + "€"
  const formatted = price.toLocaleString("es-ES", {
    style: "currency",
    currency: "EUR",
  });

  return (
    <p className="price-tag">
      {onSale ? (
        <>
          <strong className="price-tag--sale">{formatted}</strong>
          <span className="price-tag__badge">OFERTA</span>
        </>
      ) : (
        <strong>{formatted}</strong>
      )}
    </p>
  );
}
