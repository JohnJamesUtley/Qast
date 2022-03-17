using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Spell {
	public float Accuracy;
	public float FizzlePoint;
	public float Speed;
	public float TimeActive;
	public float SpellWait;
    public float Cooldown;
	public string Name;
	public GameObject Fizzle;
	public GameObject WizardObject;
	public Wizard WizardScript;
	public GameBehaviors Game;
    public bool WizardDead;
	public Spell(float SpellWait, GameObject Fizzle, float FizzlePoint, string Name, float Cooldown){
		this.FizzlePoint = FizzlePoint;
		this.Name = Name;
		this.Fizzle = Fizzle;
		this.SpellWait = SpellWait;
        this.Cooldown = Cooldown;
		TimeActive = 0;
        WizardDead = false;
	}
	public void SetInstance(float Accuracy, float Speed){
		this.Accuracy = Accuracy;
		this.Speed = Speed;
	}
	public void Cast(GameObject WizardT){
		Debug.Log (this);
		WizardObject = WizardT;
		WizardScript = WizardT.GetComponent<Wizard>();
		WizardScript.NewCastEffects ();
		Game = GameObject.Find ("SceneManager").GetComponent<GameBehaviors> ();
		if (Accuracy > FizzlePoint) {
			CastEffect ();
            WizardScript.WireScript.SubtractInventory(Name);
		} else {
			FizzleEffect ();
		}
	}
	public virtual Spell Copy (){
		return null;
	}
	public Spell CopyBasics(Spell TheCopy){
		TheCopy.Accuracy = this.Accuracy;
		TheCopy.Speed = this.Speed;
		TheCopy.Fizzle = this.Fizzle;
        TheCopy.Cooldown = this.Cooldown;
		return TheCopy;
	}
	public virtual void WhileActive(){
		TimeActive += Time.fixedDeltaTime;
    }
	public virtual void OnNewCast(){
		return;
	}
	public virtual void Destroy(){
        if (WizardScript != null)
            WizardScript.ActiveSpells.Remove(this);
        else
            Game.KillLegacySpell(this);

	}

    public virtual void CastEffect(){
		WizardScript.ActiveSpells.Add (this);
		PlayerWire Wire = WizardScript.Wire.GetComponent<PlayerWire> ();
		Wire.SpellWait += SpellWait;
		Wire.IsWaiting = true;
		Wire.RpcSetWait (true);
        Wire.CreateCooldown(Name, Cooldown);
	}
	public virtual void FizzleEffect(){
		GameObject TheFizzle = GameObject.Instantiate (Fizzle);
		TheFizzle.transform.position = WizardObject.transform.position + WizardObject.transform.up * 0.75f;
		TheFizzle.transform.rotation = Quaternion.LookRotation (WizardObject.transform.forward, WizardObject.transform.up);
		Debug.Log ("Spell Fizzled");
	}
    public virtual void OnDeath(){
        WizardDead = true;
    }
	public override string ToString ()
	{
		return Name + ": Acr%" + Accuracy * 100 + " Fiz%" + FizzlePoint * 100 + " Spd-" + Speed;
	}
}
