using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemSound : MonoBehaviour
{
    [SerializeField] AudioClip PartcileBirthSound;
    private ParticleSystem particleSystem;
    private int numberOfParticles = 0;
    private GameObject audioSources;

    private void Start()
    {
        audioSources = new GameObject("PSAudioSources");
        audioSources.transform.SetParent(transform);
        particleSystem = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        int count = particleSystem.particleCount;
        if (count > numberOfParticles)
        {
            AudioSource a = audioSources.AddComponent<AudioSource>();
            a.clip = PartcileBirthSound;
            a.playOnAwake = false;
            a.Play();
        }
        numberOfParticles = count;
    }
}
