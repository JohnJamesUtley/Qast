using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MummyChar : ObjectChar
{
	SceneManager SceneManager;
	GameBehaviors Game;
	Rigidbody2D Body;
	public GameObject Dust;
	public GameObject BurnPrefab;
    public GameObject StunPrefab;
	GameObject BurnEffect;
	bool Burning = false;
	float BurnTime = 0;
	public float Speed;
	float RealSpeed;
	public GameObject Target;
	public float Force;
	bool Attacked = false;
	float AttackCooldown;
	public bool Spawning = true;
	public float TimeAlive = 0;
    bool Stunned = false;
    float TimeForStun = 0f;
    GameObject StunEffect;
	void Start(){
		SceneManager = GameObject.Find ("SceneManager").GetComponent<SceneManager> ();
		Game = GameObject.Find ("SceneManager").GetComponent<GameBehaviors> ();
		Body = gameObject.GetComponent<Rigidbody2D> ();
		SceneManager.Targets.Add (gameObject);
        Game.AddDisposable(gameObject);
    }
    void FixedUpdate(){
		if (!Spawning && !Stunned) {
			RealSpeed = Speed;
			if (Attacked) {
				RealSpeed *= 0.25f;
				AttackCooldown -= Time.fixedDeltaTime;
				if (AttackCooldown < 0)
					Attacked = false;
			}
			bool ValidTarget = false;
			float ClosestDistance = 1000000;
			Target = gameObject;
			List<GameObject> Targets = SceneManager.Targets;
            for (int i = 0; i < Targets.Count; i++) {
                if (Targets[i].GetComponent<MummyChar>() == null) {
                    if (!ValidTarget) {
                        ValidTarget = true;
                        ClosestDistance = Vector2.Distance(Targets[i].transform.position, transform.position);
                        Target = Targets[i];
                    } else if (ClosestDistance > Vector2.Distance(Targets[i].transform.position, transform.position)) {
                        ClosestDistance = Vector2.Distance(Targets[i].transform.position, transform.position);
                        Target = Targets[i];
                    }
                }
            }
			float RotationTarget = 0;
			if (ValidTarget) {
				RotationTarget = Mathf.Atan2 (Target.transform.position.y - gameObject.transform.position.y, Target.transform.position.x - gameObject.transform.position.x) * Mathf.Rad2Deg - 90;
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
			Body.AddForce (transform.up * RealSpeed);
		}
        TimeAlive += Time.fixedDeltaTime;
        if (TimeAlive > 22.5f) {
            Game.AddShake(0.02f, 0.25f);
            Destroy();
        }
        UpdateBurning();
        UpdateStun();
    }
	void OnCollisionEnter2D(Collision2D Col){
		GameObject Hit = Col.gameObject;
		if (Hit.Equals (Target) && !Attacked) {
            Game.PlayAudio("MummyHit", 0.7f, 0.05f, 1f);
			Attacked = true;
			AttackCooldown = 2.25f;
			Vector2 Dir = (Hit.transform.position - transform.position).normalized;
            Game.AddForce(Hit, Dir * Force);		
			if (Burning)
				Game.Burn (Hit);
			else
				Game.Stun (Hit, 3f, 2.5f);
		}
	}
	public override void Burn ()
	{
		if (!Burning) {
            BurnTime = 0;
            Burning = true;
			BurnEffect = GameObject.Instantiate (BurnPrefab);
			BurnEffect.transform.rotation = transform.rotation;
			BurnEffect.transform.parent = transform;
			BurnEffect.transform.localScale = new Vector2 (0.8f, 0.8f);
			BurnEffect.transform.localPosition = new Vector2(0,-0.05f);
			ParticleSystem Parts = BurnEffect.GetComponent<ParticleSystem> ();
			ParticleSystem.MainModule PartsMain = Parts.main;
			ParticleSystem.EmissionModule PartsEmis = Parts.emission;
			PartsMain.loop = true;
			PartsEmis.rateOverTime = new ParticleSystem.MinMaxCurve (15);
		}
	}
    public override void ClearBurn() {
        Burning = false;
        GameObject.Destroy(BurnEffect);
    }
    void UpdateBurning() {
        if (Burning) {
            BurnTime += Time.fixedDeltaTime;
            if (BurnTime > 10f)
                Destroy();
        }
    }
    public override void Stun(float StunTime) {
        if (!Stunned) {
            Stunned = true;
            TimeForStun = StunTime;
            StunEffect = GameObject.Instantiate(StunPrefab);
            StunEffect.transform.parent = transform;
            StunEffect.transform.localPosition = new Vector2(0, -0.05f);
        }
    }
    void UpdateStun() {
        if (Stunned) {
            TimeForStun -= Time.fixedDeltaTime;
            if (TimeForStun < 0f) {
                Stunned = false;
                GameObject.Destroy(StunEffect);
            }
        }
    }
    public override void Damage ()
	{
		Destroy ();
	}
	public override void Destroy ()
	{
		GameObject DeadDust = GameObject.Instantiate (Dust);
		DeadDust.transform.position = transform.position;
        Game.PlayAudio("ProxyHit", 0.3f, 0.1f, 1f);
		GameObject.Destroy (gameObject);
	}
	void OnDestroy(){
		SceneManager.Targets.Remove (gameObject);
	}
}
