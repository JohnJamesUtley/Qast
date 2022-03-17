using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapInfo : MonoBehaviour
{
	public float CameraSize;
	public float YOffset;
	public GameObject[] Spawns;
	public ParticleSystem[] Particles;
	public List<Event> Events;
	public bool EventAuthority;
	public Vector2 EventTime;
	public float TimeToEvent;
	void Start(){
		AddEvents ();
		foreach (Event x in Events)
			x.Map = this;
		OnStart ();
		ResetRandomEvent ();
	}
	void Update(){
		foreach (Event x in Events)
			x.Update ();
		if (EventAuthority) {
			TimeToEvent -= Time.deltaTime;
			if (TimeToEvent < 0) {
				ResetRandomEvent ();
				PlayRandomEvent ();
			}
		}
	}
	void ResetRandomEvent(){
		TimeToEvent = Random.Range (EventTime.x, EventTime.y);
	}
	void PlayRandomEvent(){
		float TotalInfluence = 0;
		foreach (Event x in Events) {
			TotalInfluence += x.Influence;
		}
		float InfluenceMeasure = Random.value * TotalInfluence;
		for (int i = 0; i < Events.Count; i++) {
			InfluenceMeasure -= Events [i].Influence;
			if (InfluenceMeasure < 0) {
				Events [i].CallEvent ();
				return;
			}
		}
	}
	public virtual void OnStart(){}
	public virtual void AddEvents (){}
}
