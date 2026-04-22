import { useEffect, useState } from 'react'
import './App.css'

interface Flag {
  name: string
  enabled: boolean
}

function App() {
  const [flags, setFlags] = useState<Flag[]>([])

  useEffect(() => {
    fetch('http://localhost:5100/api/flags')
      .then(r => r.json())
      .then(setFlags)
      .catch(() => setFlags([]))
  }, [])

  const toggle = async (name: string) => {
    const response = await fetch(`http://localhost:5200/api/flags/${name}/toggle`, { method: 'PUT' })
    if (response.ok) {
      const updated: Flag = await response.json()
      setFlags(prev => prev.map(f => f.name === updated.name ? updated : f))
    }
  }

  const betaExport = flags.find(f => f.name === 'beta-export')?.enabled ?? false

  return (
    <div className="feature-flags">
      <header className="ff-header">
        <span className="ff-logo">Feature Flags</span>
        {betaExport && (
          <button className="ff-export-btn">Export</button>
        )}
      </header>
      <main className="ff-content">
        <h1>Feature Flags</h1>
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
      </main>
    </div>
  )
}

export default App
