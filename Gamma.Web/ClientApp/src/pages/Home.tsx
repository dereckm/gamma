import React, { useState } from "react";
import './Home.css'
import Notification, { useNotification } from '../components/Notification'

interface ParserResponse {
  result: string;
  executionTimeMs: number;
}

const Home = () => {
  const [code, setCode] = useState('');
  const [ast, setAst] = useState('');
  const { notification, notify } = useNotification();

  const handleParseClick = () => {
    fetch('http://localhost:5000/javascript/parse', { 
      method: 'POST',
      body: JSON.stringify(code),
      headers: {
        "Content-Type": "application/json"
      }
    })
      .then(response => {
        if (!response.ok) {
          response.text().then(raw => {
            notify(`Could not run code: ${raw}`, 'error')
          })
          return;
        }
        response.json().then((parserResponse: ParserResponse) => {
          setAst(parserResponse.result)
          notify(`Sucessfully parsed code in ${parserResponse.executionTimeMs}ms!`, 'success')
        })
      })
      .catch(console.error)
  }

  const handleRunClick = () => {
    fetch('http://localhost:5000/javascript/interpret', { 
      method: 'POST',
      body: JSON.stringify(code),
      headers: {
        "Content-Type": "application/json"
      }
    })
      .then(response => {
        if (!response.ok) {
          response.text().then(raw => {
            notify(`Could not run code: ${raw}`, 'error')
          })
          return;
        }
        response.json().then((parserResponse: ParserResponse) => {
          notify(`> ${parserResponse.result} (${parserResponse.executionTimeMs}ms)`, 'success')

        })
      })
      .catch(error => {
        console.log(error)
        notify(`Could not run code: ${error}`, 'error')
      })
  }

  

  return (
    <div className='content'>
      <div className="editor">
        <textarea onChange={(v) => setCode(v.currentTarget.value)}>{code}</textarea>
        <div className='code-block'><pre><code>{ast}</code></pre></div>
      </div>
      <div className="controls">
        <button className="control" onClick={handleParseClick}>Parse</button>
        <button className="control" onClick={handleRunClick}>Run</button>
      </div>
      <Notification message={notification.message} type={notification.type} />
    </div>
  )
}



export default Home