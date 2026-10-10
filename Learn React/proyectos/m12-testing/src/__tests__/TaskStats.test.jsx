import { describe, it, expect } from 'vitest'
import { render, screen } from '@testing-library/react'
import TaskStats from '../components/TaskStats'

describe('TaskStats', () => {
  it('should render zero stats when no tasks', () => {
    render(<TaskStats tasks={[]} />)

    expect(screen.getByText('0')).toBeInTheDocument()
  })

  it('should display correct stats', () => {
    const tasks = [
      { id: 1, title: 'Tarea 1', completed: false },
      { id: 2, title: 'Tarea 2', completed: true },
      { id: 3, title: 'Tarea 3', completed: true }
    ]

    render(<TaskStats tasks={tasks} />)

    expect(screen.getByText('3')).toBeInTheDocument() // Total
    expect(screen.getByText('2')).toBeInTheDocument() // Completadas
    expect(screen.getByText('1')).toBeInTheDocument() // Pendientes
  })

  it('should display labels', () => {
    render(<TaskStats tasks={[]} />)

    expect(screen.getByText('Total')).toBeInTheDocument()
    expect(screen.getByText('Completadas')).toBeInTheDocument()
    expect(screen.getByText('Pendientes')).toBeInTheDocument()
  })
})
