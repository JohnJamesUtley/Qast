using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Effect
{
    public GameObject Wizard;
	public Wizard WizardScript;
    public GameBehaviors Game;
    public Effect(GameObject Wizard, GameBehaviors Game) {
        this.Wizard = Wizard;
        this.Game = Game;
		WizardScript = Wizard.GetComponent<Wizard> ();
    }
	abstract public void Initialize();
    abstract public void Run();
    abstract public void Clear();
}
