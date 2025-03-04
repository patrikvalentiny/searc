import { useState } from 'react'
import './App.css'

interface EmailResult {
  id: number;
  content: string;
  filename: string;
  timestamp: string;
}

function App() {
  const [searchTerm, setSearchTerm] = useState('')
  const [results, setResults] = useState<EmailResult[]>([])
  const [loading, setLoading] = useState(false)

  const handleSearch = async () => {
    setLoading(true)
    // TODO: Implement actual API call
    await new Promise(resolve => setTimeout(resolve, 1000))
    setResults([
      { id: 1, content: "Sample email content...", filename: "email1.txt", timestamp: "2001-05-10" },
      { id: 2, content: "Another email content...", filename: "email2.txt", timestamp: "2001-05-11" }
    ])
    setLoading(false)
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
              <p className="text-sm text-gray-500">Date: {result.timestamp}</p>
              <p className="mt-2">{result.content}</p>
              <div className="card-actions justify-end">
                <button className="btn btn-primary">Download</button>
              </div>
              <