import { useEffect, useState } from 'react'
import './App.css'

interface Flag {
  name: string
  enabled: boolean
}

interface PagedResult<T> {
  items: T[]
  page: {
    page: number
    pageSize: number
    totalCount: number
    totalPages: number
  }
}

function App() {
  const [flags, setFlags] = useState<Flag[]>([])
  const [page, setPage] = useState(1)
  const [hasNextPage, setHasNextPage] = useState(true)
  const [loading, setLoading] = useState(false)
  const pageSize = 5

  const isStandalone = window.location.port === '3001'
  const backendBase = isStandalone ? 'http://localhost:5200' : 'http://localhost:5100'

  useEffect(() => {
    setLoading(true)
    fetch(`${backendBase}/api/flags?page=${page}&pageSize=${pageSize}`)
      .then(r => {
        if (!r.ok) throw new Error('Network response was not ok');
        return r.json();
      })
      .then((result: PagedResult<Flag>) => {
        if (result && result.items) {
          setFlags(prev => [...prev, ...result.items])
          setHasNextPage(result.page.page < result.page.totalPages)
        }
        setLoading(false)
      })
      .catch(() => {
        setLoading(false)
      })
  }, [page, backendBase])

  const toggle = async (name: string) => {
    const response = await fetch(`http://localhost:5200/api/flags/${name}/toggle`, { method: 'PUT' })
    if (response.ok) {
      const updated: Flag = await response.json()
      setFlags(prev => prev.map(f => f.name === updated.name ? updated : f))

      // Apply theme immediately if standalone and toggling dark-mode
      if (isStandalone && updated.name === 'dark-mode') {
        document.documentElement.classList.toggle('dark', updated.enabled)
      }
    }
  }

  // Initial theme application for standalone
  useEffect(() => {
    if (isStandalone && flags.length > 0) {
      const dark = flags.find(f => f.name === 'dark-mode')?.enabled ?? false
      document.documentElement.classList.toggle('dark', dark)
    }
  }, [isStandalone, flags])

  const betaExport = flags.find(f => f.name === 'beta-export')?.enabled ?? false

  return (
    <div className={`feature-flags ${isStandalone ? 'is-standalone' : 'is-remote'}`}>
      {isStandalone && (
        <header className="ff-header">
          <span className="ff-logo">Feature Flags (Standalone)</span>
          {betaExport && (
            <button className="ff-export-btn">Export</button>
          )}
        </header>
      )}
      <main className="ff-content">
        <h1>Feature Flags</h1>
        {betaExport && !isStandalone && (
          <div className="ff-actions">
            <button className="ff-export-btn">Export</button>
          </div>
        )}
        <table className="ff-table">
          <thead>
            <tr>
              <th>Flag</th>
              <th>Status</th>
              <th>Actie</th>
            </tr>
          </thead>
          <tbody>
            {flags.map(flag => (
              <tr key={flag.name}>
                <td>{flag.name}</td>
                <td className={flag.enabled ? 'enabled' : 'disabled'}>
                  {flag.enabled ? 'aan' : 'uit'}
                </td>
                <td>
                  <button onClick={() => toggle(flag.name)}>
                    {flag.enabled ? 'Uitschakelen' : 'Inschakelen'}
                  </button>
                </td>
              </tr>
            ))}
          </tbody>
        </table>

        <div className="ff-pagination">
          {hasNextPage && (
            <button 
              className="ff-load-more" 
              onClick={() => setPage(prev => prev + 1)}
              disabled={loading}
            >
              {loading ? 'Laden...' : 'Meer laden'}
            </button>
          )}
          {!hasNextPage && flags.length > 0 && (
            <p className="ff-end-message">Geen flags meer beschikbaar.</p>
          )}
        </div>
      </main>
    </div>
  )
}

export default App
