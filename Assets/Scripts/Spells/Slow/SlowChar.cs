using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowChar : ObjectChar {
    public GameObject PreGoop;
    public GameObject GoopParts;
    float PopTime = 0.5f;
    GameBehaviors Game;
    Rigidbody2D Body;
    void Start() {
        Game = GameObject.Find("SceneManager").GetComponent<GameBehaviors>();
        Body = gameObject.GetComponent<Rigidbody2D>();
    }
    void Update() {
        if(Vector2.Distance(Vector2.zero, Body.velocity) < 3f) {
            PopTime -= Time.deltaTime;
            if (PopTime < 0) {
                Destroy();
            }
        }
    }
    public override void Burn() {
        Destroy();
    }
    public override void Damage() {
        Destroy();
    }
    public override void Destroy() {
        GameObject Goop = GameObject.Instantiate(PreGoop);
        GameObject Parts = GameObject.Instantiate(GoopParts);
        Parts.transform.position = transform.position;
        Game.PlayAudio("SlowExplode", 0.3f, 0.05f, 1f);
        Game.AddDisposable(Goop);
        Goop.transform.position = transform.position;
        Goop.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
        GameObject.Destroy(gameObject);
    }
}
