using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockChar : ObjectChar {
    Rigidbody2D Body;
    float Speed = 10f;
    float Torque = 0.025f;
    float DetectionRange = 4f;
    float Range = 1.25f;
    float TimeAlive = 0f;
    float Force = 50f;
    SceneManager SceneManager;
    GameBehaviors Game;
    GameObject Target;
    bool ValidTarget;
    public GameObject PreExplosion;
    public GameObject Caster;
    void Start() {
        Body = gameObject.GetComponent<Rigidbody2D>();
        SceneManager = GameObject.Find("SceneManager").GetComponent<SceneManager>();
        Game = GameObject.Find("SceneManager").GetComponent<GameBehaviors>();
    }
    void OnDrawGizmos() {
        Gizmos.DrawWireSphere(transform.position, DetectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, Range);
    }
    void OnCollisionEnter2D(Collision2D Col) {
        Destroy();
    }
    void FixedUpdate() {
        TimeAlive += Time.fixedDeltaTime;
        if (TimeAlive > 10f)
            Destroy();
        Body.AddForce(transform.up * Speed);
        float ClosestDistance = 10000;
        ValidTarget = false;
        Target = gameObject;
        List<GameObject> Targets = SceneManager.Targets;
        for (int i = 0; i < Targets.Count; i++) {
            float Dist = Vector2.Distance(Targets[i].transform.position, transform.position);
            if (Dist < DetectionRange) {
                if (TimeAlive > 0.75f || !Targets[i].Equals(Caster)) {
                    if (!ValidTarget) {
                        ValidTarget = true;
                        ClosestDistance = Dist;
                        Target = Targets[i];
                    } else if (ClosestDistance > Vector2.Distance(Targets[i].transform.position, transform.position)) {
                        ClosestDistance = Dist;
                        Target = Targets[i];
                    }
                }
            }
        }
        if (ValidTarget) {
            Vector2 LocalCords = transform.InverseTransformPoint(Target.transform.position);
            if (LocalCords.x < 0) {
                Body.AddTorque(Random.value * Torque * 3);
            } else {
                Body.AddTorque(Random.value * -Torque * 3);
            }
        } else {
            if (Random.value > 0.5) {
                Body.AddTorque(Random.value * Torque);
            } else {
                Body.AddTorque(Random.value * -Torque);
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
        Game.PlayAudio("ShatterExplosion", 1f, 0.05f, 1f, 0.3f, 0.1f);
        Game.AddShake(0.02f, 0.25f);
       GameObject Parts = GameObject.Instantiate(PreExplosion);
        Parts.transform.position = transform.position;
        List<GameObject> Hit = Game.GetMoveables(transform.position, Range, gameObject);
        foreach (GameObject x in Hit) {
            Game.Stun(x, 5f, 1f);
            Vector2 Dir = (x.transform.position - transform.position).normalized;
            Game.AddForce(x, Dir * Force);
        }
        GameObject.Destroy(gameObject);
    }
}
