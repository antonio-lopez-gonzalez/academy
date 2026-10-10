import { describe, it, expect } from 'vitest'
import { render, screen } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import TaskForm from '../components/TaskForm'

describe('TaskForm', () => {
  it('should render form', () => {
    render(<TaskForm onSubmit={vi.fn()} />)

    expect(screen.getByLabelText('Título de la tarea')).toBeInTheDocument()
    expect(screen.getByRole('button', { name: /añadir/i })).toBeInTheDocument()
  })

  it('should call onSubmit with title', async () => {
    const user = userEvent.setup()
    const onSubmit = vi.fn()

    render(<TaskForm onSubmit={onSubmit} />)

    const input = screen.getByLabelText('Título de la tarea')
    await user.type(input, 'Nueva tarea')
    await user.click(screen.getByRole('button', { name: /añadir/i }))

    expect(onSubmit).toHaveBeenCalledWith('Nueva tarea')
  })

  it('should clear input after submit', async () => {
    const user = userEvent.setup()

    render(<TaskForm onSubmit={vi.fn()} />)

    const input = screen.getByLabelText('Título de la tarea')
    await user.type(input, 'Nueva tarea')
    await user.click(screen.getByRole('button', { name: /añadir/i }))

    expect(input).toHaveValue('')
  })

  it('should not submit empty title', async () => {
    const user = userEvent.setup()
    const onSubmit = vi.fn()

    render(<TaskForm onSubmit={onSubmit} />)

    await user.click(screen.getByRole('button', { name: /añadir/i }))

    expect(onSubmit).not.toHaveBeenCalled()
  })
})
