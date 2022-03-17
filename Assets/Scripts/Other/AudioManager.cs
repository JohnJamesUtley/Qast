using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {
    public GameObject BasicSound;
    public SoundClip[] Sounds;
    public List<AudioSource> Players;
    public List<AudioCutTimer> AutoStop;
    // Start is called before the first frame update
    void Start() {
        AutoStop = new List<AudioCutTimer>();
        Players = new List<AudioSource>();
        for (int i = 0; i < 4; i++)
            Players.Add(GameObject.Instantiate(BasicSound).GetComponent<AudioSource>());
        foreach (AudioSource x in Players)
            x.gameObject.transform.parent = transform;

    }
    void Update() {
        List<AudioCutTimer> Remove = new List<AudioCutTimer>();
        foreach (AudioCutTimer x in AutoStop) {
            x.TimeLeft -= Time.deltaTime;
            if (x.TimeLeft < 0) {
                x.CooldownLeft -= Time.deltaTime;
                float Percent = x.CooldownLeft / x.Cooldown;
                x.Src.volume = Percent * x.Volume;
                if (x.CooldownLeft < 0) {
                    x.Src.Stop();
                    Remove.Add(x);
                }
            }
        }
        foreach (AudioCutTimer x in Remove) {
            AutoStop.Remove(x);
        }
    }
        public AudioSource PlayAudio(string Name, float Volume, float Pitch, float Time, float Cooldown) {
        AudioSource Src = PlayAudio(Name, Volume, Pitch);
        AutoStop.Add(new AudioCutTimer(Time, Cooldown, Volume, Src));
        return Src;
    }
    public AudioSource PlayAudio(string Name, float Volume, float Pitch) {
        AudioSource Src = Players[0];
        bool SrcSet = false;
        foreach (AudioSource x in Players) {
            if (!x.isPlaying) {
                SrcSet = true;
                Src = x;
                break;
            }
        }
        if (!SrcSet) {
            Players.Add(GameObject.Instantiate(BasicSound).GetComponent<AudioSource>());
            Src = Players[Players.Count - 1];
            Players[Players.Count - 1].gameObject.transform.parent = transform;
        }
        bool Found = false;
        foreach (SoundClip x in Sounds) {
            if (Name == x.Name) {
                Src.clip = x.Sound;
                Found = true;
                break;
            }
        }
        if(!Found)
            Debug.LogError("No Audio Clip: " + Name);
        Src.volume = Volume;
        Src.pitch = Pitch;
        Src.Play();
        return Src;
    }
    public void ClearSound() {
        foreach (AudioSource x in Players) {
            x.Stop();
        }
    }
}
