import { memo } from 'react'
import ProductItem from './ProductItem'

// React.memo: solo se re-renderiza si cambian las props
const ProductList = memo(function ProductList({ products, onProductClick }) {
  console.log('ProductList render')
  
  return (
    <div className="product-list">
      <h2>Productos ({products.length})</h2>
      <div className="products-grid">
        {products.map(product => (
          <ProductItem 
            key={product.id} 
            product={product}
            onClick={onProductClick}
          />
        ))}
      </div>
    </div>
  )
})

export default ProductList
