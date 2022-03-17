using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseSpell {
	public Spell Basic;
	public Texture2D SpellCast;
	public float Mod;
	public Sprite Glyph;
	public BaseSpell(Spell Basic, Texture2D SpellCast, float Mod, Sprite Glyph){
		this.Basic = Basic;
		this.SpellCast = SpellCast;
		this.Mod = Mod;
		this.Glyph = Glyph;
	}
}
