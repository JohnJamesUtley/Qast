using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Desert3MapInfo : MapInfo
{
    public SandstormEvent Sand;
    public BarricadeEvent Barrier;
    public override void OnStart() {
        foreach (ParticleSystem x in Particles) {
            x.Simulate(5);
            x.Play();
        }
    }
    public override void AddEvents() {
        Events.Add(Sand);
        Events.Add(Barrier);
    }
}
