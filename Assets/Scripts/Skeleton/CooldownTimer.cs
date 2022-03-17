using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CooldownTimer : MonoBehaviour
{
    public float TotalTime;
    public Sprite[] Progression;
    public int CurrentKnot;
    Image Item;
	Color OriginalColor;
    public void Initialize() {
        Item = gameObject.GetComponent<Image>();
		if(OriginalColor == Color.clear)
			OriginalColor = Item.color;
        Item.color = Color.clear;
    }
    public void Enable(float Time) {
        Item.sprite = Progression[0];
		Item.color = OriginalColor;
		CurrentKnot = 0;
		TotalTime = Time;
    }
    public void Disable() {
        Item.color = Color.clear;
    }
    public void UpdatePercent(float TimeLeft) {
		int Knots = (int)(((TotalTime - TimeLeft) / TotalTime) / (1f / Progression.Length));
        if (Knots > CurrentKnot) {
            CurrentKnot = Knots;
			if(CurrentKnot < Progression.Length)
            	Item.sprite = Progression[CurrentKnot];
			else
				Item.sprite = Progression[0];
        }
    }
}
