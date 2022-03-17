using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningProjectile : SpellChar {
	public GameObject ScatterEffect;
	public Lightning Parent;
	public float Speed;
	Rigidbody2D Body;
	void Start(){
        Power = 5;
		Body = gameObject.GetComponent<Rigidbody2D> ();
		Body.velocity = transform.up * Speed;
	}
    public override void StaticContact(Collision2D Col) {
        Parent.StaticContact();
    }
    public override void WizardContact(Collision2D Col) {
        Parent.WizardContact(Col.gameObject);
    }
    public override void ObjectContact(Collision2D Col) {
        Parent.ObjectContact(Col.gameObject);
    }
    public override void SpellContact(Collision2D Col) {
        Col.gameObject.GetComponent<SpellChar>().Destroy();
    }
    public void Scatter(){
		gameObject.GetComponent<BoxCollider2D> ().isTrigger = true;
		GameObject Scatter = GameObject.Instantiate(ScatterEffect);
		Scatter.transform.position = this.transform.position;
		Scatter.transform.eulerAngles = new Vector3(transform.eulerAngles.z - 90,-90,90);
		GameObject.Destroy (this.gameObject);
	}
    public override void Destroy() {
        Parent.Destroy();
    }
}
