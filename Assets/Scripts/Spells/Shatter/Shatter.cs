using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shatter : Spell {
    public GameObject PreBomb;
    GameObject Bomb;
    float Range = 3;
    float Force = 50;
	bool Exploded = false;
    public Shatter(float SpellWait, GameObject Fizzle, float FizzlePoint, string Name, float Cooldown, GameObject Bomb) : base(SpellWait, Fizzle, FizzlePoint, Name, Cooldown) {
        this.PreBomb = Bomb;
    }
    public override void CastEffect() {
        base.CastEffect();
        Bomb = GameObject.Instantiate(PreBomb);
        Bomb.GetComponent<ShatterProjectile>().Parent = this;
        Quaternion Direction = Quaternion.Euler(new Vector3(0, 0, WizardObject.GetComponent<Wizard>().RotationTarget));
        Bomb.transform.rotation = Direction;
        Bomb.transform.position = WizardObject.transform.position + Direction * Vector3.up;
        Game.AddDisposable(Bomb);
    }
    public override Spell Copy() {
        Shatter TheCopy = new Shatter(SpellWait, Fizzle, FizzlePoint, Name, Cooldown, PreBomb);
        return CopyBasics(TheCopy);
    }
    public override void Destroy() {
        Game.RemoveDisposable(Bomb);
        base.Destroy();
    }
    public override void WhileActive() {
        if (TimeActive >= 4.6f) {
            Game.StartCoroutine (Explode());
        }
        base.WhileActive();
    }
	public IEnumerator Explode() {
		if (!Exploded) {
            Exploded = true;
			yield return new WaitForSeconds(0.4f);
			Game.AddShake (0.2f, 0.5f);
			Bomb.GetComponent<ShatterProjectile> ().Explode ();
			Damage ();
			Destroy ();
		}
	}
    public void Damage() {
		List<GameObject> InRange = Game.GetMoveables (Bomb.transform.position, Range, Bomb);
        for (int i = 0; i < InRange.Count; i++) {
            if (!Game.CheckInterferance(InRange[i].transform.position, Bomb.transform.position, new string[] { "Static"})) {
                Game.Burn(InRange[i]);
                Game.AddForce(InRange[i], (InRange[i].transform.position - Bomb.transform.position).normalized * Force);
            }
        }
    }
}