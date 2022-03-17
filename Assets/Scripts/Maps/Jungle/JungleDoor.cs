using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JungleDoor : MonoBehaviour {
    public bool Opened;
    Vector2 OrgPos;
    Vector2 ToBe;
    Vector2 StartPos;
    float Percent = 1;
    GameBehaviors Game;
    void Start() {
        Game = GameObject.Find("SceneManager").GetComponent<GameBehaviors>();
        OrgPos = transform.position;
    }
    /*
    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.tag == "Wizard")
            if (Opened) {
                Close();
                return;
            } else
                Open();
    }
    */
    public void Open() {
        Opened = true;
        ToBe = OrgPos - (Vector2)transform.up * 2 * transform.localScale.y;
        Percent = 0;
        StartPos = transform.position;
        Game.PlayAudio("Rumble", 0.2f, 0.15f, 1f, 0.6f, 0.1f);
    }
    public void Close() {
        Opened = false;
        ToBe = OrgPos;
        Percent = 0;
        StartPos = transform.position;
        Game.PlayAudio("Rumble", 0.2f, 0.15f, 1f, 0.6f, 0.1f);
    }
    void FixedUpdate() {
        if (Percent < 1f) {
            Percent += Time.fixedDeltaTime;
            transform.position = Vector2.Lerp(StartPos,ToBe,Percent);
        }
    }
}
