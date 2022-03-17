using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloak : Spell {
	SpriteRenderer WizardSprite;
	bool Appearing;
	public Cloak(float SpellWait, GameObject Fizzle, float FizzlePoint, string Name, float Cooldown) : base(SpellWait, Fizzle, FizzlePoint, Name, Cooldown) {
		Appearing = false;
	}
	public override void CastEffect(){
		base.CastEffect ();
		WizardSprite = WizardObject.transform.GetChild (0).GetComponent<SpriteRenderer> ();
		WizardScript.StartCoroutine (Disappear());
        Game.PlayAudio("CloakCast", 0.2f, 0.05f, 1f);
	}
	public override Spell Copy ()
	{
		Cloak TheCopy = new Cloak (SpellWait, Fizzle, FizzlePoint, Name, Cooldown);
		return CopyBasics (TheCopy);
	}
	public override void Destroy ()
	{
		WizardSprite.color = new Color(1,1,1,1);
		base.Destroy ();
	}
	public override void WhileActive ()
	{
		if(TimeActive >= 4 && !WizardDead){
			WizardScript.StartCoroutine (Appear());
		}
		base.WhileActive ();
	}
	public override void OnNewCast ()
	{
		WizardScript.StartCoroutine (Appear ());
		base.OnNewCast ();
	}
	IEnumerator Disappear(){
		Game.RemoveTarget (WizardObject);
		while (WizardSprite.color.a > 0) {
			WizardSprite.color = new Color(1,1,1, WizardSprite.color.a - 0.05f);
			yield return 0;
		}
	}
	IEnumerator Appear(){
		if(!Appearing){
			Game.AddTarget (WizardObject);
			Appearing = true;
            Game.PlayAudio("CloakCast", 0.2f, 0.05f, 1f);
            while (WizardSprite.color.a < 1) {
				WizardSprite.color = new Color(1,1,1, WizardSprite.color.a + 0.05f);
				yield return 0;
			}
            Destroy();
		}
	}
    public override void OnDeath() {
        base.OnDeath();
        Destroy();
    }
}
