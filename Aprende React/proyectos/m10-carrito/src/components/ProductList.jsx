import { useCart } from "../contexts/CartContext";

const products = [
  {
    id: 1,
    name: "Camiseta React",
    price: 29.99,
    image: "https://via.placeholder.com/300x300?text=Camiseta",
    description: "Camiseta con logo de React",
  },
  {
    id: 2,
    name: "Taza JavaScript",
    price: 14.99,
    image: "https://via.placeholder.com/300x300?text=Taza",
    description: "Taza con diseño de JavaScript",
  },
  {
    id: 3,
    name: "Gorra Developer",
    price: 19.99,
    image: "https://via.placeholder.com/300x300?text=Gorra",
    description: "Gorra para developers",
  },
  {
    id: 4,
    name: "Sticker Pack",
    price: 9.99,
    image: "https://via.placeholder.com/300x300?text=Stickers",
    description: "Pack de stickers de programación",
  },
  {
    id: 5,
    name: "Hoodie Code",
    price: 49.99,
    image: "https://via.placeholder.com/300x300?text=Hoodie",
    description: "Hoodie con código de muestra",
  },
  {
    id: 6,
    name: "Mouse Pad",
    price: 12.99,
    image: "https://via.placeholder.com/300x300?text=MousePad",
    description: "Mouse pad con atajos de teclado",
  },
];

export default function ProductList() {
  const { addItem } = useCart();

  return (
    <div className="product-list">
      <h2>Productos</h2>
      <div className="products-grid">
        {products.map((product) => (
          <div key={product.id} className="product-card">
            <img src={product.image} alt={product.name} />
            <div className="product-info">
              <h3>{product.name}</h3>
              <p className="product-description">{product.description}</p>
              <p className="product-price">${product.price}</p>
              <button onClick={() => addItem(product)}>Añadir al carrito</button>
            </div>
          </div>
        ))}
      </div>
    </div>
  );
}
