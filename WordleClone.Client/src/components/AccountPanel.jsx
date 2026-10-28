function AccountPanel({ username, points, gamesWon, onLogout }) {
  return (
    <section className="content-card account-card">
      <div className="section-heading">
        <div><div className="eyebrow">PLAYER PROFILE</div><h2>Your account</h2></div>
        <span className="avatar account-large-avatar">{username.slice(0, 1).toUpperCase()}</span>
      </div>
      <div className="account-welcome"><p>Good to have you here,</p><h3>{username}<span>.</span></h3></div>
      <div className="account-stats">
        <div><span>Total points</span><strong>{points.toLocaleString()}</strong></div>
        <div><span>Games won</span><strong>{gamesWon.toLocaleString()}</strong></div>
      </div>
      <button className="button button-outline" onClick={onLogout}>Sign out of Wordl</button>
    </section>
  )
}

export default AccountPanel