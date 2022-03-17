using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wizard : MonoBehaviour
{
	SceneManager SceneManager;
	SpellManager SpellManager;
    GameBehaviors Game;
	public Animator WandLight;
	Rigidbody2D Body;
	public Vector2 MovementDirection;
	GameObject Target;
	public float Speed;
	public float RotationTarget;
    public bool Casting;
	public GameObject Wire;
    public PlayerWire WireScript;
	public List<Spell> ActiveSpells = new List<Spell> ();
    public List<Effect> ActiveEffects = new List<Effect>();
	public List<Effect> AddEffects = new List<Effect> ();
	public List<Effect> RemoveEffects = new List<Effect> ();
	public float ForceMultiplier;
	public List<GameObject> ExemptTargets;
	//Outside Mods
	public bool NoMoving = false;
	public bool NoCasting = false;
	void Start ()
	{
        LastStep = transform.position;
        Body = gameObject.GetComponent<Rigidbody2D> ();
		WandLight = transform.GetChild (1).GetComponent<Animator> ();
		SpellManager = GameObject.Find ("SpellManager").GetComponent<SpellManager> ();
		SceneManager = GameObject.Find ("SceneManager").GetComponent<SceneManager> ();
        Game = GameObject.Find("SceneManager").GetComponent<GameBehaviors>();
        WireScript = Wire.GetComponent<PlayerWire>();
		ExemptTargets = new List<GameObject> ();
        ExemptTargets.Add(gameObject);
    }
    void Update() {
		RunEffects ();
    }
	void RunEffects(){
		foreach (Effect x in AddEffects) {
			x.Initialize ();
			ActiveEffects.Add (x);
		}
		AddEffects = new List<Effect> ();
		foreach (Effect x in ActiveEffects) {
			x.Run ();
		}
		foreach (Effect x in RemoveEffects)
			ActiveEffects.Remove (x);
		RemoveEffects = new List<Effect> ();
	}
    public void AddEffect(Effect ToAdd) {
		bool AlreadyHave = false;
		foreach (Effect x in AddEffects)
			if (x.GetType ().Equals (ToAdd.GetType()))
				AlreadyHave = true;
		foreach (Effect x in ActiveEffects)
			if (x.GetType ().Equals (ToAdd.GetType()))
				AlreadyHave = true;
		if(!AlreadyHave){
			AddEffects.Add(ToAdd);
		}
    }
	public void RemoveEffect(Effect ToRemove){
		RemoveEffects.Add (ToRemove);
	}
	public void NewCastEffects ()
	{
		List<Spell> ToActivate = new List<Spell> ();
		foreach (Spell x in ActiveSpells) {
			ToActivate.Add (x);
		}
		foreach (Spell x in ToActivate) {
			x.OnNewCast ();
		}
	}

	public void ActiveSpellEffects ()
	{
		List<Spell> ToActivate = new List<Spell> ();
		foreach (Spell x in ActiveSpells) {
			ToActivate.Add (x);
		}
		foreach (Spell x in ToActivate) {
			x.WhileActive ();
		}
	}
	public void CastSpell(Spell ToCast){
		if (!NoCasting) {
			ToCast.Cast (gameObject);
		}
	}
	void FindTarget ()
	{
		List<GameObject> AvaliableTargets = new List<GameObject> ();
		for (int i = 0; i < SceneManager.Targets.Count; i++) {
			bool Avaliable = true;
			for (int j = 0; j < ExemptTargets.Count; j++)
				if(SceneManager.Targets[i].Equals(ExemptTargets[j])){
					Avaliable = false;
				}
			if(Avaliable)
				AvaliableTargets.Add (SceneManager.Targets [i]);
		}
		if (AvaliableTargets.Count != 0) {
			Target = AvaliableTargets [0];
			float ClosestDistance = Vector2.Distance (gameObject.transform.position, Target.transform.position);
			for (int i = 0; i < AvaliableTargets.Count; i++) {
				if (Vector2.Distance (gameObject.transform.position, AvaliableTargets [i].transform.position) < ClosestDistance) {
					Target = AvaliableTargets [i];
					ClosestDistance = Vector2.Distance (gameObject.transform.position, Target.transform.position);
			    }
			}
		}
	}

	public void Move ()
	{
		if (!NoMoving) {
			if (Speed < 0.95f && Speed > 0.2f) {
				Body.AddForce (MovementDirection * 2.25f * ForceMultiplier, ForceMode2D.Force);
				if (Target == null)
					FindTarget ();
				if (Target != null) {
					RotationTarget = Mathf.Atan2 (Target.transform.position.y - gameObject.transform.position.y, Target.transform.position.x - gameObject.transform.position.x) * Mathf.Rad2Deg - 90;
				} else {
					RotationTarget = Mathf.Atan2 (MovementDirection.y, MovementDirection.x) * Mathf.Rad2Deg - 90;
				}
			} else if (Speed > 0.2f) {
				if (!Casting)
					Body.AddForce (MovementDirection * 3f * ForceMultiplier, ForceMode2D.Force);
				if (Target != null)
					Target = null;
				RotationTarget = Mathf.Atan2 (MovementDirection.y, MovementDirection.x) * Mathf.Rad2Deg - 90;
			} else {
				if (Target != null)
					Target = null;
			}
			if (RotationTarget < 0) {
				RotationTarget += 360;
			}
			float OldRotation = Body.rotation;
			if (OldRotation < 0) {
				OldRotation += 360;
			}
			float Difference = RotationTarget - OldRotation;
			if (Mathf.Abs (Difference) > 180) {
				if (RotationTarget < OldRotation) {
					Difference += 360;
				} else {
					Difference -= 360;
				}
			}
			Body.rotation += Difference * Time.fixedDeltaTime * 15;
		}
        PlayStepSounds();
	}
    Vector2 LastStep;
    public void PlayStepSounds() {
        if (Vector2.Distance(transform.position, LastStep) > 0.85f) {
            Game.PlayAudio("Step", 0.2f, 0.1f, 0.9f);
            LastStep = transform.position;
        }
    }
	public void Kill ()
	{
		Debug.Log ("Wizard Killed");
        List<Spell> ToOnDeath = new List<Spell>();
        foreach (Spell x in ActiveSpells) {
            SceneManager.LegacySpells.Add(x);
            ToOnDeath.Add(x);
        }
        foreach (Spell x in ToOnDeath)
            x.OnDeath();
        WireScript.IsDead = true;
        WireScript.RpcSetDead (true);
		SceneManager.EliminateWizard (gameObject);
		GameObject.Destroy (gameObject);
	}
}
