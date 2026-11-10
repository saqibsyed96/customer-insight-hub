import { useQuery } from '@tanstack/react-query'
import { useState } from 'react'
import { fetchCustomers } from '../../api/customers'

export function CustomerList() {
  const [search, setSearch] = useState('')

  // useQuery fetches the data, caches it, and gives us loading / error states for free.
  const { data, isPending, isError, error, refetch } = useQuery({
    queryKey: ['customers'],
    queryFn: fetchCustomers,
  })

  if (isPending) {
    return <p className="status">Loading customers...</p>
  }

  if (isError) {
    return (
      <div className="status status-error" role="alert">
        <p>{error.message}</p>
        <button onClick={() => refetch()}>Try again</button>
      </div>
    )
  }

  // Client-side search over name, email and city.
  const term = search.trim().toLowerCase()
  const customers = term
    ? data.filter((c) =>
        [c.name, c.email, c.city ?? ''].some((field) => field.toLowerCase().includes(term)),
      )
    : data

  return (
    <section>
      <div className="toolbar">
        <input
          type="search"
          placeholder="Search by name, email or city"
          value={search}
          onChange={(e) => setSearch(e.target.value)}
          aria-label="Search customers"
        />
        <span className="count">
          {customers.length} of {data.length} customers
        </span>
      </div>

      {customers.length === 0 ? (
        <p className="status">No customers match your search.</p>
      ) : (
        <table>
          <thead>
            <tr>
              <th>Name</th>
              <th>Email</th>
              <th>City</th>
              <th>Country</th>
              <th>Website</th>
            </tr>
          </thead>
          <tbody>
            {customers.map((c) => (
              <tr key={c.id}>
                <td>{c.name}</td>
                <td>{c.email}</td>
                <td>{c.city ?? '-'}</td>
                <td>{c.country ?? '-'}</td>
                <td>
                  {c.website ? (
                    <a href={c.website} target="_blank" rel="noreferrer">
                      {c.website.replace(/^https?:\/\//, '')}
                    </a>
                  ) : (
                    '-'
                  )}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </section>
  )
}
