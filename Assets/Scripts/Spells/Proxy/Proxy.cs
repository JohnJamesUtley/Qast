using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Proxy : Spell {
	GameObject PreDummy;
	GameObject Dummy;
	public Proxy(float SpellWait, GameObject Fizzle, float FizzlePoint, string Name, float Cooldown, GameObject Dummy) : base(SpellWait, Fizzle, FizzlePoint, Name, Cooldown) {
		PreDummy = Dummy;
	}
	public override void CastEffect(){
		base.CastEffect ();
		Dummy = GameObject.Instantiate (PreDummy);
		Quaternion Direction = Quaternion.Euler(new Vector3(0,0,WizardObject.GetComponent<Wizard>().RotationTarget));
		Dummy.transform.rotation = Direction;
		Dummy.transform.position = WizardObject.transform.position + Direction * Vector3.up;
		Dummy.GetComponent<Rigidbody2D> ().AddForce (Dummy.transform.up * 25f);
		Dummy.GetComponent<ProxyChar> ().Caster = WizardScript;
		WizardScript.ExemptTargets.Add (Dummy);
        Game.PlayAudio("ProxyHit", 0.3f, 0.05f, 1f);
    }
    public override Spell Copy ()
	{
		Proxy TheCopy = new Proxy (SpellWait, Fizzle, FizzlePoint, Name, Cooldown, PreDummy);
		return CopyBasics (TheCopy);
	}
	public override void Destroy (){
		Dummy.GetComponent<ProxyChar> ().Destroy ();
	}
}
