import { useState, useMemo, useCallback, lazy, Suspense } from 'react'
import ProductList from '../components/ProductList'
import Filters from '../components/Filters'

// Datos simulados
const generateProducts = () => 
  Array.from({ length: 1000 }, (_, i) => ({
    id: i,
    name: `Producto ${i}`,
    category: i % 5 === 0 ? 'Electrónica' : i % 3 === 0 ? 'Ropa' : 'Hogar',
    price: Math.random() * 500,
    stock: Math.floor(Math.random() * 100)
  }))

function Dashboard() {
  const [products] = useState(generateProducts)
  const [filters, setFilters] = useState({
    category: 'Todas',
    maxPrice: 500,
    search: ''
  })
  const [showChart, setShowChart] = useState(false)

  // Carga diferida del gráfico
  const SalesChart = lazy(() => import('../components/SalesChart'))

  // useMemo: filtrado optimizado
  const filteredProducts = useMemo(() => {
    console.log('Filtrando productos...')
    return products.filter(product => {
      const matchCategory = filters.category === 'Todas' || 
                           product.category === filters.category
      const matchPrice = product.price <= filters.maxPrice
      const matchSearch = product.name.toLowerCase()
        .includes(filters.search.toLowerCase())
      
      return matchCategory && matchPrice && matchSearch
    })
  }, [products, filters])

  // useCallback: handlers estables
  const handleFilterChange = useCallback((newFilters) => {
    setFilters(newFilters)
  }, [])

  const handleProductClick = useCallback((id) => {
    console.log('Producto clickeado:', id)
  }, [])

  return (
    <div className="dashboard">
      <h1>Dashboard de Productos</h1>
      
      <Filters 
        filters={filters} 
        onFilterChange={handleFilterChange}
      />

      <ProductList 
        products={filteredProducts}
        onProductClick={handleProductClick}
      />

      <div className="chart-section">
        <button 
          onClick={() => setShowChart(!showChart)}
          className="toggle-chart"
        >
          {showChart ? 'Ocultar' : 'Mostrar'} gráfico de ventas
        </button>

        {showChart && (
          <Suspense fallback={<div>Cargando gráfico...</div>}>
            <SalesChart />
          </Suspense>
        )}
      </div>
    </div>
  )
}

export default Dashboard
