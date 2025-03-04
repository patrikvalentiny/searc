import { useState } from 'react'
import './App.css'
import { SearchBar } from './components/SearchBar'
import { FileCard } from './components/FileCard'
import { GetSearchRequest } from './services/SearchService'
import { FileDetailsDTO } from './models/FileDetailsDTO'

function App() {
  const [searchTerm, setSearchTerm] = useState('')
  const [results, setResults] = useState<FileDetailsDTO[]>([])
  const [loading, setLoading] = useState(false)

  const handleSearch = async () => {
    setLoading(true)
    try {
      // Simulating API response with mock data
      const data = await GetSearchRequest(searchTerm)
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