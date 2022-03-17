using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lightning : Spell {
	public GameObject PreProjectile;
	public GameObject Ash;
	GameObject Project;
	AnimationCurve SpeedCurve;
	AnimationCurve AccuracyCurve;
	public Lightning(float SpellWait, GameObject Fizzle, float FizzlePoint, string Name, float Cooldown, GameObject Projectile, GameObject Ash, AnimationCurve SpeedCurve, AnimationCurve AccuracyCurve) : base(SpellWait, Fizzle, FizzlePoint, Name, Cooldown) {
		this.PreProjectile = Projectile;
		this.Ash = Ash;
		this.SpeedCurve = SpeedCurve;
		this.AccuracyCurve = AccuracyCurve;
	}
	public override void CastEffect(){
		base.CastEffect ();
        Game.PlayAudio("LightningCast", 0.3f, 0.15f, 1f);
        Project = GameObject.Instantiate (PreProjectile);
		Project.GetComponent<LightningProjectile> ().Parent = this;
		Project.GetComponent<LightningProjectile> ().Speed = SpeedCurve.Evaluate (Accuracy) * 13;
		Quaternion Direction = Quaternion.Euler(new Vector3(0,0,WizardObject.GetComponent<Wizard>().RotationTarget));
		Project.transform.position = WizardObject.transform.position + Direction * Vector3.up;
		Project.transform.rotation = Direction;
		float Mult = 0;
		if (Random.value > 0.5f) {
			Mult = 1;
		} else {
			Mult = -1;
		}
		Vector3 Euler = Project.transform.rotation.eulerAngles + new Vector3(0,0,30 * AccuracyCurve.Evaluate (Speed / 2) * Mult * Random.value);
		Project.transform.eulerAngles = new Vector3(Euler.x,Euler.y,Euler.z);
	}
	public override Spell Copy ()
	{
		Lightning TheCopy = new Lightning (SpellWait, Fizzle, FizzlePoint, Name, Cooldown, PreProjectile, Ash, SpeedCurve, AccuracyCurve);
		return CopyBasics (TheCopy);
	}
	public override void WhileActive ()
	{
		if(TimeActive >= 3){
			Destroy ();
		}
		base.WhileActive ();
	}
	public void StaticContact(){
		Destroy ();
	}
	public void WizardContact(GameObject HitWizard){
		Destroy ();
		GameObject Remains = GameObject.Instantiate (Ash);
		Remains.transform.position = HitWizard.transform.position;
		HitWizard.GetComponent<Wizard> ().Kill ();
	}
    public void ObjectContact(GameObject HitObject) {
        Destroy();
		HitObject.GetComponent<ObjectChar>().Damage();
    }
    public override void Destroy ()
	{
        Game.PlayAudio("LightningDestroy", 0.4f, 0.1f, 1f);
        Project.GetComponent<LightningProjectile> ().Scatter();
		base.Destroy ();
	}
}
