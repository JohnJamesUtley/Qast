using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionChar : ObjectChar {
    SceneManager SceneManager;
    GameBehaviors Game;
    Rigidbody2D Body;
    public Wizard Caster;
    public GameObject MagicDust;
    public float Speed;
    public GameObject Target;
    public float Force;
    float MoveDiff = 1.25f;
    float RotationDiff = 35;
    float TimeFromJump = 0;
    Vector2 MoveTo;
    bool Direct = false;
    public float TimeAlive = 0;
    void Start() {
        SceneManager = GameObject.Find("SceneManager").GetComponent<SceneManager>();
        Game = GameObject.Find("SceneManager").GetComponent<GameBehaviors>();
        Body = gameObject.GetComponent<Rigidbody2D>();
        SceneManager.Targets.Add(gameObject);
        Game.AddDisposable(gameObject);
        Game.PlayAudio("Minion", 0.15f, 0.05f, 1f);
    }
    void FixedUpdate() {
        bool ValidTarget = false;
        float ClosestDistance = 1000000;
        GameObject CurrentTarget = gameObject;
        List<GameObject> Targets = new List<GameObject>();
        foreach (GameObject x in SceneManager.Targets)
            Targets.Add(x);
        foreach (GameObject x in Caster.ExemptTargets)
            Targets.Remove(x);
        for (int i = 0; i < Targets.Count; i++) {
            if (!ValidTarget) {
                ValidTarget = true;
                ClosestDistance = Vector2.Distance(Targets[i].transform.position, transform.position);
                CurrentTarget = Targets[i];
            } else if (ClosestDistance > Vector2.Distance(Targets[i].transform.position, transform.position)) {
                ClosestDistance = Vector2.Distance(Targets[i].transform.position, transform.position);
                CurrentTarget = Targets[i];
            }
        }
        if (Vector2.Distance(MoveTo, transform.position) < 0.1f) {
            MoveTo = JumpingPosition(Target.transform.position);
        }
        if (Target != null) {
            if (Vector2.Distance(MoveTo, transform.position) > Vector2.Distance(MoveTo, Target.transform.position)) {
                MoveTo = JumpingPosition(Target.transform.position);
            }
            if (Vector2.Distance(MoveTo, transform.position) > MoveDiff * 1.25f) {
                MoveTo = JumpingPosition(Target.transform.position);
            }
        }
        if (!CurrentTarget.Equals(gameObject)) {
            bool NewTarget = false;
            if (Target != null) {
                if (!Target.Equals(CurrentTarget)) {
                    NewTarget = true;
                }
            } else {
                NewTarget = true;
            }
            if (NewTarget) {
                Target = CurrentTarget;
                MoveTo = JumpingPosition(Target.transform.position);
            }
        }
        float RotationTarget = 0;
        if (ValidTarget) {
            RotationTarget = Mathf.Atan2(MoveTo.y - gameObject.transform.position.y, MoveTo.x - gameObject.transform.position.x) * Mathf.Rad2Deg - 90;
        }
        if (RotationTarget < 0) {
            RotationTarget += 360;
        }
        float OldRotation = Body.rotation;
        if (OldRotation < 0) {
            OldRotation += 360;
        }
        float Difference = RotationTarget - OldRotation;
         if (Mathf.Abs(Difference) > 180) {
            if (RotationTarget < OldRotation) {
                Difference += 360;
            } else {
                Difference -= 360;
            }
        }
        Body.rotation += Difference * Time.fixedDeltaTime * 15;
        Body.AddForce(transform.up * Speed);
        TimeAlive += Time.fixedDeltaTime;
        TimeFromJump += Time.fixedDeltaTime;
        if (TimeFromJump > 1f) {
            if (Target != null) {
                MoveTo = JumpingPosition(Target.transform.position);
            }
        }
        if (TimeAlive > 12.5f) {
            Destroy();
        }
    }
    Vector2 JumpingPosition(Vector2 EndPos) {
        TimeFromJump = 0;
        if (Vector2.Distance(transform.position, EndPos) < MoveDiff) {
            Direct = true;
            return Target.transform.position;
        } else {
            Direct = false;
            float Rotation = Mathf.Atan2(Target.transform.position.y - gameObject.transform.position.y, Target.transform.position.x - gameObject.transform.position.x) * Mathf.Rad2Deg;
            Rotation += (Random.value * 2 - 1) * RotationDiff;
            if (Rotation < 0) {
                Rotation += 360;
            }
            Rotation *= Mathf.Deg2Rad;
            return new Vector2(Mathf.Cos(Rotation) * MoveDiff + transform.position.x, Mathf.Sin(Rotation) * MoveDiff + transform.position.y);
        }
    }
    void OnCollisionEnter2D(Collision2D Col) {
        GameObject Hit = Col.gameObject;
        if (Hit.Equals(Target)) {
            Vector2 Dir = (Hit.transform.position - transform.position).normalized;
            Game.AddForce(Hit, Dir * Force);
            Game.Stun(Hit, 3f, 2.5f);
            Destroy();
        }
    }
    public override void ClearBurn() {
        Destroy();
    }
    public override void Stun(float StunTime) {
        Destroy();
    }
    public override void Damage() {
        Destroy();
    }
    public override void Destroy() {
        Game.PlayAudio("Minion", 0.15f, 0.05f, 1f);
        GameObject DeadDust = GameObject.Instantiate(MagicDust);
        DeadDust.transform.position = transform.position;
        GameObject.Destroy(gameObject);
    }
    void OnDestroy() {
        Caster.ExemptTargets.Remove(gameObject);
        SceneManager.Targets.Remove(gameObject);
    }
}
