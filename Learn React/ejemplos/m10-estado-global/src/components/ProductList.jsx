import { useCart } from "../contexts/CartContext";

const products = [
  { id: 1, name: "Camiseta", price: 19.99 },
  { id: 2, name: "Pantalón", price: 39.99 },
  { id: 3, name: "Zapatos", price: 59.99 },
  { id: 4, name: "Gorra", price: 14.99 },
];

export default function ProductList() {
  const { addItem } = useCart();

  return (
    <div className="product-list">
      <h2>Productos</h2>
      {products.map((product) => (
        <div key={product.id} className="product-card">
          <h3>{product.name}</h3>
          <p className="price">${product.price}</p>
          <button onClick={() => addItem(product)}>Añadir al carrito</button>
        </div>
      ))}
    </div>
  );
}
