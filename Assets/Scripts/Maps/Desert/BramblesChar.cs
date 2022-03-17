using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BramblesChar : ObjectChar {
    Vector3 OrgPos;
    int Level = 2;
    public Sprite[] Visuals;
    public GameObject PreBurn;
    public GameObject PreParts;
    GameBehaviors Game;
    bool Burning = false;
    float BurnTime;
    GameObject Parts;
    SpriteRenderer Srt;
    Collider2D Col;
    public void Start() {
        Srt = gameObject.GetComponent<SpriteRenderer>();
        Col = gameObject.GetComponent<Collider2D>();
        Game = GameObject.Find("SceneManager").GetComponent<GameBehaviors>();
        OrgPos = transform.position;
    }
    public void Update() {
        if (Burning) {
            BurnTime += Time.deltaTime;
            if(BurnTime > 2.55f) {
                Damage();
                BurnTime = 0;
                if (Level == 0)
                    ClearBurn();
            }
        }
    }
    void OnCollisionEnter2D(Collision2D collision) {
        if (Burning) {
            Game.Burn(collision.gameObject);
        }
    }
    public override void ClearBurn() {
        GameObject.Destroy(Parts);
        Burning = false;
    }
    public override void Burn() {
        if (!Burning) {
            Burning = true;
            BurnTime = 0;
            Parts = GameObject.Instantiate(PreBurn);
            Parts.transform.parent = transform;
            Parts.transform.localPosition = new Vector2(0, 0.5f);
            Parts.transform.localScale = Vector3.one;
            Parts.transform.rotation = transform.rotation;
        }
    }
    public override void Damage() {
        if (Level != 0) {
            Level--;
            UpdateVisuals();
        }
    }
    public void Heal() {
        if (Level != 2) {
            Level++;
            UpdateVisuals();
        }
    }
    void UpdateVisuals() {
        GameObject Sticks = GameObject.Instantiate(PreParts);
        Sticks.transform.position = transform.position;
        Game.PlayAudio("ProxyHit", 0.3f, 0.05f, 1f);
        Srt.sprite = Visuals[Level];
        if (Level == 0) {
            Col.enabled = false;
            transform.position = OrgPos;
        } else if(Level == 1){
            Col.enabled = true;
        }
    }
}
