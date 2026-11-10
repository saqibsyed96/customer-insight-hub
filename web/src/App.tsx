import { useState } from 'react'
import { CustomerForm } from './features/customers/CustomerForm'
import { CustomerList } from './features/customers/CustomerList'

export default function App() {
  const [isAdding, setIsAdding] = useState(false)

  return (
    <main className="container">
      <header className="page-header">
        <div>
          <h1>Customer Insight Hub</h1>
          <p className="subtitle">Customers</p>
        </div>
        {!isAdding && (
          <button className="primary" onClick={() => setIsAdding(true)}>
            Add customer
          </button>
        )}
      </header>

      {isAdding && <CustomerForm onDone={() => setIsAdding(false)} />}

      <CustomerList />
    </main>
  )
}
