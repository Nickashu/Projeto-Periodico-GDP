using System;
using System.Linq;
using UnityEngine;

public class SoundController : MonoBehaviour {   //Será uma classe Singleton
    private static SoundController instance;

    public Sound[] sounds;

    public static SoundController GetInstance() {
        return instance;
    }

    private void Awake() {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        foreach(Sound s in sounds) {
            foreach(GameObject go in s.origins) {
                s.audioSource = go.AddComponent<AudioSource>();
                s.audioSource.clip = s.clip;
                s.audioSource.clip.name = s.name;
                s.audioSource.volume = s.volume;
                s.audioSource.pitch = s.pitch;
                s.audioSource.loop = s.loop;
                s.audioSource.playOnAwake = false;    //Sons que tocam logo no início serão adicionados diretamente aos seus gameObjects
                //Configurações para som 3D:
                if (s.is3D) {
                    s.audioSource.spatialBlend = 1f;
                    s.audioSource.maxDistance = 25f;
                    s.audioSource.minDistance = 3f;
                    s.audioSource.rolloffMode = AudioRolloffMode.Linear;
                }
            }
        }
    }

    public void PlaySound(string soundName, GameObject go) {
        Sound s = Array.Find(sounds, sound => sound.name.Equals(soundName));   //Procurando o som informado pelo seu nome
        if (s != null) {
            AudioSource[] audios = go.GetComponents<AudioSource>();
            audios.FirstOrDefault(a => a.clip.name.Equals(soundName)).Play();
        }
    }

    public void PauseSound(string soundName) {
        Sound s = Array.Find(sounds, sound => sound.name.Equals(soundName));   //Procurando o som informado pelo seu nome
        if (s != null)
            s.audioSource.Pause();
    }
}
