import ProductCard from "./ProductCard.jsx";

export default function ProductGrid({ products }) {
  // Return temprano: la lista vacía tiene su propio mensaje
  if (products.length === 0) {
    return <p className="empty-state">📭 No hay productos disponibles.</p>;
  }

  return (
    <div className="product-grid">
      {/* Un ProductCard por producto; key estable con el id */}
      {products.map((product) => (
        <ProductCard key={product.id} product={product} />
      ))}
    </div>
  );
}
