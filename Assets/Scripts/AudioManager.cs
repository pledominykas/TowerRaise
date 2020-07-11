using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public enum SoundType { OnDeath, OnShoot, OnLevelComplete, OnBrickBuy, OnButtonClick}
    public List<Sound> Sounds = new List<Sound>();
    private Dictionary<SoundType, AudioSource> audioSources = new Dictionary<SoundType, AudioSource>();


    private void Awake()
    {
        foreach(Sound s in Sounds)
        {
            AudioSource audioS = gameObject.AddComponent<AudioSource>();
            audioS.clip = s.Clip;
            audioS.volume = s.volume;
            audioS.loop = s.loop;
            audioS.pitch = s.pitch;
            audioSources.Add(s.Key, audioS);
        }
    }

    public void PlaySound(SoundType _type)
    {
        if (audioSources.ContainsKey(_type))
        {
            audioSources[_type].Play();
        }
    }


    [System.Serializable]
    public class Sound
    {
        public SoundType Key;
        public AudioClip Clip;
        public bool loop = false;
        public float volume = 1f;
        public float pitch = 1f;
    }
}

