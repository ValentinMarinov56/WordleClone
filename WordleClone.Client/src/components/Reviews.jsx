import { useCallback, useEffect, useState } from 'react'
import { createReview, deleteReview, getReviews } from '../services/reviewApi.js'

function Reviews({ isLoggedIn, userId, username, onLoginRequest }) {
  const [reviews, setReviews] = useState([])
  const [text, setText] = useState('')
  const [error, setError] = useState('')
  const [isLoading, setIsLoading] = useState(true)
  const [isPosting, setIsPosting] = useState(false)
  const [deletingReviewId, setDeletingReviewId] = useState(null)

  const loadReviews = useCallback(() => {
    getReviews()
      .then((items) => setReviews(items))
      .catch(() => setError('Could not load reviews right now.'))
      .finally(() => setIsLoading(false))
  }, [])

  useEffect(() => { loadReviews() }, [loadReviews])

  async function handleSubmit(event) {
    event.preventDefault()
    if (!isLoggedIn) {
      onLoginRequest()
      return
    }
    const trimmedText = text.trim()
    if (!trimmedText) return
    setError('')
    setIsPosting(true)
    try {
      await createReview({ text: trimmedText })
      setText('')
      loadReviews()
    } catch {
      setError('Your review could not be posted. Please try again.')
    } finally {
      setIsPosting(false)
    }
  }

  async function handleDelete(reviewId) {
    setError('')
    setDeletingReviewId(reviewId)
    try {
      await deleteReview(reviewId)
      setReviews((current) => current.filter((review) => review.id !== reviewId))
    } catch {
      setError('Could not delete your review. Please try again.')
    } finally {
      setDeletingReviewId(null)
    }
  }

  return (
    <section className="content-card reviews-card">
      <div className="section-heading">
        <div><div className="eyebrow">NOTES FROM THE COMMUNITY</div><h2>Wordl reviews</h2></div>
        <span className="section-icon">✳</span>
      </div>
      <p className="section-intro">A little love for the game, from the people who play it.</p>
      <form className="review-composer" onSubmit={handleSubmit}>
        <div className="composer-avatar avatar">{username?.slice(0, 1).toUpperCase() || 'W'}</div>
        <div className="composer-main">
          <textarea aria-label="Write a review" placeholder={isLoggedIn ? 'What do you think of Wordl?' : 'Sign in to leave a review…'} value={text} onChange={(event) => setText(event.target.value)} maxLength={500} rows={3} />
          <div className="composer-footer">
            <span>{text.length}/500</span>
            <button className="button button-dark" type="submit" disabled={isPosting || (isLoggedIn && !text.trim())}>
              {isPosting ? 'Posting…' : isLoggedIn ? 'Post review ↗' : 'Log in to review'}
            </button>
          </div>
        </div>
      </form>
      {error && <p className="form-error review-error" role="alert">{error}</p>}
      <div className="review-list">
        {isLoading ? <div className="empty-state">Gathering kind words…</div> : reviews.length === 0 ? <div className="empty-state">No reviews yet. Start the conversation.</div> : reviews.map((review) => (
          <article className="review-item" key={review.id}>
            <span className="avatar avatar-small">{(review.authorUsername || '?').slice(0, 1).toUpperCase()}</span>
            <div className="review-body"><div className="review-meta"><strong>{review.authorUsername || 'Wordl player'}</strong><span>Community member</span></div><p>{review.text}</p></div>
            {isLoggedIn && Number(review.userId) === userId && (
              <button
                className="review-delete-button"
                onClick={() => handleDelete(review.id)}
                disabled={deletingReviewId === review.id}
                aria-label={`Delete review by ${review.authorUsername}`}
              >
                {deletingReviewId === review.id ? 'Deleting…' : 'Delete'}
              </button>
            )}
            <span className="review-spark">✳</span>
          </article>
        ))}
      </div>
    </section>
  )
}

export default Reviews