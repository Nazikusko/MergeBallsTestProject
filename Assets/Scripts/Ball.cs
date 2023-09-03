using System;
using DG.Tweening;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private const float BALL_SCALE_ANIMATION_DURATION = 0.35f;
    private const float PARTICLE_SCALE_MULTIPLAYER = 0.25f;

    [SerializeField] private int _ballNumber;
    [SerializeField] private Transform _ballSprite;
    [SerializeField] private Rigidbody2D _ballRigidBody;
    [SerializeField] private ParticleSystem _destroyParticles;
    [SerializeField] private ParticleSystem _spawnParticles;
    [SerializeField] private Color particleColor;

    public static Action<Ball, Ball, Vector2> OnBallsMatch;

    public int BallNumber => _ballNumber;
    public Rigidbody2D BallRigidBody => _ballRigidBody;

    public void DestroyBall()
    {
        _destroyParticles.transform.SetParent(null);
        _destroyParticles.GetComponent<ParticleSetup>().SetupParticle(particleColor, 1.5f + (_ballNumber * PARTICLE_SCALE_MULTIPLAYER));
        _destroyParticles.Play(true);

        _ballSprite.SetParent(null);
        _ballSprite.DOScale(0.1f, BALL_SCALE_ANIMATION_DURATION).SetEase(Ease.OutSine).OnComplete(() => { Destroy(_ballSprite.gameObject); });

        Destroy(gameObject);
        Destroy(_destroyParticles.gameObject, 2f);
    }

    public void SpawnedNewBall()
    {
        _ballSprite.localScale = Vector3.one * 0.2f;
        _ballSprite.DOScale(Vector3.one, BALL_SCALE_ANIMATION_DURATION).SetEase(Ease.OutSine);
        _spawnParticles.GetComponent<ParticleSetup>().SetupParticle(particleColor, 1.2f + (_ballNumber * PARTICLE_SCALE_MULTIPLAYER));
        _spawnParticles.Play(true);
    }

    void Update()
    {
        _ballSprite.rotation = Quaternion.identity;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!gameObject.activeSelf || collision.gameObject.layer != LayerMask.NameToLayer("Balls")) return;

        var otherBall = collision.gameObject.GetComponent<Ball>();

        if (otherBall.BallNumber == _ballNumber || _ballNumber == 10)
        {
            otherBall.gameObject.SetActive(false);
            OnBallsMatch?.Invoke(this, otherBall, collision.GetContact(0).point);
        }
    }
}
