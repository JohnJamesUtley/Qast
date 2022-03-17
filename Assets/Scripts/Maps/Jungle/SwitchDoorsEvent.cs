using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class SwitchDoorsEvent : Event
{
    public BrazierChar[] Braziers;
    public override void EventAction() {
        base.EventAction();
        foreach (BrazierChar x in Braziers) {
            bool Burnt = x.Burning;
            if (!Burnt)
                x.Burn();
            else
                x.ClearBurn();
        }
        Game.AddShake(0.05f, 1f);
        Closed = true;
    }
}
