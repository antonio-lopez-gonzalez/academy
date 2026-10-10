import { CartProvider } from "./contexts/CartContext";
import Header from "./components/Header";
import ProductList from "./components/ProductList";
import Cart from "./components/Cart";

export default function App() {
  return (
    <CartProvider>
      <div className="app">
        <Header />
        <main className="main-content">
          <ProductList />
          <Cart />
        </main>
      </div>
    </CartProvider>
  );
}
