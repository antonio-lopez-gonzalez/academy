import { memo } from 'react'

const Filters = memo(function Filters({ filters, onFilterChange }) {
  console.log('Filters render')
  
  const categories = ['Todas', 'Electrónica', 'Ropa', 'Hogar']
  
  return (
    <div className="filters">
      <div className="filter-group">
        <label>Categoría:</label>
        <select
          value={filters.category}
          onChange={(e) => onFilterChange({ ...filters, category: e.target.value })}
        >
          {categories.map(cat => (
            <option key={cat} value={cat}>{cat}</option>
          ))}
        </select>
      </div>

      <div className="filter-group">
        <label>Precio máximo: ${filters.maxPrice}</label>
        <input
          type="range"
          min="0"
          max="500"
          value={filters.maxPrice}
          onChange={(e) => onFilterChange({ 
            ...filters, 
            maxPrice: Number(e.target.value) 
          })}
        />
      </div>

      <div className="filter-group">
        <label>Buscar:</label>
        <input
          type="text"
          value={filters.search}
          onChange={(e) => onFilterChange({ ...filters, search: e.target.value })}
          placeholder="Buscar producto..."
        />
      </div>
    </div>
  )
})

export default Filters
