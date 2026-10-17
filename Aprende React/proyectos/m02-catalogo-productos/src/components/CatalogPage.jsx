import { products } from "../data/products.js";
import ProductGrid from "./ProductGrid.jsx";

export default function CatalogPage() {
  return (
    <main className="catalog-page">
      <h1>Catálogo</h1>
      <p className="catalog-page__count">
        {/* Ternario para el singular/plural: detalle profesional */}
        {products.length} {products.length === 1 ? "producto" : "productos"}
      </p>
      <ProductGrid products={products} />
    </main>
  );
}
