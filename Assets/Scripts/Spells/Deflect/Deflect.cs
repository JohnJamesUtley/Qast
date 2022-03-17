using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deflect : Spell {
	public GameObject PreDeflector;
	public float LifeTime = 0.4f;
	GameObject Deflector;
	public Deflect(float SpellWait, GameObject Fizzle, float FizzlePoint, string Name, float Cooldown, GameObject PreDeflector) : base(SpellWait, Fizzle, FizzlePoint, Name, Cooldown) {
		this.PreDeflector = PreDeflector;
	}
	public override void CastEffect(){
		base.CastEffect ();
		Deflector = GameObject.Instantiate (PreDeflector);
		Deflector.GetComponent <Deflector> ().Parent = this;
		Deflector.transform.position = WizardObject.transform.position + WizardObject.transform.up * 1;
		Deflector.transform.rotation = Quaternion.LookRotation (WizardObject.transform.forward, WizardObject.transform.up);
        Game.PlayAudio("DeflectCast", 0.6f, 0.05f, 1f);
	}
	public override Spell Copy ()
	{
		Deflect TheCopy = new Deflect (SpellWait, Fizzle, FizzlePoint, Name, Cooldown, PreDeflector);
		return CopyBasics (TheCopy);
	}
	public override void WhileActive ()
	{
		if(TimeActive >= LifeTime){
			Destroy ();
		}
		base.WhileActive ();
	}
	public override void OnNewCast ()
	{
		Destroy ();
		base.OnNewCast ();
	}
	public override void Destroy ()
	{
		GameObject.Destroy (Deflector);
		base.Destroy ();
	}
}
