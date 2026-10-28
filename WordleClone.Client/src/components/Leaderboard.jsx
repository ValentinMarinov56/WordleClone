import { useEffect, useState } from 'react'
import { getUsers } from '../services/userApi.js'

function Leaderboard() {
  const [players, setPlayers] = useState([])
  const [error, setError] = useState('')
  const [isLoading, setIsLoading] = useState(true)

  useEffect(() => {
    getUsers()
      .then((users) => setPlayers([...users].sort((a, b) => b.points - a.points)))
      .catch(() => setError('Could not load the leaderboard. Please try again later.'))
      .finally(() => setIsLoading(false))
  }, [])

  return (
    <section className="content-card leaderboard-card">
      <div className="section-heading">
        <div><div className="eyebrow">THE WORDL COMMUNITY</div><h2>Leaderboard</h2></div>
        <span className="section-icon">♛</span>
      </div>
      <p className="section-intro">The sharpest minds, ranked by points.</p>
      {isLoading ? <div className="empty-state">Loading players…</div> : error ? <div className="empty-state form-error">{error}</div> : players.length === 0 ? <div className="empty-state">No players yet. Be the first to play.</div> : (
        <div className="leaderboard-table-wrap">
          <table className="leaderboard-table">
            <thead><tr><th>Rank</th><th>Player</th><th>Wins</th><th>Points</th></tr></thead>
            <tbody>
              {players.map((player, index) => (
                <tr key={player.id}>
                  <td><span className={`rank-number ${index < 3 ? `rank-${index + 1}` : ''}`}>{String(index + 1).padStart(2, '0')}</span></td>
                  <td><span className="player-cell"><span className="avatar avatar-small">{player.username.slice(0, 1).toUpperCase()}</span><strong>{player.username}</strong></span></td>
                  <td>{player.gamesWon}</td>
                  <td className="points-cell">{player.points.toLocaleString()} <span>pts</span></td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </section>
  )
}

export default Leaderboard