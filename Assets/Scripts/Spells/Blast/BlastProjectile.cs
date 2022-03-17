using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlastProjectile : SpellChar {
	public Blast Parent;
	public GameObject Explosion;
	public float Speed;
	Rigidbody2D Body;
	void Start(){
        Power = 2;
		Body = gameObject.GetComponent<Rigidbody2D> ();
		Body.velocity = transform.up * Speed;
	}
    public override void StaticContact(Collision2D Col) {
        Parent.StaticContact(Col.contacts[0].point);
    }
    public override void WizardContact(Collision2D Col) {
        Parent.WizardContact(Col.gameObject);
    }
    public override void ObjectContact(Collision2D Col) {
        Parent.ObjectContact(Col.gameObject);
    }
    public override void SpellContact(Collision2D Col) {
        Col.gameObject.GetComponent<SpellChar>().Burn();
    }
    public void Explode(Vector2 ScorchPos){
		GameObject Explode = GameObject.Instantiate (Explosion);
		Explode.transform.position = this.transform.position;
        Parent.Game.Scorch(ScorchPos, 1);
		GameObject.Destroy (gameObject);
	}
	public void Explode(){
		GameObject Explode = GameObject.Instantiate (Explosion);
		Explode.transform.position = this.transform.position;
		GameObject.Destroy (gameObject);
	}
    public override void Destroy() {
        Parent.Destroy();
    }
}
