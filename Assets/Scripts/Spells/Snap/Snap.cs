using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snap : Spell {
	public GameObject PreProjectile;
	GameObject Project;
    GameObject TeleEffect;
    float TimeTillTrail;
    float Intervals;
	public Snap(float SpellWait, GameObject Fizzle, float FizzlePoint, string Name, float Cooldown, GameObject Projectile, GameObject TeleEffect) : base(SpellWait, Fizzle, FizzlePoint, Name, Cooldown) {
		this.PreProjectile = Projectile;
        this.TeleEffect = TeleEffect;
        Intervals = 0.1f;
	}
	public override void CastEffect(){
		base.CastEffect ();
		Project = GameObject.Instantiate (PreProjectile);
		Quaternion Direction = Quaternion.Euler(new Vector3(0,0,WizardObject.GetComponent<Wizard>().RotationTarget));
		Project.transform.rotation = Direction;
		Project.transform.position = WizardObject.transform.position + Direction * Vector3.up * 0.6f;
		Project.GetComponent<SnapProjectile> ().Parent = this;
        TimeTillTrail = 0.25f;
        Game.PlayAudio("SnapCast", 0.3f, 0.05f, 1f);
    }
	public override Spell Copy ()
	{
		Snap TheCopy = new Snap (SpellWait, Fizzle, FizzlePoint, Name, Cooldown, PreProjectile, TeleEffect);
		return CopyBasics (TheCopy);
	}
	public override void Destroy ()
	{
		Project.GetComponent<SnapProjectile> ().Break ();
		base.Destroy();
	}
	public override void WhileActive ()
	{
        TimeTillTrail -= Time.fixedDeltaTime;
        if(TimeTillTrail <= 0){
            if(Project != null)
                Project.GetComponent<SnapProjectile>().Trail();
            TimeTillTrail = Intervals;
        }
		if(TimeActive >= 4){
			Destroy ();
		}
		base.WhileActive ();
	}
	public override void OnNewCast ()
	{
		base.OnNewCast ();
	}
	public void StaticContact(Collision2D Col){
		//Destroy ();
	}
	public void ObjectContact(GameObject Hit){
		//Quaternion Direction = Quaternion.Euler(new Vector3(0,0,Random.value * 360));
        Quaternion Direction = Quaternion.LookRotation(Vector3.forward, Project.GetComponent<Rigidbody2D>().velocity).normalized;
        Vector3 NewPosition = Hit.transform.position + Direction * Vector2.up * 3f;
        GameObject Effect1 = GameObject.Instantiate(TeleEffect);
        Effect1.transform.position = Hit.transform.position;
        GameObject Effect2 = GameObject.Instantiate(TeleEffect);
        Effect2.transform.position = NewPosition;
        Game.Teleport (Hit,  NewPosition);
        Game.Stun(Hit, 3f, 1f);
		Destroy ();
	}
}
