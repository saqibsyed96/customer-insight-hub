// Shape of a customer as returned by GET /api/customers (matches CustomerDto in the backend).
export interface Customer {
  id: string
  name: string
  email: string
  phone: string | null
  city: string | null
  country: string | null
  website: string | null
  createdAtUtc: string
}

export async function fetchCustomers(): Promise<Customer[]> {
  const response = await fetch('/api/customers')

  if (!response.ok) {
    throw new Error(`Could not load customers (HTTP ${response.status})`)
  }

  return (await response.json()) as Customer[]
}
