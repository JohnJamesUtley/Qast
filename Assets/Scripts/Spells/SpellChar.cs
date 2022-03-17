using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellChar : MonoBehaviour {
    public int Power;
    void OnCollisionEnter2D(Collision2D Col) {
        if (Col.gameObject.tag == "Static") {
            StaticContact(Col);
        } else if (Col.gameObject.tag == "Wizard") {
            WizardContact(Col);
        } else if (Col.gameObject.tag == "Object") {
            ObjectContact(Col);
        } else if (Col.gameObject.tag == "Spell") {
            if(Power >= Col.gameObject.GetComponent<SpellChar>().Power)
                SpellContact(Col);
        }
    }
    public virtual void StaticContact(Collision2D Col) {
        return;
    }
    public virtual void WizardContact(Collision2D Col) {
        return;
    }
    public virtual void ObjectContact(Collision2D Col) {
        return;
    }
    public virtual void SpellContact(Collision2D Col) {
        return;
    }
    public virtual void Burn() {
        Destroy();
        return;
    }
    public virtual void Destroy() {
        Debug.LogError("No Destory Method",gameObject);
        return;
    }
}
