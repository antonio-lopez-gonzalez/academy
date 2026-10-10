import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import App from "./App.jsx";
import "./index.css";

// createRoot: punto de entrada de React 19 (API de cliente)
createRoot(document.getElementById("root")).render(
  <StrictMode>
    <App />
  </StrictMode>
);
