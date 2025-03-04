import { useState } from 'react'
import './App.css'

function App() {
  const [count, setCount] = useState(0)

  return (
    <>
      <button className="btn" onClick={() => setCount(count + 1)}>
        Hello world!
      </button>
      <div>{count}</div>
    </>
  )
}

export default App
