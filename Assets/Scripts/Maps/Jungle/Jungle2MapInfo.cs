using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jungle2MapInfo : MapInfo {
    public BrazierChar[] Braziers;
    public BrazierChar[] Open;
    public SwitchDoorsEvent Switch;
    public RandomizeDoorsEvent Rand;
    public override void OnStart() {
        StartCoroutine("StartSwitchedDoors");
    }
    public IEnumerator StartSwitchedDoors() {
        yield return new WaitForSeconds(1f);
        foreach (BrazierChar x in Open)
            x.Burn();
    }
    public override void AddEvents() {
        Switch.Braziers = Braziers;
        Rand.Braziers = Braziers;
        Events.Add(Switch);
        Events.Add(Rand);
    }
}
