import { useCart } from "../contexts/CartContext";

export default function Header() {
  const { itemCount } = useCart();

  return (
    <header className="header">
      <h1>Tienda Online</h1>
      <div className="cart-icon">
        🛒 <span className="cart-count">{itemCount}</span>
      </div>
    </header>
  );
}
