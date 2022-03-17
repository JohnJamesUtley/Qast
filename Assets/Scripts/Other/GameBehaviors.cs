using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBehaviors : MonoBehaviour
{
	public GameObject Burning;
	public GameObject StunnedEffect;
    public GameObject Ash;
	public GameObject Engraving;
    public GameObject ScorchMark;
    SceneManager SceneManager;
	SpellManager SpellManager;
    AudioManager AudioManager;
	CameraShake Shaker;
	void Start(){
		SceneManager = gameObject.GetComponent<SceneManager> ();
		SpellManager = gameObject.GetComponent<SpellManager> ();
        AudioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        Shaker = gameObject.GetComponent<CameraShake> ();
	}
	public List<GameObject> GetMoveables(){
		GameObject[] Objects = GameObject.FindGameObjectsWithTag("Object");
		GameObject[] Wizards = GameObject.FindGameObjectsWithTag("Wizard");
		List<GameObject> InRange = new List<GameObject>();
		for(int i = 0; i < Objects.Length; i++) {
			InRange.Add(Objects[i]);
		}
		for (int i = 0; i < Wizards.Length; i++) {
			InRange.Add(Wizards[i]);
		}
		return InRange;
	}
	public List<GameObject> GetMoveables(Vector2 Pos, float Range){
		GameObject[] Objects = GameObject.FindGameObjectsWithTag("Object");
		GameObject[] Wizards = GameObject.FindGameObjectsWithTag("Wizard");
		List<GameObject> InRange = new List<GameObject>();
		for(int i = 0; i < Objects.Length; i++) {
			if(Vector2.Distance(Objects[i].transform.position, Pos) <= Range) {
				InRange.Add(Objects[i]);
			}
		}
		for (int i = 0; i < Wizards.Length; i++) {
			if (Vector2.Distance(Wizards[i].transform.position, Pos) <= Range) {
				InRange.Add(Wizards[i]);
			}
		}
		return InRange;
	}
	public List<GameObject> GetMoveables(Vector2 Pos, float Range, GameObject Exempt){
		GameObject[] Objects = GameObject.FindGameObjectsWithTag("Object");
		GameObject[] Wizards = GameObject.FindGameObjectsWithTag("Wizard");
		List<GameObject> InRange = new List<GameObject>();
		for(int i = 0; i < Objects.Length; i++) {
			if(Vector2.Distance(Objects[i].transform.position, Pos) <= Range && !Exempt.Equals(Objects[i])) {
				InRange.Add(Objects[i]);
			}
		}
		for (int i = 0; i < Wizards.Length; i++) {
			if (Vector2.Distance(Wizards[i].transform.position, Pos) <= Range && !Exempt.Equals(Wizards[i])) {
				InRange.Add(Wizards[i]);
			}
		}
		return InRange;
	}
    /// <summary>
    /// Checks whether there are any physical objects between two points with the given tags.
    /// </summary>
    /// <param name="PositionOne">The First position.</param>
    /// <param name="PositionTwo">The Second Position.</param>
    /// <param name="Tags">Tags which the function checks for.</param>
    /// <returns>Returns true if there was an interfering object.</returns>
    public bool CheckInterferance(Vector2 PositionOne, Vector2 PositionTwo, string[] Tags) {
       RaycastHit2D[] Objects = Physics2D.RaycastAll(PositionOne, PositionTwo - PositionOne, Vector2.Distance(PositionOne,PositionTwo));
        for(int i = 0; i < Objects.Length; i++) {
            for (int j = 0; j < Tags.Length; j++) {
                if (Tags[j].Equals(Objects[i].transform.gameObject.tag)) {
                    return true;
                }
            }
        }
        return false;
    }
	public void Teleport(GameObject Teleported, Vector2 Position){
		Collider2D[] Hit = Physics2D.OverlapPointAll(Position);
        PlayAudio("Teleport", 1f, 0.05f, 1f);
        foreach (Collider2D x in Hit) {
            if (x != null) {
                if (x.gameObject.tag == "Static") {
                    GameObject Body;
                    if (Teleported.tag == "Wizard")
                        Body = Teleported.transform.GetChild(0).gameObject;
                    else
                        Body = Teleported;
                    GameObject Engraved = GameObject.Instantiate(Engraving);
                    Engraved.transform.rotation = Body.transform.rotation;
                    Engraved.transform.position = Position;
                    Engraved.GetComponent<SpriteRenderer>().sprite = Body.GetComponent<SpriteRenderer>().sprite;
                    SceneManager.Disposables.Add(Engraved);
                    Teleported.GetComponent<Wizard>().Kill();
                    PlayAudio("Step", 1f, 0.05f, 0.2f);
                    return;
                }
                if (x.gameObject.tag == "Void") {
                    if (Teleported.tag == "Wizard") {
                        Teleported.GetComponent<Wizard>().Kill();
                    } else {
                        Teleported.GetComponent<ObjectChar>().Destroy();
                    }
                    return;
                }
            }
        }
        if (Teleported.tag != "Spell") {
            Teleported.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
		Teleported.transform.position = Position;
    }
    public void Burn(GameObject ToBeBurned) {
        if (ToBeBurned.tag == "Wizard") {
            ToBeBurned.GetComponent<Wizard>().AddEffect(new Burning(ToBeBurned, this, Burning, Ash));
        } else if (ToBeBurned.tag == "Object") {
			ToBeBurned.GetComponent<ObjectChar>().Burn();
        }
	}
    public void ClearBurn(GameObject ToBeCleared) {
        if (ToBeCleared.tag == "Wizard") {
            foreach (Effect x in ToBeCleared.GetComponent<Wizard>().ActiveEffects)
                if (x.ToString() == "Burning")
                    x.Clear();
        } else if (ToBeCleared.tag == "Object") {
            ToBeCleared.GetComponent<ObjectChar>().ClearBurn();
        }
    }
    public void Stun(GameObject ToBeStunned, float StunTime, float ResistTime) {
        if (ToBeStunned.tag == "Wizard") {
            ToBeStunned.GetComponent<Wizard>().AddEffect(new Stun(ToBeStunned, this, StunnedEffect, StunTime, ResistTime));
        } else if (ToBeStunned.tag == "Object") {
            ToBeStunned.GetComponent<ObjectChar>().Stun(StunTime);
        }
    }
	public void StunResistance(GameObject ToAddResistance, float ResistTime){
		ToAddResistance.GetComponent<Wizard>().AddEffect(new StunResistance(ToAddResistance, this, ResistTime));
	}
    public void Scorch(Vector2 Pos, float Size) {
        GameObject Mark = GameObject.Instantiate(ScorchMark);
        Mark.transform.position = Pos;
        Mark.transform.Rotate(0, 0, Random.Range(0, 360));
        Mark.transform.localScale *= Size;
        AddDisposable(Mark);
    }
	public void AddTarget(GameObject Target){
		SceneManager.Targets.Add (Target);
	}
	public void RemoveTarget(GameObject Target){
		for(int i = 0; i < SceneManager.Targets.Count; i++){
			if (SceneManager.Targets [i].Equals (Target))
				SceneManager.Targets.RemoveAt (i);
		}
	}
	public void AddDisposable(GameObject Disposed){
		SceneManager.Disposables.Add (Disposed);
	}
    public void RemoveDisposable(GameObject Disposed) {
        SceneManager.Disposables.Remove(Disposed);
    }
	public void AddShake(float Intensity, float Time) {
		Shaker.AddShake (new ShakeOrder(Intensity, Time));
	}
    public void KillLegacySpell(Spell x) {
        SceneManager.LegacySpells.Remove(x);
    }
    public float GetRandom() {
        return Random.value;
    }
    public void AddForce(GameObject Obj, Vector2 Force) {
        if (Obj.tag == "Wizard")
            Force *= 0.1f;
        Obj.GetComponent<Rigidbody2D>().AddForce(Force);
    }
    public void PlayAudio(string Name, float Volume, float PitchVariable, float PitchConstant, float Time, float Cooldown) {
        float Pitch = PitchConstant + PitchVariable * (Random.value - 0.5f) * 2f;
        AudioManager.PlayAudio(Name, Volume, Pitch, Time, Cooldown);
    }
    public void PlayAudio(string Name, float Volume, float PitchVariable, float PitchConstant) {
        float Pitch = PitchConstant + PitchVariable * (Random.value - 0.5f) * 2f;
        AudioManager.PlayAudio(Name, Volume, Pitch);
    }
    public void StopAudio() {
        AudioManager.ClearSound();
    }
}
