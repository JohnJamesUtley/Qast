using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knock : Spell {
    GameObject PreKnock;
    GameObject Rocket;
    public Knock(float SpellWait, GameObject Fizzle, float FizzlePoint, string Name, float Cooldown, GameObject PreKnock) : base(SpellWait, Fizzle, FizzlePoint, Name, Cooldown) {
        this.PreKnock = PreKnock;
    }
    public override void CastEffect() {
        base.CastEffect();
        Rocket = GameObject.Instantiate(PreKnock);
        Quaternion Direction = Quaternion.Euler(new Vector3(0, 0, WizardObject.GetComponent<Wizard>().RotationTarget));
        Rocket.transform.rotation = Direction;
        Rocket.transform.position = WizardObject.transform.position + Direction * Vector3.up;
        Rocket.GetComponent<KnockChar>().Caster = WizardObject;
        WizardScript.ExemptTargets.Add(Rocket);
        Game.PlayAudio("StandardCast", 0.4f, 0.1f, 1f);
    }
    public override Spell Copy() {
        Knock TheCopy = new Knock(SpellWait, Fizzle, FizzlePoint, Name, Cooldown, PreKnock);
        return CopyBasics(TheCopy);
    }
    public override void Destroy() {
        Rocket.GetComponent<KnockChar>().Destroy();
    }
}
