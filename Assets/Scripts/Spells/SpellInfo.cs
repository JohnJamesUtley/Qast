using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class SpellInfo {
    public Sprite Glyph;
    [TextArea(6, 8)]
    public string Description;
    public string Name;
    public SpellInfo(string Name, string Description, Sprite Glyph) {
        this.Name = Name;
        this.Description = Description;
        this.Glyph = Glyph;
    }
}

