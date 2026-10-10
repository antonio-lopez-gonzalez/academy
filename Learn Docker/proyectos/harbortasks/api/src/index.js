// index.js — API REST de HarborTasks
// CRUD básico de tareas sobre PostgreSQL, con Express.
// Diseñado para funcionar tanto en Docker Compose como en Kubernetes
// (siempre lee la config desde variables de entorno).

import express from "express";
import pg from "pg";

const app = express();
const PORT = process.env.PORT || 3000;
const DATABASE_URL =
  process.env.DATABASE_URL || "postgres://harbor:harborpass@localhost:5432/harbortasks";

const pool = new pg.Pool({ connectionString: DATABASE_URL });

app.use(express.json());

// Crea la tabla si no existe (arranque simplificado; en prod usar migraciones)
async function ensureSchema() {
  await pool.query(`
    CREATE TABLE IF NOT EXISTS tasks (
      id SERIAL PRIMARY KEY,
      title TEXT NOT NULL,
      done BOOLEAN NOT NULL DEFAULT false,
      created_at TIMESTAMPTZ NOT NULL DEFAULT now()
    )
  `);
}

// GET /api/tasks — lista las tareas
app.get("/api/tasks", async (_req, res) => {
  const { rows } = await pool.query("SELECT * FROM tasks ORDER BY id");
  res.json(rows);
});

// POST /api/tasks — crea una tarea { title }
app.post("/api/tasks", async (req, res) => {
  const { title } = req.body ?? {};
  if (!title) return res.status(400).json({ error: "title es obligatorio" });
  const { rows } = await pool.query(
    "INSERT INTO tasks (title) VALUES ($1) RETURNING *",
    [title]
  );
  res.status(201).json(rows[0]);
});

// PATCH /api/tasks/:id — cambia done/title
app.patch("/api/tasks/:id", async (req, res) => {
  const { title, done } = req.body ?? {};
  const { rows } = await pool.query(
    "UPDATE tasks SET title = COALESCE($1, title), done = COALESCE($2, done) WHERE id = $3 RETURNING *",
    [title, done, req.params.id]
  );
  if (rows.length === 0) return res.status(404).json({ error: "tarea no encontrada" });
  res.json(rows[0]);
});

// DELETE /api/tasks/:id — elimina una tarea
app.delete("/api/tasks/:id", async (req, res) => {
  const { rowCount } = await pool.query("DELETE FROM tasks WHERE id = $1", [req.params.id]);
  if (rowCount === 0) return res.status(404).json({ error: "tarea no encontrada" });
  res.status(204).end();
});

// Healthcheck usado por Docker (HEALTHCHECK) y por las probes de K8s
app.get("/healthz", (_req, res) => {
  res.json({ status: "ok" });
});

ensureSchema()
  .then(() => app.listen(PORT, () => console.log(`api lista en :${PORT}`)))
  .catch((err) => {
    console.error("No se pudo conectar a la BD:", err.message);
    process.exit(1);
  });