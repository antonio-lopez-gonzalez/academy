import { useParams, Link } from "react-router-dom";
import { posts } from "../data/posts";

export default function PostDetail() {
  const { slug } = useParams();
  const post = posts.find((p) => p.slug === slug);

  if (!post) {
    return <h1>Post no encontrado</h1>;
  }

  return (
    <article>
      <h1>{post.title}</h1>
      <p className="post-meta">
        Por {post.author} · {post.date} ·{" "}
        <Link to={`/category/${post.category}`}>{post.category}</Link>
      </p>
      <div className="post-content">{post.content}</div>
      <Link to="/">← Volver al inicio</Link>
    </article>
  );
}
