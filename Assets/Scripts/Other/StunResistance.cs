using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunResistance : Effect {
	float ResistanceTime;
	float TimeAlive = 0;
	public StunResistance(GameObject TWizard, GameBehaviors Game, float ResistanceTime) : base(TWizard, Game) {
		this.ResistanceTime = ResistanceTime;
	}
	public override void Initialize (){}
	public override void Run ()
	{
		TimeAlive += Time.deltaTime;
		if (TimeAlive > ResistanceTime)
			Clear();
	}
	public override void Clear ()
	{
		WizardScript.RemoveEffect (this);
	}
    public override string ToString() {
        return "StunResistance";
    }
}
