import { useCallback, useEffect, useState } from 'react'
import './App.css'
import { getUserById, updateUserStats } from './services/userApi.js'
import AccountPanel from './components/AccountPanel'
import AddWordl from './components/AddWordl'
import AuthModal from './components/AuthModal'
import Leaderboard from './components/Leaderboard'
import Navbar from './components/Navbar'
import Reviews from './components/Reviews'
import WordleGame from './components/WordleGame'

function App() {
  const [player, setPlayer] = useState(() => {
    const token = localStorage.getItem('token')
    if (!token) return null
    return {
      id: Number(localStorage.getItem('userId') || 0),
      username: localStorage.getItem('username') || 'Player',
      token,
      points: 0,
      gamesWon: 0,
    }
  })
  const [view, setView] = useState('play')
  const [showAuth, setShowAuth] = useState(false)
  const [wordRefresh, setWordRefresh] = useState(0)
  const [notice, setNotice] = useState('')

  useEffect(() => {
    if (!player?.id) return
    getUserById(player.id)
      .then((profile) => {
        setPlayer((current) => current ? { ...current, points: profile.points, gamesWon: profile.gamesWon } : current)
      })
      .catch(() => setNotice('Could not refresh your account details.'))
  }, [player?.id])

  const handleAuthenticated = useCallback((user) => {
    localStorage.setItem('token', user.token)
    localStorage.setItem('userId', String(user.id))
    localStorage.setItem('username', user.username)
    setPlayer({ ...user, points: 0, gamesWon: 0 })
    setView('play')
    setNotice(`Welcome, ${user.username}. Your next Wordl is waiting.`)
  }, [])

  function handleLogout() {
    localStorage.removeItem('token')
    localStorage.removeItem('userId')
    localStorage.removeItem('username')
    setPlayer(null)
    setView('play')
    setNotice('You are signed out. Come back for another round soon.')
  }

  async function handleGameComplete(result) {
    if (!player) return
    const nextPlayer = {
      ...player,
      points: player.points + (result.won ? (7 - result.attempts) * 10 : 0),
      gamesWon: player.gamesWon + (result.won ? 1 : 0),
    }
    setPlayer(nextPlayer)
    try {
      await updateUserStats({ points: nextPlayer.points, gamesWon: nextPlayer.gamesWon })
    } catch {
      setNotice('Game finished, but your stats could not be saved. Please sign in again if your session expired.')
    }
  }

  function handleNavigate(nextView) {
    if (nextView === 'add-wordl' && !player) {
      setShowAuth(true)
      setNotice('Sign in to add a word to the pool.')
      return
    }
    setView(nextView)
    setNotice('')
  }

  return (
    <div className="app-shell">
      <Navbar
        activeView={view}
        isLoggedIn={Boolean(player)}
        username={player?.username}
        onNavigate={handleNavigate}
        onAccount={() => player ? setView('account') : setShowAuth(true)}
        onLogout={handleLogout}
      />

      <main className="page-content">
        {notice && <div className="notice-banner" role="status"><span>✳</span>{notice}<button onClick={() => setNotice('')} aria-label="Dismiss message">×</button></div>}

        {view === 'play' && (
          <>
            <section className="hero-section">
              <div className="hero-copy">
                <div className="eyebrow hero-eyebrow"><span className="live-dot" /> A LITTLE WORD GAME, A LOT OF JOY</div>
                <h1>Find the word.<br /><span>Find your flow.</span></h1>
                <p>A fresh five-letter challenge, every time. Take a breath, trust your instincts, and make every guess count.</p>
                <div className="hero-proof"><div className="proof-avatars"><span>J</span><span>M</span><span>A</span></div><span>Made for curious minds</span><span className="proof-star">✳</span></div>
              </div>
              <div className="hero-art" aria-hidden="true">
                <div className="floating-word"><span className="art-tile art-green">W</span><span className="art-tile art-yellow">O</span><span className="art-tile">R</span><span className="art-tile art-green">D</span><span className="art-tile">S</span></div>
                <div className="art-sparkle sparkle-one">✳</div><div className="art-sparkle sparkle-two">✦</div>
                <span className="art-caption">YOUR DAILY<br />MOMENT OF FOCUS</span>
              </div>
            </section>
            <WordleGame key={wordRefresh} isLoggedIn={Boolean(player)} onLoginRequest={() => setShowAuth(true)} onComplete={handleGameComplete} />
            <div className="how-to-play"><span className="how-number">01</span><p><strong>Guess the word</strong><br />You have six chances to find today’s five-letter word.</p><span className="how-divider" /><span className="how-number">02</span><p><strong>Follow the colors</strong><br />Green is right, yellow is close, gray is not in the word.</p></div>
          </>
        )}
        {view === 'reviews' && <Reviews isLoggedIn={Boolean(player)} userId={player?.id} username={player?.username} onLoginRequest={() => setShowAuth(true)} />}
        {view === 'leaderboard' && <Leaderboard />}
        {view === 'add-wordl' && <AddWordl onAdded={() => setWordRefresh((current) => current + 1)} />}
        {view === 'account' && player && <AccountPanel username={player.username} points={player.points} gamesWon={player.gamesWon} onLogout={handleLogout} />}
      </main>

      <footer className="site-footer"><span>© 2026 WORDL CLUB</span><span>MADE FOR THE LOVE OF WORDS <b>✳</b></span></footer>
      {showAuth && <AuthModal onClose={() => setShowAuth(false)} onSuccess={handleAuthenticated} />}
    </div>
  )
}

export default App