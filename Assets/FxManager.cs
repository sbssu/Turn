using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FxManager : MonoBehaviour
{
    public static FxManager Instance { get; private set; }

    [SerializeField] ParticleSystem[] particles;

    private void Awake()
    {
        Instance = this;
    }
    public void PlayFx(string name, Vector3 position)
    {
        ParticleSystem prefeb = System.Array.Find(particles, p => p.name == name);

        if (prefeb != null)
        {
            ParticleSystem particle = Instantiate(prefeb);
            particle.transform.position = position;
            particle.Play();
        }
    }

}
