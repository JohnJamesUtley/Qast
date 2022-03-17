using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blast : Spell {
	public GameObject PreProjectile;
	GameObject Project;
	public Blast(float SpellWait, GameObject Fizzle, float FizzlePoint, string Name, float Cooldown, GameObject Projectile) : base(SpellWait, Fizzle, FizzlePoint, Name, Cooldown) {
		this.PreProjectile = Projectile;
	}
	public override void CastEffect(){
		base.CastEffect ();
		Project = GameObject.Instantiate (PreProjectile);
		Project.GetComponent<BlastProjectile> ().Parent = this;
		Quaternion Direction = Quaternion.Euler(new Vector3(0,0,WizardObject.GetComponent<Wizard>().RotationTarget));
		Project.transform.rotation = Direction;
		Project.transform.position = WizardObject.transform.position + Direction * Vector3.up;
        Game.PlayAudio("BlastCast", 0.4f, 0.15f, 1f);
    }
    public override Spell Copy ()
	{
		Blast TheCopy = new Blast (SpellWait, Fizzle, FizzlePoint, Name, Cooldown, PreProjectile);
		return CopyBasics (TheCopy);
	}
	public override void Destroy ()
	{
		Project.GetComponent<BlastProjectile>().Explode ();
        Game.PlayAudio("BlastHit", 0.4f, 0.05f, 1f);
		base.Destroy ();
	}
	public void Destroy (Vector2 Scorch)
	{
		Project.GetComponent<BlastProjectile>().Explode (Scorch);
        Game.PlayAudio("BlastHit", 0.4f, 0.05f, 1f);
        base.Destroy ();
	}
	public override void WhileActive ()
	{
		if(TimeActive >= 3){
			Destroy ();
		}
		base.WhileActive ();
	}
	public void StaticContact(Vector2 Scorch){
		Destroy (Scorch);
	}
	public void WizardContact(GameObject HitWizard){
		Game.Burn (HitWizard);
		Destroy ();
	}
    public void ObjectContact(GameObject HitObject) {
        Game.Burn(HitObject);
        Destroy();
    }
}
