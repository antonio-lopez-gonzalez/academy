import { useCart } from "../contexts/CartContext";

export default function Cart() {
  const { items, total, removeItem, updateQuantity, clearCart } = useCart();

  if (items.length === 0) {
    return (
      <div className="cart">
        <h2>Carrito</h2>
        <p>El carrito está vacío</p>
      </div>
    );
  }

  return (
    <div className="cart">
      <h2>Carrito</h2>
      {items.map((item) => (
        <div key={item.id} className="cart-item">
          <div>
            <h4>{item.name}</h4>
            <p>${item.price}</p>
          </div>
          <div className="quantity-controls">
            <button
              onClick={() =>
                updateQuantity(item.id, Math.max(0, item.quantity - 1))
              }
            >
              -
            </button>
            <span>{item.quantity}</span>
            <button onClick={() => updateQuantity(item.id, item.quantity + 1)}>
              +
            </button>
            <button onClick={() => removeItem(item.id)}>Eliminar</button>
          </div>
        </div>
      ))}
      <div className="cart-total">
        <strong>Total: ${total.toFixed(2)}</strong>
      </div>
      <button onClick={clearCart} className="clear-btn">
        Vaciar carrito
      </button>
    </div>
  );
}
