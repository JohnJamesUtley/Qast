using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class RandomizeDoorsEvent : Event
{
    public BrazierChar[] Braziers; 
    public override void EventAction() {
        base.EventAction();
        foreach (BrazierChar x in Braziers)
            if (Random.value > 0.5f)
                x.Burn();
            else
                x.ClearBurn();
        Game.AddShake(0.05f, 1f);
        Closed = true;
    }
}
