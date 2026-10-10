import { CartProvider, useCart } from "./contexts/CartContext";
import ProductList from "./components/ProductList";
import Cart from "./components/Cart";

export default function App() {
  return (
    <CartProvider>
      <div className="app">
        <h1>Tienda</h1>
        <div className="layout">
          <ProductList />
          <Cart />
        </div>
      </div>
    </CartProvider>
  );
}
