using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Canopy : MonoBehaviour {
    Collider2D Area;
    SceneManager SceneManager;
    SpriteRenderer Rend;
    void Start() {
        Area = gameObject.GetComponent<Collider2D>();
        SceneManager = GameObject.Find("SceneManager").GetComponent<SceneManager>();
        Rend = gameObject.GetComponent<SpriteRenderer>();
    }
    void Update() {
        List<Collider2D> Results = new List<Collider2D>();
        ContactFilter2D Filter = new ContactFilter2D();
        Area.OverlapCollider(Filter, Results);
        bool Half = false;
        foreach(Collider2D x in Results) {
            if(x.gameObject.tag != "Static") {
                foreach(GameObject y in SceneManager.Targets) {
                    if (x.gameObject.Equals(y))
                        Half = true;
                }
            }
        }
        if (Half)
            Rend.color = new Color(1, 1, 1, 0.65f);
        else
            Rend.color = new Color(1, 1, 1, 1);
    }
}
