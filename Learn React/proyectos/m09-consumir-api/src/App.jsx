import { Routes, Route } from "react-router-dom";
import Home from "./pages/Home";
import CharacterPage from "./pages/CharacterPage";

export default function App() {
  return (
    <Routes>
      <Route path="/" element={<Home />} />
      <Route path="/character/:id" element={<CharacterPage />} />
    </Routes>
  );
}
