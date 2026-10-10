import { memo } from 'react'

// React.memo: solo se re-renderiza si cambian las props
const ExpensiveList = memo(function ExpensiveList({ items, onItemClick }) {
  console.log('ExpensiveList render')
  
  return (
    <ul className="expensive-list">
      {items.map(item => (
        <li 
          key={item.id} 
          onClick={() => onItemClick(item.id)}
          className="list-item"
        >
          <span>{item.name}</span>
          <span className="value">${item.value.toFixed(2)}</span>
        </li>
      ))}
    </ul>
  )
})

export default ExpensiveList
