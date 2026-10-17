import { Link } from "react-router-dom";
import { posts } from "../data/posts";

export default function Home() {
  return (
    <div>
      <h1>Blog</h1>
      <div className="post-list">
        {posts.map((post) => (
          <article key={post.id} className="post-card">
            <h2>
              <Link to={`/post/${post.slug}`}>{post.title}</Link>
            </h2>
            <p className="post-meta">
              Por {post.author} · {post.date} ·{" "}
              <Link to={`/category/${post.category}`}>{post.category}</Link>
            </p>
            <p>{post.content.substring(0, 100)}...</p>
          </article>
        ))}
      </div>
    </div>
  );
}
