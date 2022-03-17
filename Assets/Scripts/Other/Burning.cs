using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Burning : Effect {
	float TimeToKill;
	bool Killed;
	public GameObject Ash;
    public GameObject BurnEffect;
	GameObject PreEffect;
    List<GameObject> AttemptedBurns;
    List<float> CoolDowns;
    public Burning(GameObject TWizard, GameBehaviors Game,GameObject PreBurning, GameObject Ash) : base(TWizard, Game){
		this.Ash = Ash;
		PreEffect = PreBurning;
    }
	public override void Initialize ()
	{
		Killed = false;
		TimeToKill = 5;
		BurnEffect = GameObject.Instantiate(PreEffect);
		BurnEffect.transform.position = Wizard.transform.position;
		BurnEffect.transform.rotation = Wizard.transform.rotation;
		BurnEffect.transform.parent = Wizard.transform;
	}
	public override void Run(){
		TimeToKill -= Time.deltaTime;
		if(TimeToKill < 0 && Killed == false){
			GameObject Remains = GameObject.Instantiate (Ash);
			Remains.transform.position = BurnEffect.transform.position;
            Transform Wiz = BurnEffect.transform.parent;
            BurnEffect.transform.parent = null;
            Wiz.gameObject.GetComponent<Wizard> ().Kill ();
			Killed = true;
		}
		if(TimeToKill < -3){
			Clear ();
		}
        //SetNewFires();
	}
    void SetNewFires() {
        float Range = 0.5f;
		List<GameObject> InRange = Game.GetMoveables (BurnEffect.transform.position,Range,BurnEffect);
        for (int i = 0; i < InRange.Count; i++) {
            bool NotAttempted = true;
            foreach (GameObject x in AttemptedBurns)
                if (x.Equals(InRange[i]))
                    NotAttempted = false;
            if (NotAttempted) {
                if (Game.GetRandom() > 0.5f)
                    Game.Burn(InRange[i]);
                AttemptedBurns.Add(InRange[i]);
                CoolDowns.Add(1f);
            }
        }
        List<GameObject> ToRemove = new List<GameObject>();
        for (int i = 0; i < AttemptedBurns.Count; i++) {
            CoolDowns[i] -= Time.deltaTime;
            if (CoolDowns[i] < 0) {
                ToRemove.Add(AttemptedBurns[i]);
                CoolDowns.RemoveAt(i);

            }
        }
        foreach(GameObject x in ToRemove) {
            AttemptedBurns.Remove(x);
        }
    }
    public override void Clear() {
		WizardScript.RemoveEffect (this);
		if (BurnEffect != null)
			GameObject.Destroy (BurnEffect);
    }
    public override string ToString() {
        return "Burning";
    }
}
