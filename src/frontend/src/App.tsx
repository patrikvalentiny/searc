import { useState } from 'react'
import './App.css'
import { SearchBar } from './components/SearchBar'
import { FileCard } from './components/FileCard'

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
    <div className="flex flex-col items-center justify-center h-full w-full py-2 gap-4">
      <SearchBar 
        searchTerm={searchTerm}
        setSearchTerm={setSearchTerm}
        handleSearch={handleSearch}
        loading={loading}
      />

      <div className="flex flex-row flex-wrap gap-4 w-full justify-center">
        {results.map(result => (
          <FileCard key={result.id} result={result} />
        ))}
      </div>
    </div>
  )
}

export default App