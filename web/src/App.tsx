import { CustomerList } from './features/customers/CustomerList'

export default function App() {
  return (
    <main className="container">
      <header>
        <h1>Customer Insight Hub</h1>
        <p className="subtitle">Customers</p>
      </header>
      <CustomerList />
    </main>
  )
}
