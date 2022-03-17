using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfoAdjuster : MonoBehaviour
{
    int Full = 1952;
    float FullScale = 1.15f;
    // Start is called before the first frame update
    void Start()
    {
        float Percent = (float)Screen.width / (float)1952;
        transform.localScale = new Vector2(Percent * FullScale, Percent * FullScale);
    }

}
