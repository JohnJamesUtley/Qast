using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class MummiesEvent : Event
{
	public GameObject MummyPrefab;
	public GameObject DirtParticles;
	public GameObject[] MummySpawns;
	public float[] MummySpawnRanges;
	public List<GameObject> Mummys;
	bool SpawningPhase = true;
	public float Scale;
	float OptimalScale = 1.4f;
	public override void EventAction ()
	{
		base.EventAction ();
		Game.AddShake (0.05f,2.6f);
		Mummys = new List<GameObject> ();
		Scale = 0.01f;
		int NumMummies = Random.Range (3,6);
		for (int i = 0; i < NumMummies; i++) {
			GameObject Parts = GameObject.Instantiate (DirtParticles);
			Mummys.Add (GameObject.Instantiate (MummyPrefab));
			int SpawnNum = Random.Range (0,MummySpawns.Length);
			Vector2 Position = (Random.insideUnitCircle * MummySpawnRanges[SpawnNum]) + (Vector2)MummySpawns[SpawnNum].transform.position;
			Mummys [i].transform.position = Position;
			Parts.transform.position = Position;
			Mummys [i].transform.parent = Map.gameObject.transform;
			Mummys [i].transform.localScale = new Vector2 (Scale, Scale);
			Mummys [i].transform.rotation = Quaternion.Euler(new Vector3 (0, 0, Random.Range (0, 360)));
		}
        Game.PlayAudio("Rumble", 0.5f, 0f, 1f);
	}

	public override void Update ()
	{
		base.Update ();
		if (!Closed) {
			if (SpawningPhase) {
				Scale += OptimalScale * Time.deltaTime / 2.5f;
				if (Scale < OptimalScale) {
					for (int i = 0; i < Mummys.Count; i++)
						Mummys [i].transform.localScale = new Vector2 (Scale, Scale);
				} else {
					SpawningPhase = false;
					for (int i = 0; i < Mummys.Count; i++)
						Mummys [i].GetComponent<MummyChar> ().Spawning = false;
					Closed = true;
					SpawningPhase = true;
				}
			}
		}
	}
}
