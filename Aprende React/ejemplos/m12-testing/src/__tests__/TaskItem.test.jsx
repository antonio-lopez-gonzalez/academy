import { describe, it, expect } from 'vitest'
import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import TaskItem from '../components/TaskItem'

describe('TaskItem', () => {
  const mockTask = { id: 1, title: 'Tarea de prueba', completed: false }

  it('should render task', () => {
    render(
      <TaskItem 
        task={mockTask} 
        onToggle={vi.fn()} 
        onDelete={vi.fn()} 
      />
    )

    expect(screen.getByText('Tarea de prueba')).toBeInTheDocument()
    expect(screen.getByRole('checkbox')).not.toBeChecked()
  })

  it('should render completed task', () => {
    const completedTask = { ...mockTask, completed: true }
    
    render(
      <TaskItem 
        task={completedTask} 
        onToggle={vi.fn()} 
        onDelete={vi.fn()} 
      />
    )

    expect(screen.getByRole('checkbox')).toBeChecked()
  })

  it('should call onToggle when checkbox clicked', async () => {
    const user = userEvent.setup()
    const onToggle = vi.fn()

    render(
      <TaskItem 
        task={mockTask} 
        onToggle={onToggle} 
        onDelete={vi.fn()} 
      />
    )

    await user.click(screen.getByRole('checkbox'))

    expect(onToggle).toHaveBeenCalledWith(1)
  })

  it('should call onDelete when delete button clicked', async () => {
    const user = userEvent.setup()
    const onDelete = vi.fn()

    render(
      <TaskItem 
        task={mockTask} 
        onToggle={vi.fn()} 
        onDelete={onDelete} 
      />
    )

    await user.click(screen.getByLabelText('Eliminar tarea'))

    expect(onDelete).toHaveBeenCalledWith(1)
  })
})
