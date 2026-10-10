import { useState } from 'react'
import TaskForm from './components/TaskForm'
import TaskList from './components/TaskList'
import TaskStats from './components/TaskStats'

function App() {
  const [tasks, setTasks] = useState([])

  function addTask(title) {
    const newTask = {
      id: Date.now(),
      title,
      completed: false,
      createdAt: new Date().toISOString()
    }
    setTasks(prev => [...prev, newTask])
  }

  function toggleTask(id) {
    setTasks(prev =>
      prev.map(task =>
        task.id === id ? { ...task, completed: !task.completed } : task
      )
    )
  }

  function deleteTask(id) {
    setTasks(prev => prev.filter(task => task.id !== id))
  }

  function clearCompleted() {
    setTasks(prev => prev.filter(task => !task.completed))
  }

  return (
    <div className="app">
      <header>
        <h1>Task Manager</h1>
        <TaskStats tasks={tasks} />
      </header>
      <TaskForm onSubmit={addTask} />
      <TaskList 
        tasks={tasks} 
        onToggle={toggleTask} 
        onDelete={deleteTask}
        onClearCompleted={clearCompleted}
      />
    </div>
  )
}

export default App
