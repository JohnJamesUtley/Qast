using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellBookUI : MonoBehaviour
{
    SpellInfo[] Info;
    public GameObject[] SpellBookViewers;
    public Text SpellName;
    public Image SpellGlyph;
    public Text SpellDescript;
    void Start() {
        Info = GameObject.Find("SpellManager").GetComponent<SpellInfoAccess>().AllSpellInfo;
        for(int i = 0; i < Info.Length; i++) {
            SpellBookViewers[i].transform.GetChild(0).GetComponent<Text>().text = Info[i].Name;
            SpellBookViewers[i].transform.GetChild(1).GetComponent<Image>().sprite = Info[i].Glyph;
        }
    }
    public void SpellBookViewSpell(int ID) {
        SpellName.text = Info[ID].Name;
        SpellGlyph.sprite = Info[ID].Glyph;
        SpellDescript.text = Info[ID].Description;
    }
}
