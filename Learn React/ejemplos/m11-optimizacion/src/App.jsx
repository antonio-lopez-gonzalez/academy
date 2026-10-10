import { useState, useMemo, useCallback, memo } from 'react'
import ExpensiveList from './components/ExpensiveList'

function App() {
  const [count, setCount] = useState(0)
  const [text, setText] = useState('')
  const [items] = useState(() => 
    Array.from({ length: 100 }, (_, i) => ({
      id: i,
      name: `Item ${i}`,
      value: Math.random() * 100
    }))
  )

  // useMemo: solo se recalcula cuando cambia text
  const filteredItems = useMemo(() => {
    console.log('Filtrando items...')
    return items.filter(item => 
      item.name.toLowerCase().includes(text.toLowerCase())
    )
  }, [items, text])

  // useCallback: función estable
  const handleItemClick = useCallback((id) => {
    console.log('Item clicked:', id)
  }, [])

  return (
    <div className="app">
      <h1>Optimización con useMemo y useCallback</h1>
      
      <div className="controls">
        <button onClick={() => setCount(c => c + 1)}>
          Count: {count}
        </button>
        
        <input
          type="text"
          value={text}
          onChange={(e) => setText(e.target.value)}
          placeholder="Buscar..."
        />
      </div>

      <ExpensiveList 
        items={filteredItems} 
        onItemClick={handleItemClick}
      />
    </div>
  )
}

export default App
