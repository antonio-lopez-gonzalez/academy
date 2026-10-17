import { useFetch } from "../hooks/useFetch";
import type { Post, Comment } from "../types";

interface PostDetailProps {
  postId: number;
  onVolver: () => void;
}

export function PostDetail({ postId, onVolver }: PostDetailProps) {
  const { datos: post, cargando: postCargando } = useFetch<Post>(
    `https://jsonplaceholder.typicode.com/posts/${postId}`
  );
  const { datos: comentarios } = useFetch<Comment[]>(
    `https://jsonplaceholder.typicode.com/posts/${postId}/comments`
  );

  return (
    <article className="post-detail">
      <button className="volver" onClick={onVolver}>
        ← Volver
      </button>

      {post ? (
        <>
          <h1>{post.title}</h1>
          <p className="post-body">{post.body}</p>
        </>
      ) : (
        <p>{postCargando ? "Cargando post..." : "No se encontro el post"}</p>
      )}

      <h2>Comentarios ({comentarios?.length ?? 0})</h2>
      {comentarios?.map((c) => (
        <div key={c.id} className="comentario">
          <strong>{c.name}</strong> <span className="email">{c.email}</span>
          <p>{c.body}</p>
        </div>
      ))}
    </article>
  );
}