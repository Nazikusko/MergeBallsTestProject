using System;
using DG.Tweening;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private const float BALL_SCALE_ANIMATION_DURATION = 0.35f;
    private const float PARTICLE_SCALE_MULTIPLAYER = 0.25f;
    public int BallNumber => _ballNumber;
    public Guid Guid => _guid;
    public bool IsInMargeProcess;

    [SerializeField] private int _ballNumber;
    [SerializeField] private Transform _ballSprite;
    [SerializeField] private ParticleSystem _destroyParticles;
    [SerializeField] private ParticleSystem _spawnParticles;
    [SerializeField] private Color particleColor;

    private Guid _guid;
    private ColliderPart[] _ballColliders;

    private void OnEnable()
    {
        _guid = Guid.NewGuid();
        _ballColliders = GetComponentsInChildren<ColliderPart>();
        IsInMargeProcess = false;
    }

    public void SetKinematic(bool isKinematic)
    {
        foreach (var part in _ballColliders)
        {
            part.Rigidbody.isKinematic = isKinematic;
        }
    }

    public void SetVelocity(Vector3 velocity)
    {
        foreach (var part in _ballColliders)
        {
            part.Rigidbody.velocity = velocity;
        }
    }

    public void Sleep()
    {
        foreach (var part in _ballColliders)
        {
            part.Rigidbody.Sleep();
        }
    }

    public void DestroyBall()
    {
        _destroyParticles.transform.SetParent(null);
        _destroyParticles.GetComponent<ParticleSetup>().SetupParticle(particleColor, 1.5f + (_ballNumber * PARTICLE_SCALE_MULTIPLAYER));
        _destroyParticles.Play(true);

        //_ballSprite.SetParent(null);
        //transform.DOScale(0.1f, BALL_SCALE_ANIMATION_DURATION).SetEase(Ease.OutSine).OnComplete(() => { Destroy(_ballSprite.gameObject); });

        Destroy(_destroyParticles.gameObject, 2f);
        Destroy(gameObject);
    }

    public void SpawnNewBall()
    {
        //transform.localScale = Vector3.one * 0.2f;
        //transform.DOScale(Vector3.one, BALL_SCALE_ANIMATION_DURATION).SetEase(Ease.OutSine);
        _spawnParticles.GetComponent<ParticleSetup>().SetupParticle(particleColor, 1.2f + (_ballNumber * PARTICLE_SCALE_MULTIPLAYER));
        _spawnParticles.Play(true);
    }
}
