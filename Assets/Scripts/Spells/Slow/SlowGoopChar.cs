using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowGoopChar : ObjectChar {
    List<GameObject> Affected;
    Collider2D Area;
    SpriteRenderer Spr;
    float Change = 4.5f;
    bool BeingCleared = false;
    float TimeAlive = 0;
    float TimeCleared = 0;
    void Start() {
        Affected = new List<GameObject>();
        Area = gameObject.GetComponent<Collider2D>();
        Spr = gameObject.GetComponent<SpriteRenderer>();
    }
    void Update() {
        TimeAlive += Time.deltaTime;
        if (TimeAlive > 14f && !BeingCleared)
            BeingCleared = true;
        if (BeingCleared) {
            TimeCleared += Time.deltaTime;
            if (TimeCleared > 1f)
                Destroy();
            Spr.color = new Color(1, 1, 1, 1 - TimeCleared);
        }
        List<GameObject> Current = new List<GameObject>();
        foreach (GameObject x in Affected)
            if(x != null)
                Current.Add(x);
        List<Collider2D> Results = new List<Collider2D>();
        ContactFilter2D Filter = new ContactFilter2D();
        Area.OverlapCollider(Filter, Results);
        foreach (GameObject x in Current) {
            bool Accounted = false;
            foreach (Collider2D y in Results) {
                if (x.Equals(y.gameObject))
                    Accounted = true;
            }
            if (!Accounted)
                if (x.GetComponent<Rigidbody2D>() != null) {
                    x.GetComponent<Rigidbody2D>().drag /= Change;
                }
        }
        foreach (Collider2D y in Results) {
            bool Accounted = false;
            foreach (GameObject x in Current) {
                if (x.Equals(y.gameObject))
                    Accounted = true;
            }
            if (!Accounted)
                if (y.gameObject.GetComponent<Rigidbody2D>() != null)
                    y.gameObject.GetComponent<Rigidbody2D>().drag *= Change;
        }
        Affected = new List<GameObject>();
        foreach (Collider2D y in Results) {
            Affected.Add(y.gameObject);
        }
    }
    public override void ClearBurn() {
        BeingCleared = true;
    }
    public override void Destroy() {
        GameObject.Destroy(gameObject);
    }
}