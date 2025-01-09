using System;
using UnityEngine;

public class ColliderPart : MonoBehaviour
{
    public static event Action<Ball, Ball, Vector2> OnBallsMatch;

    private Rigidbody2D _rigidbody;
    private Ball _ball;

    public Ball Ball => _ball;
    public Rigidbody2D Rigidbody => _rigidbody;

    private void OnEnable()
    {
        _ball = transform.parent.parent.GetComponent<Ball>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _rigidbody.mass = _ball.BallNumber * 0.01f;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (_ball.IsInMergeProcess || collision.gameObject.layer != LayerMask.NameToLayer("Balls"))
            return;

        var otherBall = collision.transform.GetComponent<ColliderPart>().Ball;
        if (otherBall.Guid.Equals(_ball.Guid) || otherBall.IsInMergeProcess)
            return;

        if (otherBall.BallNumber == _ball.BallNumber || _ball.BallNumber == 10 || otherBall.BallNumber == 10)
        {
            if (_ball.BallNumber != 10)
            {
                _ball.IsInMergeProcess = true;
            }

            if (otherBall.BallNumber != 10)
            {
                otherBall.IsInMergeProcess = true;
            }

            OnBallsMatch?.Invoke(_ball, otherBall, collision.GetContact(0).point);
        }
    }
}
