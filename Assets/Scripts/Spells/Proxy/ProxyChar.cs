using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProxyChar : ObjectChar {
	public GameObject BurnPrefab;
	public GameObject PuffEffect;
	public Wizard Caster;
    GameObject BurnEffect;
    SceneManager SceneManager;
	GameBehaviors Game;
	float TimeAlive = 0;
	bool Burning = false;
	float BurnTime = 0;
	void Start(){
		SceneManager = GameObject.Find ("SceneManager").GetComponent<SceneManager> ();
		Game = GameObject.Find ("SceneManager").GetComponent<GameBehaviors> ();
		SceneManager.Targets.Add (gameObject);
        Game.AddDisposable(gameObject);
    }
    void Update(){
		TimeAlive += Time.deltaTime;
		if (TimeAlive > 20f)
			Destroy ();
		if (Burning) {
			BurnTime += Time.fixedDeltaTime;
			if (BurnTime > 10f)
				Destroy ();
		}
	}
	void OnCollisionEnter2D(Collision2D Col){
		if (Burning) {
			GameObject Hit = Col.gameObject;
			if (Hit.tag == "Wizard" || Hit.tag == "Object")
				Game.Burn (Hit);
		}
	}
	public override void Burn (){
		if (!Burning) {
			Burning = true;
			BurnEffect = GameObject.Instantiate (BurnPrefab);
			BurnEffect.transform.rotation = transform.rotation;
			BurnEffect.transform.parent = transform;
			BurnEffect.transform.localScale = new Vector2 (0.8f, 0.8f);
			BurnEffect.transform.localPosition = new Vector2(0,0);
			ParticleSystem Parts = BurnEffect.GetComponent<ParticleSystem> ();
			ParticleSystem.MainModule PartsMain = Parts.main;
			ParticleSystem.EmissionModule PartsEmis = Parts.emission;
			PartsMain.loop = true;
			PartsEmis.rateOverTime = new ParticleSystem.MinMaxCurve (15);
		}
		SpawnPuff ();
	}
    public override void ClearBurn() {
        Burning = false;
        BurnTime = 0;
        GameObject.Destroy(BurnEffect);
    }
    public override void Damage (){
		SpawnPuff ();
	}
	public override void Destroy (){
		SpawnPuff ();
		GameObject.Destroy (gameObject);
	}
	void SpawnPuff(){
		GameObject Puff = GameObject.Instantiate (PuffEffect);
		Puff.transform.position = transform.position;
        Game.PlayAudio("ProxyHit", 0.3f, 0.05f, 1f);
	}
	void OnDestroy(){
		SceneManager.Targets.Remove (gameObject);
		Caster.ExemptTargets.Remove (gameObject);
	}
}
