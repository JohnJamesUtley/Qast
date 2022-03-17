using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HowToUI : MonoBehaviour {
	public Canvas[] HowTo1;
	public Canvas[] HowTo2;
	public Canvas[] HowTo3;
	public Canvas[] HowTo4;
	public Animator[] Anims;
	public ParticleSystem[] Particles;
    public FollowUI[] Follows;
	bool Ran4;
	void Start(){
		Ran4 = false;
	}
    public void LoadUI1(){
		if (Ran4) {
			Anims [4].SetTrigger ("Reset");
			Ran4 = false;
		}
		StartCoroutine ("Reveal1");
	}
	public void LoadUI2(){
		Anims [0].SetTrigger ("Reset");
		Anims [1].SetTrigger ("Reset");
		Anims [2].SetTrigger ("Appear");
        Follows[0].Set(true);
		Particles [0].Play ();
	}
	public void LoadUI3(){
        Follows[0].Set(false);
        Follows[1].Set(true);
        Anims[2].SetTrigger ("Reset");
		Particles [0].Stop ();
		Anims [3].SetTrigger ("Appear");
		StartCoroutine ("Reveal3");
	}
	public void LoadUI4(){
        Follows[1].Set(false);
        Anims[3].SetTrigger ("Reset");
		Particles [1].Stop ();
		Anims [4].SetTrigger ("Appear");
		Ran4 = true;
	}
	IEnumerator Reveal1(){
		Anims [0].SetTrigger ("Appear");
		yield return new WaitForSeconds (1.5f);
		Anims [1].SetTrigger ("Appear");
	}
	IEnumerator Reveal3(){
		yield return new WaitForSeconds (0.4f);
		Particles [1].Play ();
	}

}
