using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
	List<ShakeOrder> Shakes = new List<ShakeOrder>();
	float TotalIntensity = 0;
	Vector3 OriginalPos;
	void Start() {
		OriginalPos = Camera.main.gameObject.transform.position;
	}
    void Update() {
        if (Shakes.Count != 0) {
            List<ShakeOrder> ToRemove = new List<ShakeOrder>();
            for (int i = 0; i < Shakes.Count; i++) {
                Shakes[i].Time -= Time.deltaTime;
                if (Shakes[i].Time <= 0) {
                    TotalIntensity -= Shakes[i].Intensity;
                    ToRemove.Add(Shakes[i]);
                }
            }
            for (int i = 0; i < ToRemove.Count; i++) {
                Shakes.Remove(ToRemove[i]);
            }
            Camera.main.gameObject.transform.position = OriginalPos + Random.insideUnitSphere * TotalIntensity;
        }
    }
	public void AddShake(ShakeOrder Shake) {
		Shakes.Add (Shake);
		TotalIntensity += Shake.Intensity;
	}
}
