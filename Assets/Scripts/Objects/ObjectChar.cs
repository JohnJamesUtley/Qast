using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectChar : MonoBehaviour {
    public virtual void Burn() {}
    public virtual void ClearBurn() {}
	public virtual void Damage() {}
    public virtual void Stun(float StunTime) {}
    public virtual void Destroy() {}
}
