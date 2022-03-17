using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShatterProjectile : SpellChar {
    public Shatter Parent;
    public GameObject Explosion;
    public float Speed;
    Rigidbody2D Body;
    void Start() {
        Power = 0;
        Body = gameObject.GetComponent<Rigidbody2D>();
        Body.velocity = transform.up * Speed;
    }
    public void Explode() {
        GameObject Boom = GameObject.Instantiate(Explosion);
        Boom.transform.position = transform.position;
        Boom.transform.rotation = transform.rotation;
        Parent.Game.Scorch(gameObject.transform.position, 4);
        Parent.Game.PlayAudio("ShatterExplosion", 1f, 0.15f, 1f);
        GameObject.Destroy(gameObject);
    }
}
