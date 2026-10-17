import { useState } from 'react'

export default function TaskForm({ onSubmit }) {
  const [title, setTitle] = useState('')
  const [error, setError] = useState('')

  function handleSubmit(e) {
    e.preventDefault()
    
    if (!title.trim()) {
      setError('El título es obligatorio')
      return
    }

    if (title.length < 3) {
      setError('El título debe tener al menos 3 caracteres')
      return
    }

    onSubmit(title)
    setTitle('')
    setError('')
  }

  return (
    <form onSubmit={handleSubmit} className="task-form">
      <div className="form-group">
        <input
          type="text"
          value={title}
          onChange={(e) => {
            setTitle(e.target.value)
            if (error) setError('')
          }}
          placeholder="Nueva tarea"
          aria-label="Título de la tarea"
          aria-invalid={!!error}
          aria-describedby={error ? 'title-error' : undefined}
        />
        {error && (
          <span id="title-error" className="error" role="alert">
            {error}
          </span>
        )}
      </div>
      <button type="submit">Añadir</button>
    </form>
  )
}
