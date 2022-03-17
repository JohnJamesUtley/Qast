using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnapProjectile : SpellChar {
	public GameObject BreakEffect;
    public GameObject TrailEffect;
	public Snap Parent;
	public float Speed;
	Rigidbody2D Body;
	void Start(){
        Power = 10;
		Body = gameObject.GetComponent<Rigidbody2D> ();
		Body.velocity = transform.up * Speed;
	}
    public override void StaticContact(Collision2D Col) {
        Parent.StaticContact(Col);
    }
    public override void WizardContact(Collision2D Col) {
        Parent.ObjectContact(Col.gameObject);
    }
    public override void ObjectContact(Collision2D Col) {
        Parent.ObjectContact(Col.gameObject);
    }
    public override void SpellContact(Collision2D Col) {
        Parent.ObjectContact(Col.gameObject);
    }
    public void Break(){
		GameObject Effect = GameObject.Instantiate(BreakEffect);
		Effect.transform.position = this.transform.position;
		GameObject.Destroy (this.gameObject);
	}
    public void Trail() {
        GameObject Trail = GameObject.Instantiate(TrailEffect);
        Trail.transform.position = this.transform.position;
        Trail.transform.rotation = this.transform.rotation;
        Trail.GetComponent<SpriteRenderer>().sprite = this.GetComponent<SpriteRenderer>().sprite;
    }
    public override void Destroy() {
        Parent.Destroy();
    }
}
