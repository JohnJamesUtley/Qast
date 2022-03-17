using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShatterChar : ObjectChar {
	Shatter Spell;
	void Start(){
		Spell = gameObject.GetComponent<ShatterProjectile> ().Parent;
    }
    public override void Burn() {
        Destroy();
    }
	public override void Damage ()
	{
		Destroy ();
	}
    public override void Destroy() {
		Spell.WizardScript.StartCoroutine(Spell.Explode());
    }
}
