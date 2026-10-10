import { useState } from "react";
import styles from "./DetailLists.module.css";

export default function CommentList({ comments, onAdd }) {
  const [content, setContent] = useState("");

  async function handleSubmit(e) {
    e.preventDefault();
    if (!content.trim()) return;
    await onAdd(content);
    setContent("");
  }

  return (
    <section className={styles.section}>
      <h3>Comentarios</h3>

      <form onSubmit={handleSubmit} className={styles.form}>
        <textarea
          value={content}
          onChange={(e) => setContent(e.target.value)}
          placeholder="Escribe un comentario"
          rows={3}
        />
        <button type="submit">Comentar</button>
      </form>

      <ul className={styles.list}>
        {comments.map((comment) => (
          <li key={comment.id}>
            <div className={styles.meta}>
              <strong>{comment.authorName}</strong>
              <time>{new Date(comment.createdAt).toLocaleString()}</time>
            </div>
            <p>{comment.content}</p>
          </li>
        ))}
      </ul>
    </section>
  );
}
