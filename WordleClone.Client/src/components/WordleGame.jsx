import { useCallback, useEffect, useMemo, useState } from 'react'
import { getWordls } from '../services/wordlApi.js'

const MAX_GUESSES = 6
const WORD_LENGTH = 5

function getLetterStates(guess, answer) {
  const states = Array(WORD_LENGTH).fill('absent')
  const remaining = {}

  for (let index = 0; index < WORD_LENGTH; index += 1) {
    if (guess[index] === answer[index]) states[index] = 'correct'
    else remaining[answer[index]] = (remaining[answer[index]] ?? 0) + 1
  }

  for (let index = 0; index < WORD_LENGTH; index += 1) {
    if (states[index] === 'correct') continue
    const letter = guess[index]
    if (remaining[letter] > 0) {
      states[index] = 'present'
      remaining[letter] -= 1
    }
  }

  return states
}

function WordleGame({ isLoggedIn, onLoginRequest, onComplete }) {
  const [wordBank, setWordBank] = useState([])
  const [answer, setAnswer] = useState('')
  const [guesses, setGuesses] = useState([])
  const [draft, setDraft] = useState('')
  const [message, setMessage] = useState('')
  const [isLoading, setIsLoading] = useState(true)
  const [isPlaying, setIsPlaying] = useState(false)
  const [result, setResult] = useState(null)

  useEffect(() => {
    let isMounted = true
    getWordls()
      .then((items) => {
        if (!isMounted) return
        const words = items
          .map((item) => item.word?.toLowerCase())
          .filter((word) => Boolean(word && /^[a-z]{5}$/.test(word)))
        setWordBank(words)
      })
      .catch(() => {
        if (isMounted) setMessage('Could not load the word list. Check that the API is running and try again.')
      })
      .finally(() => { if (isMounted) setIsLoading(false) })
    return () => { isMounted = false }
  }, [])

  const keyboardStates = useMemo(() => {
    const states = {}
    guesses.forEach((guess) => {
      getLetterStates(guess, answer).forEach((state, index) => {
        const letter = guess[index]
        if (state === 'correct' || (state === 'present' && states[letter] !== 'correct') || (!states[letter] && state === 'absent')) {
          states[letter] = state
        }
      })
    })
    return states
  }, [answer, guesses])

  function startGame() {
    if (!isLoggedIn) {
      onLoginRequest()
      return
    }
    if (wordBank.length === 0) {
      setMessage('No five-letter words are available yet. Add a Wordl and try again.')
      return
    }
    const nextAnswer = wordBank[Math.floor(Math.random() * wordBank.length)]
    setAnswer(nextAnswer)
    setGuesses([])
    setDraft('')
    setMessage('')
    setResult(null)
    setIsPlaying(true)
  }

  const submitGuess = useCallback(() => {
    if (draft.length !== WORD_LENGTH) {
      setMessage('Your guess needs 5 letters.')
      return
    }
    const nextGuesses = [...guesses, draft]
    setGuesses(nextGuesses)
    setDraft('')
    setMessage('')

    if (draft === answer) {
      setResult('won')
      setIsPlaying(false)
      onComplete({ won: true, attempts: nextGuesses.length })
    } else if (nextGuesses.length >= MAX_GUESSES) {
      setResult('lost')
      setIsPlaying(false)
      onComplete({ won: false, attempts: nextGuesses.length })
    }
  }, [answer, draft, guesses, onComplete])

  useEffect(() => {
    function handleKeyDown(event) {
      if (!isPlaying) return
      if (event.key === 'Enter') {
        event.preventDefault()
        submitGuess()
      } else if (event.key === 'Backspace') {
        setDraft((current) => current.slice(0, -1))
      } else if (/^[a-zA-Z]$/.test(event.key) && draft.length < WORD_LENGTH) {
        setDraft((current) => (current + event.key).toLowerCase())
        setMessage('')
      }
    }
    window.addEventListener('keydown', handleKeyDown)
    return () => window.removeEventListener('keydown', handleKeyDown)
  }, [draft, isPlaying, submitGuess])

  return (
    <section className="game-card" aria-label="Wordl game">
      <div className="game-heading">
        <div>
          <div className="eyebrow">THE DAILY WORD GAME</div>
          <h2>Six tries. One word.</h2>
          <p>Find the hidden five-letter word. Tiles show how close you are.</p>
        </div>
        <div className="tries-badge"><strong>{MAX_GUESSES}</strong><span>tries</span></div>
      </div>

      <div className="game-board" aria-label="Guess board">
        {Array.from({ length: MAX_GUESSES }, (_, rowIndex) => {
          const guess = guesses[rowIndex] ?? (rowIndex === guesses.length && isPlaying ? draft : '')
          const states = guesses[rowIndex] ? getLetterStates(guess, answer) : []
          return (
            <div className="guess-row" key={rowIndex}>
              {Array.from({ length: WORD_LENGTH }, (_, columnIndex) => (
                <div
                  className={`letter-tile ${guess[columnIndex] ? 'has-letter' : ''} ${states[columnIndex] ?? ''}`}
                  key={columnIndex}
                >
                  {guess[columnIndex] ?? ''}
                </div>
              ))}
            </div>
          )
        })}
      </div>

      <div className="game-feedback" aria-live="polite">
        {result === 'won' && <span className="success-message">Brilliant! You got it in {guesses.length} {guesses.length === 1 ? 'try' : 'tries'}.</span>}
        {result === 'lost' && <span>The word was <strong>{answer.toUpperCase()}</strong>. Better luck next time!</span>}
        {!result && message && <span className="form-error">{message}</span>}
      </div>

      <div className="game-actions">
        <button className="button button-primary start-button" onClick={startGame} disabled={isLoading || !isLoggedIn}>
          {isLoading ? 'Loading words…' : isPlaying ? 'Restart game' : result ? 'Play again' : 'Start game'}
          {!isLoggedIn && <span className="lock-mark" aria-label="Sign in required">⌑</span>}
        </button>
        <span className="keyboard-hint">Type a word and press <kbd>ENTER</kbd></span>
      </div>

      <div className="mini-keyboard" aria-label="Keyboard guide">
        {'qwertyuiop asdfghjkl zxcvbnm'.split(' ').map((row) => (
          <div className="key-row" key={row}>
            {row.split('').map((letter) => (
              <span className={`key-cap ${keyboardStates[letter] ?? ''}`} key={letter}>{letter}</span>
            ))}
          </div>
        ))}
      </div>
    </section>
  )
}

export default WordleGame