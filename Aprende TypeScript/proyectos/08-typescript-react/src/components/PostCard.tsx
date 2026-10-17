import type { Post } from "../types";

interface PostCardProps {
  post: Post;
  favorito: boolean;
  onToggleFavorito: (id: number) => void;
  onAbrir: (id: number) => void;
}

export function PostCard({
  post,
  favorito,
  onToggleFavorito,
  onAbrir,
}: PostCardProps) {
  return (
    <article className="post-card">
      <h2>{post.title}</h2>
      <p>{post.body.length > 120 ? post.body.slice(0, 120) + "..." : post.body}</p>
      <div className="acciones">
        <button onClick={() => onAbrir(post.id)}>Ver detalle</button>
        <button className="favorito" onClick={() => onToggleFavorito(post.id)}>
          {favorito ? "★ Quitar favorito" : "☆ Marcar favorito"}
        </button>
      </div>
    </article>
  );
}