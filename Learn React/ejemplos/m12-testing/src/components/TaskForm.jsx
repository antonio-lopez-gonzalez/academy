import { useState } from 'react'

export default function TaskForm({ onSubmit }) {
  const [title, setTitle] = useState('')

  function handleSubmit(e) {
    e.preventDefault()
    if (!title.trim()) return
    
    onSubmit(title)
    setTitle('')
  }

  return (
    <form onSubmit={handleSubmit} className="task-form">
      <input
        type="text"
        value={title}
        onChange={(e) => setTitle(e.target.value)}
        placeholder="Nueva tarea"
        aria-label="Título de la tarea"
      />
      <button type="submit">Añadir</button>
    </form>
  )
}
