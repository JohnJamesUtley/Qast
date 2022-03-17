using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunnedObject : MonoBehaviour
{
	public GameObject PreStunStar;
	GameObject[] StunStars;
	public float Dist;
	public float Speed;
	float Alive = 0;
	void Start(){
		StunStars = new GameObject[3];
		for (int i = 0; i < StunStars.Length; i++) {
			GameObject Created = GameObject.Instantiate (PreStunStar);
			float Angle = Mathf.Deg2Rad * i * 120;
			Vector2 Dir = new Vector2(Mathf.Cos(Angle),Mathf.Sin(Angle));
			Vector2 Pos = (Vector2)transform.position + Dir * Dist;
			Created.transform.position = Pos;
			Created.transform.parent = transform;
			StunStars [i] = Created;
		}
	}
    void Update()
    {
		Alive += Time.deltaTime;
		for (int i = 0; i < StunStars.Length; i++) {
			float Angle = Mathf.Deg2Rad * ((float)i * 120f + Alive * Speed);
			Vector2 Dir = new Vector2(Mathf.Cos(Angle),Mathf.Sin(Angle));
			Vector2 Pos = (Vector2)transform.position + Dir * Dist;
			StunStars[i].transform.position = Pos;
		}
    }
}
