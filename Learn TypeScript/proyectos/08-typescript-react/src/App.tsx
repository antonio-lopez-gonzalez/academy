import { useState } from "react";
import { useFetch } from "./hooks/useFetch";
import { useLocalStorage } from "./hooks/useLocalStorage";
import { PostCard } from "./components/PostCard";
import { PostDetail } from "./components/PostDetail";
import type { Post } from "./types";

export default function App() {
  const { datos: posts, cargando, error, recargar } = useFetch<Post[]>(
    "https://jsonplaceholder.typicode.com/posts"
  );
  const [seleccion, setSeleccion] = useState<number | null>(null);
  const [favoritos, setFavoritos] = useLocalStorage<number[]>("favoritos", []);

  function toggleFavorito(id: number) {
    setFavoritos(
      favoritos.includes(id)
        ? favoritos.filter((f) => f !== id)
        : [...favoritos, id]
    );
  }

  if (seleccion !== null) {
    return (
      <main>
        <PostDetail postId={seleccion} onVolver={() => setSeleccion(null)} />
      </main>
    );
  }

  return (
    <main className="contenedor">
      <header className="cabecera">
        <h1>Posts del blog</h1>
        <button className="recargar" onClick={recargar}>
          Recargar
        </button>
      </header>

      {cargando && <p className="estado">Cargando...</p>}
      {error && <p className="estado estado--error">Error: {error}</p>}

      <section className="lista-posts">
        {posts?.map((post) => (
          <PostCard
            key={post.id}
            post={post}
            favorito={favoritos.includes(post.id)}
            onToggleFavorito={toggleFavorito}
            onAbrir={setSeleccion}
          />
        ))}
      </section>
    </main>
  );
}