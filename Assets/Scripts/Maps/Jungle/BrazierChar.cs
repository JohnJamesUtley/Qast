using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrazierChar : ObjectChar {
    public GameObject PreBurn;
    GameObject BurnParts;
    public JungleDoor Door;
    public bool Burning;
    Vector2 OrgPos;
    void Start() {
        OrgPos = transform.position;
    }
    void Update() {
        transform.position = OrgPos;
    }
    public override void Burn() {
        if (!Burning) {
            Burning = true;
            BurnParts = GameObject.Instantiate(PreBurn);
            BurnParts.transform.parent = transform;
            BurnParts.transform.localPosition = Vector2.zero;
            BurnParts.transform.localScale = Vector2.one;
            BurnParts.transform.rotation = Quaternion.Euler(0, 0, 0);
            Door.Open();
        }
    }
    public override void ClearBurn() {
        if (Burning) {
            Burning = false;
            GameObject.Destroy(BurnParts);
            Door.Close();
        }
    }
}
