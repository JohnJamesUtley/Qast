using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slow : Spell {
    GameObject PreProj;
    GameObject Proj;
    public Slow(float SpellWait, GameObject Fizzle, float FizzlePoint, string Name, float Cooldown, GameObject Dummy) : base(SpellWait, Fizzle, FizzlePoint, Name, Cooldown) {
        PreProj = Dummy;
    }
    public override void CastEffect() {
        base.CastEffect();
        Proj = GameObject.Instantiate(PreProj);
        Quaternion Direction = Quaternion.Euler(new Vector3(0, 0, WizardObject.GetComponent<Wizard>().RotationTarget));
        Proj.transform.rotation = Direction;
        Proj.transform.position = WizardObject.transform.position + Direction * Vector3.up * 0.4f;
        Proj.GetComponent<Rigidbody2D>().AddForce(Proj.transform.up * 150f);
        Game.PlayAudio("StandardCast", 0.3f, 0.1f, 1f);
    }
    public override Spell Copy() {
        Slow TheCopy = new Slow(SpellWait, Fizzle, FizzlePoint, Name, Cooldown, PreProj);
        return CopyBasics(TheCopy);
    }
    public override void Destroy() {
        Proj.GetComponent<ProxyChar>().Destroy();
    }
}
