using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Push : Spell {
    GameObject Effect;
    float Range = 5;
    float EffectiveAngle = 35;
    float Force = 200;
    public Push(float SpellWait, GameObject Fizzle, float FizzlePoint, string Name, float Cooldown, GameObject Effect) : base(SpellWait, Fizzle, FizzlePoint, Name, Cooldown) {
        this.Effect = Effect;
    }
    public override void CastEffect() {
        base.CastEffect();
        Game.PlayAudio("PushCast", 1f, 0.05f, 1f);
        GameObject TheEffect = GameObject.Instantiate(Effect);
        TheEffect.transform.position = WizardObject.transform.position;
        TheEffect.transform.rotation = WizardObject.transform.rotation;
		List<GameObject> InRange = Game.GetMoveables (WizardObject.transform.position, Range, WizardObject);
        float WizAngle = WizardObject.transform.rotation.eulerAngles.z + 270;
        if (WizAngle > 360)
            WizAngle -= 360;
        for (int i = 0; i < InRange.Count; i++) {
            if (!Game.CheckInterferance(InRange[i].transform.position, WizardObject.transform.position, new string[] {"Static"})) {
                Vector3 dir = InRange[i].transform.position - WizardObject.transform.position;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 180;
                float AngleDif = Mathf.Abs(WizAngle - angle);
                if (AngleDif < EffectiveAngle / 2) {
                    Game.AddForce(InRange[i], dir.normalized * Force);
                }
            }
        }
        Destroy();
    }
    public override Spell Copy() {
        Push TheCopy = new Push(SpellWait, Fizzle, FizzlePoint, Name, Cooldown, Effect);
        return CopyBasics(TheCopy);
    }
}
