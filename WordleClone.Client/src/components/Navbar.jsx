const navItems = [
  { id: 'reviews', label: 'Reviews' },
  { id: 'leaderboard', label: 'Leaderboard' },
  { id: 'add-wordl', label: 'Add a Wordl' },
]

function Navbar({ activeView, isLoggedIn, username, onNavigate, onAccount, onLogout }) {
  return (
    <header className="site-header">
      <button className="brand" onClick={() => onNavigate('play')} aria-label="Wordl home">
        <span className="brand-mark">W</span>
        <span>wordl<span className="brand-period">.</span></span>
      </button>

      <nav className="main-nav" aria-label="Main navigation">
        {navItems.map((item) => (
          <button
            className={`nav-link ${activeView === item.id ? 'is-active' : ''}`}
            key={item.id}
            onClick={() => onNavigate(item.id)}
          >
            {item.label}
          </button>
        ))}
      </nav>

      {isLoggedIn ? (
        <div className="account-actions">
          <button className="account-button" onClick={onAccount} aria-label="Open account">
            <span className="avatar">{username?.slice(0, 1).toUpperCase() || 'P'}</span>
            <span className="account-name">{username || 'Player'}</span>
          </button>
          <button className="logout-button" onClick={onLogout}>Sign out</button>
        </div>
      ) : (
        <button className="nav-auth-button" onClick={onAccount}>Log in / Register</button>
      )}
    </header>
  )
}

export default Navbar