using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deflector : MonoBehaviour {
	public Deflect Parent;
	SpriteRenderer Renderer;
	void Start(){
		Renderer = gameObject.GetComponent<SpriteRenderer> ();
	}
	void FixedUpdate(){
		transform.position = Parent.WizardObject.transform.position + Parent.WizardObject.transform.up * 1;
		transform.rotation = Quaternion.LookRotation (Parent.WizardObject.transform.forward, Parent.WizardObject.transform.up);
		if (Parent.TimeActive < Parent.LifeTime / 2) {
			Renderer.color = new Color (16/255f,165/255f,255/255f,Parent.TimeActive / (Parent.LifeTime / 2f) * (77f/255f));
		} else {
			Renderer.color = new Color (16/255f,165/255f,255/255f,(77/255f) - (((Parent.TimeActive - (Parent.LifeTime / 2)) / (Parent.LifeTime / 2)) * (77/255f)));
		}
	}
}
