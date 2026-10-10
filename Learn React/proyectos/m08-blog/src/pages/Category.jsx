import { useParams, Link } from "react-router-dom";
import { posts } from "../data/posts";

export default function Category() {
  const { name } = useParams();
  const categoryPosts = posts.filter((p) => p.category === name);

  return (
    <div>
      <h1>Categoría: {name}</h1>
      {categoryPosts.length === 0 ? (
        <p>No hay posts en esta categoría</p>
      ) : (
        <div className="post-list">
          {categoryPosts.map((post) => (
            <article key={post.id} className="post-card">
              <h2>
                <Link to={`/post/${post.slug}`}>{post.title}</Link>
              </h2>
              <p className="post-meta">Por {post.author} · {post.date}</p>
            </article>
          ))}
        </div>
      )}
      <Link to="/">← Volver al inicio</Link>
    </div>
  );
}
