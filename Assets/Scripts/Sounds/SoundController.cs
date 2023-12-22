using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoundController : MonoBehaviour {   //Será uma classe Singleton
    private static SoundController instance;

    public Sound[] sounds;
    private Dictionary<string, bool> isPlayingOST = new Dictionary<string, bool> { {"OST_safe", false}, { "OST_trilha1", false }, { "OST_trilha1_timer", false }, { "OST_tension", false }, {"OST_menu", false } };
    private Dictionary<string, float> volumeOSTs = new Dictionary<string, float> { {"OST_safe", 0.5f }, { "OST_trilha1", 0.5f }, { "OST_trilha1_timer", 0.5f }, { "OST_tension", 0.8f }, {"OST_menu", 1 } };

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
            if(s.origins.Count() == 0) {   //Se o som não tiver origem especificada
                s.audioSource = gameObject.AddComponent<AudioSource>();
                s.audioSource.clip = s.clip;
                s.audioSource.clip.name = s.name;
                s.audioSource.volume = s.volume;
                s.audioSource.pitch = s.pitch;
                s.audioSource.loop = s.loop;
                s.audioSource.playOnAwake = false;    //Sons que tocam logo no início serão adicionados diretamente aos seus gameObjects
                if (s.is3D) {
                    s.audioSource.spatialBlend = 1f;
                    s.audioSource.maxDistance = 25f;
                    s.audioSource.minDistance = 3f;
                    s.audioSource.rolloffMode = AudioRolloffMode.Linear;
                }
            }
            else {
                foreach (GameObject go in s.origins) {
                    s.audioSource = go.AddComponent<AudioSource>();
                    s.audioSource.clip = s.clip;
                    s.audioSource.clip.name = s.name;
                    s.audioSource.volume = s.volume;
                    s.audioSource.pitch = s.pitch;
                    s.audioSource.loop = s.loop;
                    s.audioSource.playOnAwake = false;    //Sons que tocam logo no início serão adicionados diretamente aos seus gameObjects
                    if (s.is3D) {
                        s.audioSource.spatialBlend = 1f;
                        s.audioSource.maxDistance = 25f;
                        s.audioSource.minDistance = 3f;
                        s.audioSource.rolloffMode = AudioRolloffMode.Linear;
                    }
                }
            }
        }
    }

    public void PlaySound(string soundName, GameObject go) {
        Sound s = Array.Find(sounds, sound => sound.name.Equals(soundName));   //Procurando o som informado pelo seu nome
        if (s != null) {
            if(go != null) {
                AudioSource[] audios = go.GetComponents<AudioSource>();
                audios.FirstOrDefault(a => a.clip.name.Equals(soundName)).Play();
            }
            else {
                if (soundName.Contains("OST"))   //Se eu estiver tentando mudar a música de fundo
                    SwapTrack(soundName);
                else {
                    AudioSource[] audios = gameObject.GetComponents<AudioSource>();
                    audios.FirstOrDefault(a => a.clip.name.Equals(soundName)).Play();
                    Debug.Log(soundName);
                }
            }
        }
    }

    public void StopSound(string soundName, GameObject go) {
        Sound s = Array.Find(sounds, sound => sound.name.Equals(soundName));   //Procurando o som informado pelo seu nome
        if (s != null) {
            if(go != null) {
                AudioSource[] audios = go.GetComponents<AudioSource>();
                audios.FirstOrDefault(a => a.clip.name.Equals(soundName)).Stop();
            }
            else {
                AudioSource[] audios = gameObject.GetComponents<AudioSource>();
                audios.FirstOrDefault(a => a.clip.name.Equals(soundName)).Stop();
            }
        }
    }


    private void SwapTrack(string nameNewOST) {
        string nameOSTPlaying = "";
        foreach(KeyValuePair<string, bool> valuePair in isPlayingOST) {
            if(valuePair.Value == true) {
                nameOSTPlaying = valuePair.Key;   //Pegando o nome da trilha que está tocando
                break;
            }
        }
        if(nameOSTPlaying != nameNewOST) {
            AudioSource[] audios = gameObject.GetComponents<AudioSource>();
            AudioSource newOST = audios.FirstOrDefault(a => a.clip.name.Equals(nameNewOST));
            AudioSource oldOST = null;
            if(nameOSTPlaying != "")
                oldOST = audios.FirstOrDefault(a => a.clip.name.Equals(nameOSTPlaying));

            StopAllCoroutines();
            StartCoroutine(FadeTrack(oldOST, newOST));
            isPlayingOST[nameOSTPlaying] = false;
            isPlayingOST[nameNewOST] = true;
        }
    }


    private IEnumerator FadeTrack(AudioSource oldOST, AudioSource newOST) {    //Esta co-rotina será usada para transiocionar entre uma música e outra
        float timeToFade = 1.25f, timeElapsed = 0;
        float volumeNewOst = volumeOSTs[newOST.clip.name], volumeOldOST=1;
        if (oldOST != null)
            volumeOldOST = volumeOSTs[oldOST.clip.name];
        
        newOST.Play();
        while (timeElapsed < timeToFade) {
            if(oldOST != null)
                oldOST.volume = Mathf.Lerp(volumeOldOST, 0, timeElapsed / timeToFade);
            newOST.volume = Mathf.Lerp(0, volumeNewOst, timeElapsed / timeToFade);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        if (oldOST != null)
            oldOST.Stop();
    }
}
