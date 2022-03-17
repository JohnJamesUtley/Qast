using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class BarricadeEvent : Event
{
    public BramblesChar[] Brams;
    bool Heal2nd;
    public override void EventAction() {
        base.EventAction();
        Heal2nd = false;
        foreach (BramblesChar x in Brams)
            x.Heal();
    }
    public override void Update() {
        base.Update();
        if (Timer > 1.25f && !Heal2nd) {
            foreach (BramblesChar x in Brams)
                x.Heal();
            Heal2nd = true;
            Closed = true;
        }
    }
}
