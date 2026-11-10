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

// What the form sends to POST /api/customers (matches SaveCustomerRequest in the backend).
export interface SaveCustomerRequest {
  name: string
  email: string
  phone: string
  city: string
  country: string
  website: string
}

/**
 * An error response from our API. Carries the HTTP status and, for validation failures (400),
 * the per-field messages so a form can show them next to the right input.
 */
export class ApiError extends Error {
  status: number
  fieldErrors: Record<string, string[]>

  constructor(message: string, status: number, fieldErrors: Record<string, string[]> = {}) {
    super(message)
    this.name = 'ApiError'
    this.status = status
    this.fieldErrors = fieldErrors
  }
}

interface ProblemDetails {
  title?: string
  detail?: string
  errors?: Record<string, string[]>
}

export async function fetchCustomers(): Promise<Customer[]> {
  const response = await fetch('/api/customers')

  if (!response.ok) {
    throw new Error(`Could not load customers (HTTP ${response.status})`)
  }

  return (await response.json()) as Customer[]
}

export async function createCustomer(request: SaveCustomerRequest): Promise<Customer> {
  const response = await fetch('/api/customers', {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(request),
  })

  if (response.ok) {
    return (await response.json()) as Customer
  }

  // 400 (validation) and 409 (duplicate email) come back as ProblemDetails JSON.
  const problem = (await response.json().catch(() => null)) as ProblemDetails | null
  throw new ApiError(
    problem?.detail ?? problem?.title ?? `Request failed (HTTP ${response.status})`,
    response.status,
    problem?.errors ?? {},
  )
}
