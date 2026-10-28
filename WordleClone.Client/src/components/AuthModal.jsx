import { useState } from 'react'
import { login } from '../services/loginApi.js'
import { createUser } from '../services/userApi.js'

function AuthModal({ onClose, onSuccess }) {
  const [mode, setMode] = useState('login')
  const [username, setUsername] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState('')
  const [isSubmitting, setIsSubmitting] = useState(false)

  async function handleSubmit(event) {
    event.preventDefault()
    setError('')
    setIsSubmitting(true)

    try {
      let user
      if (mode === 'login') {
        user = await login({ username, password })
      } else {
        user = await createUser({ username, passwordHash: password })
        if (user.token) localStorage.setItem('token', user.token)
      }
      onSuccess(user)
      onClose()
    } catch (requestError) {
      const message = requestError instanceof Error ? requestError.message : 'Something went wrong. Please try again.'
      setError(message)
    } finally {
      setIsSubmitting(false)
    }
  }

  return (
    <div className="modal-backdrop" role="presentation" onMouseDown={(event) => event.target === event.currentTarget && onClose()}>
      <section className="auth-modal" role="dialog" aria-modal="true" aria-labelledby="auth-title">
        <button className="modal-close" onClick={onClose} aria-label="Close dialog">×</button>
        <div className="eyebrow">YOUR NEXT FAVORITE GAME</div>
        <h2 id="auth-title">{mode === 'login' ? 'Welcome back.' : 'Join the word club.'}</h2>
        <p className="modal-copy">{mode === 'login' ? 'Sign in to play today’s Wordl.' : 'Create an account to start playing and climb the board.'}</p>
        <form className="auth-form" onSubmit={handleSubmit}>
          <label>
            Username
            <input value={username} onChange={(event) => setUsername(event.target.value)} autoComplete="username" minLength={3} maxLength={50} required />
          </label>
          <label>
            Password
            <input type="password" value={password} onChange={(event) => setPassword(event.target.value)} autoComplete={mode === 'login' ? 'current-password' : 'new-password'} minLength={6} required />
          </label>
          {error && <p className="form-error" role="alert">{error}</p>}
          <button className="button button-primary auth-submit" type="submit" disabled={isSubmitting}>
            {isSubmitting ? 'One moment…' : mode === 'login' ? 'Log in' : 'Create account'}
          </button>
        </form>
        <p className="auth-switch">
          {mode === 'login' ? 'New around here?' : 'Already have an account?'}{' '}
          <button onClick={() => { setMode(mode === 'login' ? 'register' : 'login'); setError('') }}>
            {mode === 'login' ? 'Register' : 'Log in'}
          </button>
        </p>
      </section>
    </div>
  )
}

export default AuthModal