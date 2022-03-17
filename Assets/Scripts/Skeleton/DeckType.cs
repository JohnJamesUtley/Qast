using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class DeckType
{
    public int[] Layout;
    public string Name;
    public DeckType(int[] Layout, string Name) {
        this.Layout = Layout;
        this.Name = Name;
    }
}
