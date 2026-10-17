import { lazy, Suspense } from 'react'
import { Routes, Route, Link } from 'react-router-dom'

// Code splitting: carga diferida de páginas
const Dashboard = lazy(() => import('./pages/Dashboard'))
const Settings = lazy(() => import('./pages/Settings'))

function App() {
  return (
    <div className="app">
      <nav className="navbar">
        <Link to="/" className="nav-link">Dashboard</Link>
        <Link to="/settings" className="nav-link">Settings</Link>
      </nav>

      <Suspense fallback={<div className="loading">Cargando...</div>}>
        <Routes>
          <Route path="/" element={<Dashboard />} />
          <Route path="/settings" element={<Settings />} />
        </Routes>
      </Suspense>
    </div>
  )
}

export default App
