using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellInfoAccess : MonoBehaviour{
    public SpellInfo[] AllSpellInfo;
    public DeckType[] DeckTypes;
    public SpellInfo GetInfo(string Name){
        foreach (SpellInfo x in AllSpellInfo) {
            if (x.Name.Equals(Name)) {
                return x;
            }
        }
        Debug.LogError("Info Requested - Did Not Find: " + Name);
        return null;
    }
}
