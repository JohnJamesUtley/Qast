using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blink : Spell {
	GameObject TeleEffect;
	public Blink(float SpellWait, GameObject Fizzle, float FizzlePoint, string Name, float Cooldown, GameObject TeleEffect) : base(SpellWait, Fizzle, FizzlePoint, Name, Cooldown) {
		this.TeleEffect = TeleEffect;
	}
	public override void CastEffect(){
		base.CastEffect ();
		Quaternion Direction = Quaternion.Euler(new Vector3(0,0,WizardObject.GetComponent<Wizard>().RotationTarget));
		Vector2 Position = WizardObject.transform.position + (Direction * Vector3.up) * 4;
		GameObject Effect1 = GameObject.Instantiate (TeleEffect);
		Effect1.transform.position = WizardObject.transform.position;
		Game.Teleport (WizardObject, Position);
		GameObject Effect2 = GameObject.Instantiate (TeleEffect);
		Effect2.transform.position = Position;
		Destroy ();
	}
	public override Spell Copy ()
	{
		Blink TheCopy = new Blink (SpellWait, Fizzle, FizzlePoint, Name, Cooldown, TeleEffect);
		return CopyBasics (TheCopy);
	}
}
