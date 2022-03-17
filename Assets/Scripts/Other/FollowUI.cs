using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FollowUI : MonoBehaviour
{
    public GameObject ToFollow;
    public bool Alpha;
    ParticleSystem Parts;
    Image Img;
    bool On;
    void Start() {
        Parts = gameObject.GetComponent<ParticleSystem>();
        Img = ToFollow.GetComponent<Image>();
    }
    public void Set(bool On) {
        this.On = On;
    }
    void Update() {
        if (On) {
            transform.position = ToFollow.transform.position;
            transform.rotation = ToFollow.transform.rotation;
            if (Alpha) {
                ParticleSystem.MainModule Main = Parts.main;
                Color Min = Main.startColor.colorMin;
                Color Max = Main.startColor.colorMax;
                Min = new Color(Min.r, Min.g, Min.b, Img.color.a);
                Max = new Color(Max.r, Max.g, Max.b, Img.color.a);
                Main.startColor = new ParticleSystem.MinMaxGradient(Min, Max);
            }
        }
    }
}
