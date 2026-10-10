import { describe, it, expect } from 'vitest'
import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import TaskList from '../components/TaskList'

describe('TaskList', () => {
  const mockTasks = [
    { id: 1, title: 'Tarea 1', completed: false, createdAt: '2024-01-01' },
    { id: 2, title: 'Tarea 2', completed: true, createdAt: '2024-01-02' }
  ]

  it('should render empty state', () => {
    render(
      <TaskList 
        tasks={[]} 
        onToggle={vi.fn()} 
        onDelete={vi.fn()}
        onClearCompleted={vi.fn()}
      />
    )

    expect(screen.getByText('No hay tareas')).toBeInTheDocument()
  })

  it('should render tasks', () => {
    render(
      <TaskList 
        tasks={mockTasks} 
        onToggle={vi.fn()} 
        onDelete={vi.fn()}
        onClearCompleted={vi.fn()}
      />
    )

    expect(screen.getByText('Tarea 1')).toBeInTheDocument()
    expect(screen.getByText('Tarea 2')).toBeInTheDocument()
  })

  it('should render correct number of items', () => {
    render(
      <TaskList 
        tasks={mockTasks} 
        onToggle={vi.fn()} 
        onDelete={vi.fn()}
        onClearCompleted={vi.fn()}
      />
    )

    const items = screen.getAllByRole('listitem')
    expect(items).toHaveLength(2)
  })

  it('should show clear completed button when there are completed tasks', () => {
    render(
      <TaskList 
        tasks={mockTasks} 
        onToggle={vi.fn()} 
        onDelete={vi.fn()}
        onClearCompleted={vi.fn()}
      />
    )

    expect(screen.getByText(/limpiar completadas/i)).toBeInTheDocument()
  })

  it('should not show clear completed button when no completed tasks', () => {
    const pendingTasks = [
      { id: 1, title: 'Tarea 1', completed: false, createdAt: '2024-01-01' }
    ]

    render(
      <TaskList 
        tasks={pendingTasks} 
        onToggle={vi.fn()} 
        onDelete={vi.fn()}
        onClearCompleted={vi.fn()}
      />
    )

    expect(screen.queryByText(/limpiar completadas/i)).not.toBeInTheDocument()
  })

  it('should call onClearCompleted when button clicked', async () => {
    const user = userEvent.setup()
    const onClearCompleted = vi.fn()

    render(
      <TaskList 
        tasks={mockTasks} 
        onToggle={vi.fn()} 
        onDelete={vi.fn()}
        onClearCompleted={onClearCompleted}
      />
    )

    await user.click(screen.getByText(/limpiar completadas/i))

    expect(onClearCompleted).toHaveBeenCalledOnce()
  })
})
