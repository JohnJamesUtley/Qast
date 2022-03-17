using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class SpellManager : MonoBehaviour{
	public GameObject[] Prefabs;
	public Texture2D[] SpellCasts;
	public AnimationCurve[] Curves;
	BaseSpell[] BasicSpells;
    List<BaseSpell> LoadedSpells;
	public Sprite[] Glyphs;
    void Awake(){
		BaseSpell[] ToLoadSpells = {
			new BaseSpell (new Blast (0.5f, Prefabs[0], 0.7f, "Blast", 0.5f, Prefabs [1]), SpellCasts[0], 0.95f, Glyphs[0]),
			new BaseSpell (new Lightning (1, Prefabs[0], 0.7f, "Lightning", 2, Prefabs [2], Prefabs[3], Curves[0], Curves[1]), SpellCasts[1], 1, Glyphs[1]),
			new BaseSpell (new Deflect (0.1f, Prefabs[0], 0.675f, "Deflect", 0.1f, Prefabs [4]), SpellCasts[2], 1, Glyphs[2]),
			new BaseSpell (new Cloak (0.1f, Prefabs[0], 0.75f, "Cloak", 15), SpellCasts[3], 1, Glyphs[3]),
			new BaseSpell (new Blink (0.5f, Prefabs[0], 0.7f, "Blink", 0.5f,Prefabs[5]), SpellCasts[4], 0.925f, Glyphs[4]),
			new BaseSpell (new Snap (1f, Prefabs[0], 0.75f, "Snap", 10, Prefabs[6], Prefabs[5]), SpellCasts[5], 1, Glyphs[5]),
			new BaseSpell (new Push (0.35f, Prefabs[0], 0.675f, "Push", 3, Prefabs[7]), SpellCasts[6], 1, Glyphs[6]),
			new BaseSpell (new Shatter (1f, Prefabs[0], 0.725f, "Shatter", 4, Prefabs[8]), SpellCasts[7], 1, Glyphs[7]),
			new BaseSpell (new Proxy (0.1f, Prefabs[0], 0.7f, "Proxy", 25f, Prefabs [9]), SpellCasts[8], 1, Glyphs[9]),
            new BaseSpell (new Knock (1f, Prefabs[0], 0.7f, "Knock", 15f, Prefabs [10]), SpellCasts[9], 1, Glyphs[10]),
            new BaseSpell (new Minion (1f, Prefabs[0], 0.7f, "Minion", 20f, Prefabs [11], Prefabs[12]), SpellCasts[10], 1, Glyphs[11]),
            new BaseSpell (new Douse (0.35f, Prefabs[0], 0.675f, "Douse", 3, Prefabs[13]), SpellCasts[11], 1, Glyphs[12]),
            new BaseSpell (new Slow (0.35f, Prefabs[0], 0.675f, "Slow", 15f, Prefabs[14]), SpellCasts[12], 1, Glyphs[13]),
        };
		BasicSpells = ToLoadSpells;
	}
	public Sprite GetGlyph(string Name){
        if (Name.Equals("Empty"))
            return Glyphs[8];
		foreach (BaseSpell x in BasicSpells) {
			if (x.Basic.Name.Equals (Name))
				return x.Glyph;
		}
        Debug.LogError(Name + ": Glyph Not Found ");
		return null;
	}
    public void LoadSpells(string[] ToBeLoaded) {
		Debug.Log("Spells Loaded");
        LoadedSpells = new List<BaseSpell>();
        foreach(string x in ToBeLoaded) {
            foreach (BaseSpell y in BasicSpells) {
                if (y.Basic.Name.Equals(x))
                    LoadedSpells.Add(y);
            }
        }
    }
    public void LoadSpell(string ToBeLoaded) {
        foreach (BaseSpell x in BasicSpells) {
            if (x.Basic.Name.Equals(ToBeLoaded))
                LoadedSpells.Add(x);
        }
    }
    public void UnloadSpell(string ToBeUnloaded) {
        foreach (BaseSpell x in LoadedSpells) {
            if (x.Basic.Name.Equals(ToBeUnloaded)) {
                LoadedSpells.Remove(x);
                break;
            }
        }
    }
    public Spell RetrieveSpell(string Name){
		foreach (BaseSpell x in BasicSpells) {
			if(x.Basic.Name.Equals(Name)){
				return x.Basic;
			}
		}
		Debug.LogError ("Spell Not Found");
		return null;
	}
	public CastingInfo PlaceCast(Texture2D Test){
		float HighestScore = 0;
		int HighestScorer = 0;
        for (int i = 0; i < LoadedSpells.Count; i++) {
			float ThisScore = TestMatch (LoadedSpells [i].SpellCast, Test) * LoadedSpells [i].Mod;
			if(ThisScore > HighestScore){
				HighestScore = ThisScore;
				HighestScorer = i;
			}
		}
		return new CastingInfo(LoadedSpells [HighestScorer].Basic.Name,HighestScore + (1 - LoadedSpells [HighestScorer].Mod));
	}
	float TestMatch(Texture2D Cast, Texture2D Test){
		Color[] CastColors = Cast.GetPixels ();
		Color[] TestColors = Test.GetPixels ();
		Vector2 CastAverage = Vector2.zero;
		Vector2 TestAverage = Vector2.zero;
		int BlackInCast = 0;
		int BlackInTest = 0;
		for (int y = 0; y < Cast.height; y++) {
			for (int x = 0; x < Cast.width; x++) {
				if (CastColors [y * Cast.height + x] == Color.black) {
					CastAverage.x += x;
					CastAverage.y += y;
					BlackInCast++;
				}
			}
		}
        for (int y = 0; y < Test.height; y++) {
            for (int x = 0; x < Test.width; x++) {
                if (TestColors[y * Test.height + x] == Color.black) {
                    TestAverage.x += x;
                    TestAverage.y += y;
                    BlackInTest++;
                }
            }
        }
        CastAverage = CastAverage / BlackInCast;
		TestAverage = TestAverage / BlackInTest;
		Vector2 Change = new Vector2((int)TestAverage.x - (int)CastAverage.x,(int)TestAverage.y - (int)CastAverage.y);
        // Add To Cast to Turn Test
        // Subtract to Test to Turn Cast
		BlackInCast = 0;
		int BlackInCastPixelsUsed = 0;
		float BlackCastCovered = 0;
		BlackInTest = 0;
		float BlackTestCovered = 0;
		//Cast Is Covered
		for (int y = 0; y < Cast.height; y++) {
			for (int x = 0; x < Cast.width; x++) {
				if (CastColors [y * Cast.width + x] == Color.black) {
					BlackInCast++;
				}
			}
		}
		bool CompleteBreak = false;
		for(float i = 0.9f; i > 0; i -= 0.1f){
			for (int y = 0; y < Cast.height; y++) {
				for (int x = 0; x < Cast.width; x++) {
					float H, S, V;
					Color.RGBToHSV (CastColors [y * Cast.width + x], out H, out S, out V);
					if(Inbounds(Test,new Vector2((int)(x + Change.x),(int)(y + Change.y)))){
						if ((1 - V) < i + 0.11f && (1 - V) > i) {
							if (TestColors [(int)(y + Change.y) * Test.width + (int)(x + Change.x)] == Color.black) {
								//Debug.Log (1 - V);
								BlackCastCovered += (1 - V);
								BlackInCastPixelsUsed++;
								if (BlackInCastPixelsUsed >= BlackInCast) {
									CompleteBreak = true;
									break;
								}
							}
						}
					}
				}
				if (CompleteBreak)
					break;
			}
			if (CompleteBreak)
				break;
		}
		//Misplaced Test Pixels
		for (int y = 0; y < Test.height; y++) {
			for (int x = 0; x < Test.width; x++) {
				if (Inbounds (Cast, new Vector2((int)(x - Change.x),(int)(y - Change.y)))) {
					if (TestColors [(int)(y) * Test.width + (int)(x)] == Color.black) {
						BlackInTest++;
						float H, S, V;
						Color.RGBToHSV (CastColors [(int)(y - Change.y) * Cast.width + (int)(x - Change.x)], out H, out S, out V);
						BlackTestCovered += (1 - V);
					}
				}
			}
		}
		float PercentTest = ((float)BlackTestCovered / (float)BlackInTest);
		float PercentCast = ((float)BlackCastCovered / (float)BlackInCast);
        return (PercentTest + PercentCast) / 2f;
		//return PercentCast;
	}
	bool Inbounds(Texture2D Cast, Vector2 ToTest){
		if ((int)(ToTest.y) < Cast.height && (int)(ToTest.y) >= 0 && (int)(ToTest.x) < Cast.width && (int)(ToTest.x) >= 0) {
			return true;
		}
		return false;
	}
    public void RemoveSpell(string ToRemove) {
        foreach (BaseSpell x in LoadedSpells) {
            if (x.Basic.Name.Equals(ToRemove)) {
                LoadedSpells.Remove(x);
                break;
            }
        }
    }

    public void DebugCast(Texture2D ToDebug, int Spacing) {
        Color[] Pixels = ToDebug.GetPixels();
        string Line = "";
        for (int y = 0; y < ToDebug.height; y += Spacing) {
            for (int i = 0; i < ToDebug.width; i += Spacing) {
                Line += (int)(Pixels[y * ToDebug.width + i].r + 0.5f) + " ";
            }
            Line += "\n";
        }
        Debug.Log(Line);
    }
}
