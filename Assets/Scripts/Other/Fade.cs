using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fade : MonoBehaviour
{
    public float DisappearWait;
    public float Life;
    public float TimeAlive;
    public Color NeededColor;
    SpriteRenderer Renderer;
    void Start()
    {
        TimeAlive = 0;
        Renderer = gameObject.GetComponent<SpriteRenderer>();
    }
    void Update()
    {
        TimeAlive += Time.deltaTime;
        if (TimeAlive > Life)
        {
            GameObject.Destroy(gameObject);
        }
        else if (TimeAlive > DisappearWait)
        {
            if(Renderer != null)
            Renderer.color = new Color(NeededColor.r, NeededColor.g, NeededColor.b, 1 - (TimeAlive - DisappearWait) / (Life - DisappearWait));
        }
    }
}
