import { useState } from 'react'
import { createWordl } from '../services/wordlApi.js'

function AddWordl({ onAdded }) {
  const [word, setWord] = useState('')
  const [error, setError] = useState('')
  const [success, setSuccess] = useState('')
  const [isSubmitting, setIsSubmitting] = useState(false)

  async function handleSubmit(event) {
    event.preventDefault()
    const normalizedWord = word.trim().toLowerCase()
    if (!/^[a-z]{5}$/.test(normalizedWord)) {
      setError('A Wordl must be exactly five letters, A–Z.')
      setSuccess('')
      return
    }
    setError('')
    setSuccess('')
    setIsSubmitting(true)
    try {
      await createWordl({ word: normalizedWord })
      setWord('')
      setSuccess(`${normalizedWord.toUpperCase()} has been added to the word pool.`)
      onAdded()
    } catch {
      setError('Could not add that word. It may already exist, or your session may have expired.')
    } finally {
      setIsSubmitting(false)
    }
  }

  return (
    <section className="content-card add-word-card">
      <div className="section-heading">
        <div><div className="eyebrow">GROW THE WORD POOL</div><h2>Add a Wordl</h2></div>
        <span className="section-icon">＋</span>
      </div>
      <p className="section-intro">Know a good five-letter word? Add it to the pool for everyone to discover.</p>
      <form className="add-word-form" onSubmit={handleSubmit}>
        <label htmlFor="new-word">Your five-letter word</label>
        <div className="word-entry-row">
          <input id="new-word" className="word-entry" value={word} onChange={(event) => setWord(event.target.value.replace(/[^a-z]/gi, '').slice(0, 5))} maxLength={5} placeholder="e.g. CRANE" autoComplete="off" />
          <button className="button button-primary" type="submit" disabled={isSubmitting}>{isSubmitting ? 'Adding…' : 'Add word ↗'}</button>
        </div>
        <span className="input-helper">Letters only · five characters · lowercase is fine</span>
        {error && <p className="form-error" role="alert">{error}</p>}
        {success && <p className="success-message" role="status">{success}</p>}
      </form>
      <div className="word-pool-note"><span>✳</span><p>Keep it friendly: please submit everyday words that make the game fun for everyone.</p></div>
    </section>
  )
}

export default AddWordl