import { useState } from 'react'
import './App.css'

interface EmailResult {
  id: number;
  filename: string;
  content: string;
}

function App() {
  const [searchTerm, setSearchTerm] = useState('')
  const [results, setResults] = useState<EmailResult[]>([])
  const [loading, setLoading] = useState(false)

  const handleSearch = async () => {
    setLoading(true)
    try {
      // Simulating API response with mock data
      const data = [
        {
          id: 1,
          filename: "email_001.txt",
          content: "This is a sample email content discussing business matters."
        },
        {
          id: 2,
          filename: "email_002.txt",
          content: "Meeting scheduled for next week. Please prepare the reports."
        }
      ];
      // Simulate network delay
      await new Promise(resolve => setTimeout(resolve, 500));
      setResults(data)
    } catch (error) {
      console.error('Search failed:', error)
      // You might want to add error state handling here
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="container mx-auto p-4">
      <div className="navbar bg-base-100 shadow-lg rounded-box mb-4">
        <div className="flex-1">
          <a className="btn btn-ghost normal-case text-xl">Enron Email Search</a>
        </div>
      </div>

      <div className="form-control w-full max-w-2xl mx-auto mb-8">
        <div className="input-group">
          <input
            type="text"
            placeholder="Search emails..."
            className="input input-bordered w-full"
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
          />
          <button
            className={`btn btn-primary ${loading ? 'loading' : ''}`}
            onClick={handleSearch}
          >
            Search
          </button>
        </div>
      </div>

      <div className="space-y-4">
        {results.map(result => (
          <div key={result.id} className="card bg-base-100 shadow-xl">
            <div className="card-body">
              <h2 className="card-title">File: {result.filename}</h2>
              <p className="mt-2">{result.content}</p>
              <div className="card-actions justify-end">
                {/* <button
                  className="btn btn-primary"
                  onClick={() => window.open(`http://localhost:3000/api/download/${result.id}`, '_blank')}
                >
                  Download
                </button> */}
              </div>
            </div>
          </div>
        ))}
      </div>
    </div>
  )
}

export default App