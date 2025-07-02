using UnityEngine;
#if MIRROR
using Mirror;
#endif

namespace AndreaFrigerio.Gameplay
{
#if MIRROR
    public class BallController : NetworkBehaviour
#else
    public class BallController : MonoBehaviour
#endif
    {
        public float speed = 15f;
        public float fieldLimitX = 9f;
        public Rigidbody2D rb;

        private Vector2 direction = Vector2.right;

        private bool IsServerOrLocal =>
#if MIRROR
            isServer;
#else
            true;
#endif

        private void Start()
        {
            if (IsServerOrLocal)
            {
                ResetBall(Random.value < 0.5f ? Vector2.right : Vector2.left);
            }
            else
            {
                rb.simulated = false;
            }
        }

        private void Update()
        {
            if (!IsServerOrLocal) return;

            rb.linearVelocity = direction.normalized * speed;

            // Check field bounds
            if (Mathf.Abs(transform.position.x) > fieldLimitX)
            {
                bool leftScored = transform.position.x > 0;
                Debug.Log($"Goal! {(leftScored ? "Left" : "Right")} scores");

                // Notify game manager (local or networked)
                //GameManager.Instance?.OnBallOut(leftScored);

                ResetBall(leftScored ? Vector2.left : Vector2.right);
            }
        }

        private void OnCollisionEnter2D(Collision2D col)
        {
            if (!IsServerOrLocal) return;

            if (col.gameObject.CompareTag("Paddle"))
            {
                float y = HitFactor(transform.position, col.transform.position, col.collider.bounds.size.y);
                float x = col.relativeVelocity.x > 0 ? 1 : -1;
                direction = new Vector2(x, y).normalized;
            }
            else if (col.gameObject.CompareTag("Wall"))
            {
                direction.y *= -1;
            }
        }

        private void ResetBall(Vector2 dir)
        {
            rb.linearVelocity = Vector2.zero;
            transform.position = Vector3.zero;
            direction = dir.normalized;
        }

        private float HitFactor(Vector2 ballPos, Vector2 racketPos, float racketHeight)
        {
            return (ballPos.y - racketPos.y) / racketHeight;
        }
    }
}
