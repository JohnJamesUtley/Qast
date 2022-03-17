using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class SandstormEvent : Event
{
	public ParticleSystem Sand;
	public bool MidPoint;
	public override void EventAction ()
	{
		base.EventAction ();
		MidPoint = false;
		ParticleSystem.EmissionModule Emission = Sand.emission;
		Emission.rateOverTime = new ParticleSystem.MinMaxCurve (100f);
		ParticleSystem.MainModule Main = Sand.main;
		Main.startSpeed = new ParticleSystem.MinMaxCurve(10f);
        Game.PlayAudio("Sandstorm", 0.5f, 0f, 1f, 10f, 3f);
	}
	public override void Update ()
	{
		base.Update ();
		if (!Closed) {
			List<GameObject> All = Game.GetMoveables ();
            foreach (GameObject x in All)
                if (x != null)
                    Game.AddForce(x, 1.25f * Vector2.right);
		}
		if (!MidPoint && Timer > 10) {
			MidPoint = true;
			ParticleSystem.EmissionModule Emission = Sand.emission;
			Emission.rateOverTime = new ParticleSystem.MinMaxCurve (10f,100f);
			ParticleSystem.MainModule Main = Sand.main;
			Main.startSpeed = new ParticleSystem.MinMaxCurve(5f,10f);
		}
		if (!Closed && Timer > 12) {
			Closed = true;
			ParticleSystem.EmissionModule Emission = Sand.emission;
			Emission.rateOverTime = new ParticleSystem.MinMaxCurve (10f);
			ParticleSystem.MainModule Main = Sand.main;
			Main.startSpeed = new ParticleSystem.MinMaxCurve(5f);
		}
	}
}
