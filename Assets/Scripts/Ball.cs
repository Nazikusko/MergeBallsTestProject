using System;
using DG.Tweening;
using UnityEngine;

public enum BallInitMode
{
    Soft,
    Hard,
    Ui,
}

public class Ball : MonoBehaviour
{
    private const float BALL_SCALE_ANIMATION_DURATION = 0.35f;
    private const float PARTICLE_SCALE_MULTIPLAYER = 0.25f;
    public int BallNumber => _ballNumber;
    public Guid Guid => _guid;
    [HideInInspector] public bool IsInMergeProcess;

    [SerializeField] private int _ballNumber;
    [SerializeField] private Transform _softBall;
    [SerializeField] private Transform _spriteBall;
    [SerializeField] private ParticleSystem _destroyParticles;
    [SerializeField] private ParticleSystem _spawnParticles;
    [SerializeField] private Color particleColor;

    private Guid _guid;
    private ColliderPart[] _softBallColliders;
    private Rigidbody2D _hardBallRigidbody;

    private void OnEnable()
    {
        _guid = Guid.NewGuid();
        _softBallColliders = _softBall.GetComponentsInChildren<ColliderPart>();
        _hardBallRigidbody = _spriteBall.GetComponent<Rigidbody2D>();
        IsInMergeProcess = false;
        _softBall.gameObject.SetActive(false);
        _spriteBall.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        DOTween.Kill(this);
    }

    public void Init(BallInitMode mode)
    {
        switch (mode)
        {
            case BallInitMode.Soft:
                SwitchBalls(true); break;
            case BallInitMode.Hard:
                SwitchBalls(false);
                SetKinematic(true);
                break;
            case BallInitMode.Ui:
                SetUiMode(); break;
        }
    }

    private void SetUiMode()
    {
        SwitchBalls(false);
        _spriteBall.GetComponent<Collider2D>().enabled = false;
        SetKinematic(true);
    }

    public void SetKinematic(bool isKinematic)
    {
        if (_softBall.gameObject.activeSelf)
        {
            foreach (var part in _softBallColliders)
            {
                part.Rigidbody.isKinematic = isKinematic;
            }
        }

        if (_spriteBall.gameObject.activeSelf)
        {
            _hardBallRigidbody.isKinematic = isKinematic;
        }
    }

    public void SetVelocity(Vector3 velocity)
    {
        if (_softBall.gameObject.activeSelf)
        {
            foreach (var part in _softBallColliders)
            {
                part.Rigidbody.velocity = velocity;
            }
        }

        if (_spriteBall.gameObject.activeSelf)
        {
            _hardBallRigidbody.velocity = velocity;
        }
    }

    public void Sleep()
    {
        if (_softBall.gameObject.activeSelf)
        {
            foreach (var part in _softBallColliders)
            {
                part.Rigidbody.velocity = Vector2.zero;
                part.Rigidbody.angularVelocity = 0;
            }
        }

        if (_spriteBall.gameObject.activeSelf)
        {
            _hardBallRigidbody.Sleep();
        }
    }

    public void DestroyBall()
    {
        _destroyParticles.transform.SetParent(null);
        _destroyParticles.GetComponent<ParticleSetup>().SetupParticle(particleColor, 1.5f + (_ballNumber * PARTICLE_SCALE_MULTIPLAYER));
        _destroyParticles.Play(true);

        Destroy(gameObject);
        Destroy(_destroyParticles.gameObject, 2f);
    }

    public void SpawnNewBall()
    {
        SwitchBalls(false);

        _spriteBall.localScale = Vector3.one * 0.2f;
        _spriteBall.DOScale(Vector3.one, BALL_SCALE_ANIMATION_DURATION).SetEase(Ease.OutSine).OnComplete(() => { SwitchBalls(true); }).SetId(this);

        _spawnParticles.GetComponent<ParticleSetup>().SetupParticle(particleColor, 1.2f + (_ballNumber * PARTICLE_SCALE_MULTIPLAYER));
        _spawnParticles.Play(true);
    }

    private void SwitchBalls(bool isSoftBallOn)
    {
        _softBall.gameObject.SetActive(isSoftBallOn);
        _spriteBall.gameObject.SetActive(!isSoftBallOn);
    }
}
