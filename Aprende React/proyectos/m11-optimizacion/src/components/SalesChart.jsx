import { useMemo } from 'react'

// Componente pesado: simulación de gráfico complejo
function SalesChart() {
  console.log('SalesChart render (componente pesado)')
  
  // Simula cálculo costoso
  const chartData = useMemo(() => {
    console.log('Calculando datos del gráfico...')
    return Array.from({ length: 12 }, (_, i) => ({
      month: ['Ene', 'Feb', 'Mar', 'Abr', 'May', 'Jun', 
              'Jul', 'Ago', 'Sep', 'Oct', 'Nov', 'Dic'][i],
      sales: Math.random() * 10000
    }))
  }, [])
  
  return (
    <div className="sales-chart">
      <h3>Ventas mensuales</h3>
      <div className="chart-container">
        {chartData.map((data, index) => (
          <div key={index} className="chart-bar">
            <div 
              className="bar"
              style={{ height: `${(data.sales / 10000) * 100}%` }}
            />
            <span className="label">{data.month}</span>
          </div>
        ))}
      </div>
    </div>
  )
}

export default SalesChart
