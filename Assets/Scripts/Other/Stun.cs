using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stun : Effect {
	GameObject PreStunEffect;
	GameObject StunEffect;
	float TimeActive = 0;
	float StunTime;
	float ResistTime;
	public Stun(GameObject TWizard, GameBehaviors Game,GameObject PreStunEffect, float StunTime, float ResistTime) : base(TWizard, Game) {
		this.PreStunEffect = PreStunEffect;
		this.StunTime = StunTime;
		this.ResistTime = ResistTime;
	}
	public override void Initialize () {
		bool Resistance = false;
		foreach (Effect x in WizardScript.ActiveEffects)
			if (x.GetType ().Equals ((System.Type.GetType("StunResistance"))))
				Resistance = true;
		if (Resistance) {
			Clear ();
			return;
		}
		StunEffect = GameObject.Instantiate (PreStunEffect);
		StunEffect.transform.parent = Wizard.transform;
		StunEffect.transform.localPosition = new Vector2 (0, 0);
		WizardScript.NoMoving = true;
		WizardScript.NoCasting = true;
	}
	public override void Run(){
		TimeActive += Time.deltaTime;
		if (TimeActive > StunTime)
			Clear ();
	}
	public override void Clear() {
		Game.StunResistance (Wizard, ResistTime);
		if(StunEffect != null)
			GameObject.Destroy (StunEffect);
		WizardScript.RemoveEffect (this);
		WizardScript.NoMoving = false;
		WizardScript.NoCasting = false;
	}
    public override string ToString() {
        return "Stun";
    }
}
