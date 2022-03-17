using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minion : Spell
{
    GameObject PreMinion;
    GameObject PreMagicDust;
    GameObject[] Minis;
    public Minion(float SpellWait, GameObject Fizzle, float FizzlePoint, string Name, float Cooldown, GameObject PreMinion, GameObject PreMagicDust) : base(SpellWait, Fizzle, FizzlePoint, Name, Cooldown) {
        this.PreMinion = PreMinion;
        this.PreMagicDust = PreMagicDust;
    }
    public override void CastEffect() {
        base.CastEffect();
        Minis = new GameObject[3];
        for (int i = 0; i < 3; i++) {
            GameObject Parts = GameObject.Instantiate(PreMagicDust);
            Minis[i] = GameObject.Instantiate(PreMinion);
            Quaternion Direction = Quaternion.Euler(new Vector3(0, 0, WizardObject.GetComponent<Wizard>().RotationTarget));
            Minis[i].transform.rotation = Direction;
            Minis[i].transform.position = WizardObject.transform.position + Direction * Vector3.up * 1.5f + (Vector3)Random.insideUnitCircle * 1.25f;
            Parts.transform.position = Minis[i].transform.position;
            Minis[i].GetComponent<MinionChar>().Caster = WizardScript;
            WizardScript.ExemptTargets.Add(Minis[i]);
        }
    }
    public override Spell Copy() {
        Minion TheCopy = new Minion(SpellWait, Fizzle, FizzlePoint, Name, Cooldown, PreMinion, PreMagicDust);
        return CopyBasics(TheCopy);
    }
    public override void Destroy() {
        foreach(GameObject Mini in Minis)
           Mini.GetComponent<MinionChar>().Destroy();
    }
}
