using System.Collections.Generic;
using UnityEngine;

public class ParticleSetup : MonoBehaviour
{
    [SerializeField] private List<ParticleSystem> _particlesToSetup;

    public void SetupParticle(Color color, float scale)
    {
        foreach (ParticleSystem particle in _particlesToSetup)
        {
            ParticleSystem.MainModule settings = particle.main;
            settings.startColor = new ParticleSystem.MinMaxGradient(color);
        }

        transform.localScale = Vector3.one * scale;
    }
}
