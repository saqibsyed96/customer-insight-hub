import { useMutation, useQueryClient } from '@tanstack/react-query'
import { useState, type ChangeEvent, type FormEvent } from 'react'
import { ApiError, createCustomer, type SaveCustomerRequest } from '../../api/customers'

interface Props {
  onDone: () => void
}

const emptyForm: SaveCustomerRequest = {
  name: '',
  email: '',
  phone: '',
  city: '',
  country: '',
  website: '',
}

const fields: { name: keyof SaveCustomerRequest; label: string; required?: boolean; placeholder?: string }[] = [
  { name: 'name', label: 'Name', required: true },
  { name: 'email', label: 'Email', required: true },
  { name: 'phone', label: 'Phone' },
  { name: 'city', label: 'City' },
  { name: 'country', label: 'Country' },
  { name: 'website', label: 'Website', placeholder: 'https://' },
]

export function CustomerForm({ onDone }: Props) {
  // "Controlled inputs": React state is the single source of truth for what is typed.
  const [values, setValues] = useState<SaveCustomerRequest>(emptyForm)
  const queryClient = useQueryClient()

  const mutation = useMutation({
    mutationFn: createCustomer,
    onSuccess: async () => {
      // Tell React Query the cached list is stale so the table reloads with the new customer.
      await queryClient.invalidateQueries({ queryKey: ['customers'] })
      onDone()
    },
  })

  const apiError = mutation.error instanceof ApiError ? mutation.error : null

  // The backend sends keys like "Name"; our form fields are "name", so compare ignoring case.
  function fieldMessages(field: string): string[] {
    if (!apiError) return []
    const key = Object.keys(apiError.fieldErrors).find((k) => k.toLowerCase() === field.toLowerCase())
    return key ? apiError.fieldErrors[key] : []
  }

  function handleChange(event: ChangeEvent<HTMLInputElement>) {
    const { name, value } = event.target
    setValues((current) => ({ ...current, [name]: value }))
  }

  function handleSubmit(event: FormEvent) {
    event.preventDefault() // stop the browser's default full-page form submit
    mutation.mutate(values)
  }

  // A 400 is shown per field. Anything else (409 duplicate, 500, network) is shown once on top.
  const showTopError = mutation.isError && apiError?.status !== 400

  return (
    // noValidate: the server is the one source of truth for validation rules, so we show its messages.
    <form className="customer-form" onSubmit={handleSubmit} noValidate>
      <h2>Add customer</h2>

      {showTopError && (
        <p className="form-error" role="alert">
          {mutation.error.message}
        </p>
      )}

      {fields.map((field) => {
        const messages = fieldMessages(field.name)
        const errorId = `${field.name}-error`
        return (
          <div className="form-field" key={field.name}>
            <label htmlFor={field.name}>
              {field.label}
              {field.required && <span aria-hidden="true"> *</span>}
            </label>
            <input
              id={field.name}
              name={field.name}
              value={values[field.name]}
              onChange={handleChange}
              placeholder={field.placeholder}
              aria-invalid={messages.length > 0}
              aria-describedby={messages.length > 0 ? errorId : undefined}
            />
            {messages.length > 0 && (
              <p className="field-error" id={errorId}>
                {messages.join(' ')}
              </p>
            )}
          </div>
        )
      })}

      <div className="form-actions">
        <button type="submit" className="primary" disabled={mutation.isPending}>
          {mutation.isPending ? 'Saving...' : 'Save customer'}
        </button>
        <button type="button" onClick={onDone} disabled={mutation.isPending}>
          Cancel
        </button>
      </div>
    </form>
  )
}
