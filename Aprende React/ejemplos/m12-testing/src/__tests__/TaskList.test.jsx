import { describe, it, expect } from 'vitest'
import { render, screen } from '@testing-library/react'
import TaskList from '../components/TaskList'

describe('TaskList', () => {
  it('should render empty state', () => {
    render(<TaskList tasks={[]} onToggle={vi.fn()} onDelete={vi.fn()} />)

    expect(screen.getByText('No hay tareas')).toBeInTheDocument()
  })

  it('should render tasks', () => {
    const mockTasks = [
      { id: 1, title: 'Tarea 1', completed: false },
      { id: 2, title: 'Tarea 2', completed: true }
    ]

    render(
      <TaskList 
        tasks={mockTasks} 
        onToggle={vi.fn()} 
        onDelete={vi.fn()} 
      />
    )

    expect(screen.getByText('Tarea 1')).toBeInTheDocument()
    expect(screen.getByText('Tarea 2')).toBeInTheDocument()
  })

  it('should render correct number of items', () => {
    const mockTasks = [
      { id: 1, title: 'Tarea 1', completed: false },
      { id: 2, title: 'Tarea 2', completed: true }
    ]

    render(
      <TaskList 
        tasks={mockTasks} 
        onToggle={vi.fn()} 
        onDelete={vi.fn()} 
      />
    )

    const items = screen.getAllByRole('listitem')
    expect(items).toHaveLength(2)
  })
})
