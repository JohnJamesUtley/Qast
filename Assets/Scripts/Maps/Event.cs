using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Event {
	public string Name;
	public int TimeCost;
	public float Influence;
	public int Cooldown;
	public float Timer;
	bool Called = false;
	public GameBehaviors Game;
	public MapInfo Map;
	SceneManager Scene;
	public bool Closed = true;
	public void CallEvent(){
		if (!Called) {
			EventAction ();
		}
	}
	public virtual void EventAction(){
		Game = GameObject.Find ("SceneManager").GetComponent<GameBehaviors> ();
		Scene = GameObject.Find ("SceneManager").GetComponent<SceneManager> ();
		foreach(PlayerWire x in Scene.Players){
			x.RpcSetEventRan (Name);
		}
		Timer = 0;
		Called = true;
		Closed = false;
	}
	public virtual void Update(){
		if(Called){
			Timer += Time.deltaTime;
			if (Timer > Cooldown)
				Called = false;
		}
	}
}
