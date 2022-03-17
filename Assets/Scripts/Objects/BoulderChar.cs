using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoulderChar : ObjectChar
{
	public override void Destroy() {
		GameObject.Destroy (gameObject);
	}
}
