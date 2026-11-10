import react from '@vitejs/plugin-react'
import { defineConfig } from 'vite'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    // In development the browser talks to Vite (port 5173), and Vite forwards /api/* to the
    // ASP.NET Core API. That way the browser sees one origin and we don't need CORS.
    proxy: {
      '/api': 'http://localhost:5193',
    },
  },
})
