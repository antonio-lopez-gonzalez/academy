import { useCart } from "../contexts/CartContext";

export default function Cart() {
  const { items, total, removeItem, updateQuantity, clearCart } = useCart();

  if (items.length === 0) {
    return (
      <div className="cart">
        <h2>Carrito</h2>
        <p className="empty-cart">El carrito está vacío</p>
      </div>
    );
  }

  return (
    <div className="cart">
      <h2>Carrito</h2>
      <div className="cart-items">
        {items.map((item) => (
          <div key={item.id} className="cart-item">
            <div className="cart-item-info">
              <h4>{item.name}</h4>
              <p className="cart-item-price">${item.price}</p>
            </div>
            <div className="cart-item-controls">
              <button
                onClick={() => updateQuantity(item.id, item.quantity - 1)}
              >
                -
              </button>
              <span className="quantity">{item.quantity}</span>
              <button onClick={() => updateQuantity(item.id, item.quantity + 1)}>
                +
              </button>
              <button
                onClick={() => removeItem(item.id)}
                className="remove-btn"
              >
                Eliminar
              </button>
            </div>
          </div>
        ))}
      </div>
      <div className="cart-summary">
        <div className="cart-total">
          <strong>Total: ${total.toFixed(2)}</strong>
        </div>
        <button className="checkout-btn">Proceder al pago</button>
        <button onClick={clearCart} className="clear-btn">
          Vaciar carrito
        </button>
      </div>
    </div>
  );
}
