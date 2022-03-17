using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FontFix : MonoBehaviour
{
    public Font standard;
    void Awake() {
        Text[] fontsToFix = Object.FindObjectsOfType<Text>();
        for(int i = 0; i < fontsToFix.Length; i++) {
            if(fontsToFix[i].font == null)
                fontsToFix[i].font = standard;
        }
    }

}
