using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastingInfo {
	public float Accuracy;
	public string Name;
	public CastingInfo(string Name, float Accuracy){
		this.Accuracy = Accuracy;
		this.Name = Name;
	}
	public override string ToString ()
	{
		return "CastingInfo: Name-" + Name + " Acr%" + Accuracy * 100;
	}
}
