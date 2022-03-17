using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioCutTimer
{
    public float TimeLeft;
    public AudioSource Src;
    public float Cooldown;
    public float CooldownLeft;
    public float Volume;
    public AudioCutTimer(float TimeLeft, float Cooldown, float Volume, AudioSource Src) {
        this.TimeLeft = TimeLeft;
        this.Cooldown = Cooldown;
        CooldownLeft = Cooldown;
        this.Volume = Volume;
        this.Src = Src;
    }
}
