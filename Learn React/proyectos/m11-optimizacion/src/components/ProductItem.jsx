import { memo } from 'react'

// React.memo: evita re-renders innecesarios
const ProductItem = memo(function ProductItem({ product, onClick }) {
  console.log(`ProductItem ${product.id} render`)
  
  return (
    <div 
      className="product-item"
      onClick={() => onClick(product.id)}
    >
      <h3>{product.name}</h3>
      <p className="category">{product.category}</p>
      <p className="price">${product.price.toFixed(2)}</p>
      <p className="stock">Stock: {product.stock}</p>
    </div>
  )
})

export default ProductItem
