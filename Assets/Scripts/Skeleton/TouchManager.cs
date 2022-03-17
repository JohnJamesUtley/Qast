using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchManager : MonoBehaviour
{
	PlayerWire Wire;
	SpellManager Spells;
	GameObject Joystick;
	GameObject Wiggily;
	public GameObject JoystickPrefab;
	public GameObject TouchEffectPrefab;
	Texture2D CastingTexture;
	bool Casting;
	Vector2 JoystickPos;
	bool SetJoystick = false;
	bool Controls;
	float TimeForSpell;
	List<GameObject> TouchEffects;
	List<Vector2> Brush;
	SceneManager SceneManager;
	public bool ResetCasting;
	void Start ()
	{
		ResetCasting = false;
		TimeForSpell = 0;
		Wire = gameObject.GetComponent<PlayerWire> ();
		if (Wire.isServer) {
			SceneManager = GameObject.Find ("SceneManager").GetComponent<SceneManager> ();
		}
		Spells = GameObject.Find ("SpellManager").GetComponent<SpellManager> ();
		TouchEffects = new List<GameObject> ();
		Brush = new List<Vector2> ();
		for (int y = -10; y < 11; y++) {
			for (int x = -10; x < 11; x++) {
				if (Vector2.Distance (Vector2.zero, new Vector2 (x, y)) <= 7) {
					Brush.Add (new Vector2 (x, y));
				}
			}
		}
		Controls = true;
	}

	void Update ()
	{
		float Speed = 0;
		Vector2 Direction = Vector2.zero;
		if (Wire.isLocalPlayer && !Wire.IsDead) {
			if (Wire.isServer) {
				if (!SceneManager.ServerPlayer) {
					Controls = false;
				}
			}
			if (Controls) {
				List<Vector2> MagicTouches = new List<Vector2> ();
				Vector2 JoystickTouch = new Vector2 (0, 0);
				bool JoystickTouched = false;
				int JoystickArea = Screen.width / 3;
				//Debug.Log (Input.touchCount);
				for (int i = 0; i < Input.touchCount; i++) {
					Touch touch = Input.GetTouch (i);
					if (touch.position.x < JoystickArea) {
						JoystickTouch = touch.position;
						JoystickTouched = true;
					} else {
						MagicTouches.Add (touch.position);
					}
				}
				if (Input.GetMouseButton (0) && MagicTouches.Count == 0) {
					//if (Input.mousePosition.x < JoystickArea || Input.mousePosition.y > Screen.width - JoystickArea) {
                    if (Input.mousePosition.x < JoystickArea) {
                            JoystickTouched = true;
						JoystickTouch = Input.mousePosition;
					} else {
						MagicTouches.Add (Input.mousePosition);
					}
				}
				JoystickTouch = Camera.main.ScreenToWorldPoint (JoystickTouch);
				if (JoystickTouched) {
					if (!SetJoystick) {
						SetJoystick = true;
						Joystick = GameObject.Instantiate (JoystickPrefab);
						float JoystickAreaWorld = -Camera.main.ScreenToWorldPoint (new Vector2 (JoystickArea / 2, JoystickArea / 2)).x;
						Joystick.transform.localScale = new Vector2 (JoystickAreaWorld, JoystickAreaWorld);
						Wiggily = Joystick.transform.GetChild (0).gameObject;
						Joystick.transform.position = new Vector3 (JoystickTouch.x, JoystickTouch.y, 0);
						JoystickPos = JoystickTouch;
					} else {
						Vector2 JoystickDifferance = JoystickTouch - JoystickPos;
						float MaxReach = Joystick.transform.localScale.x / 2 - Wiggily.transform.localScale.x * Joystick.transform.localScale.x / 2;
						Vector2 Change = Vector2.ClampMagnitude (JoystickDifferance, MaxReach);
						if (Vector2.Distance (Vector2.zero, JoystickDifferance) > MaxReach) {
							JoystickTouch = JoystickPos + Change;
						}
						Wiggily.transform.position = new Vector3 (JoystickTouch.x, JoystickTouch.y, -1);
						Direction = Change * (1 / MaxReach);
						Speed = Vector2.Distance (Vector2.zero, Direction);
						Direction = Vector2.ClampMagnitude (Direction * 100, 1);
						Wire.CmdSetMovement (Direction, Speed);
					}
				} else {
					if (SetJoystick) {
						SetJoystick = false;
						GameObject.Destroy (Joystick);
						Wire.CmdSetMovement (Direction, Speed);
					}
				}
				if (!Wire.IsWaiting) {
					if (MagicTouches.Count > TouchEffects.Count) {
						int Differance = MagicTouches.Count - TouchEffects.Count;
						for (int i = 0; i < Differance; i++) {
							TouchEffects.Add (GameObject.Instantiate (TouchEffectPrefab));
						}
					} else if (MagicTouches.Count < TouchEffects.Count) {
						int Differance = TouchEffects.Count - MagicTouches.Count;
						for (int i = 0; i < Differance; i++) {
							GameObject ToDestroy = TouchEffects [i];
							TouchEffects.RemoveAt (i);
							GameObject.Destroy (ToDestroy);
						}
					}
					if (MagicTouches.Count > 0 && ResetCasting == false) {
						TimeForSpell += Time.deltaTime;
						if (!Casting) {
							Casting = true;
                            Wire.CmdSetCasting(true);
                            CastingTexture = new Texture2D (300, 300);
						}
						Color[] Pixels = CastingTexture.GetPixels ();
						for (int i = 0; i < MagicTouches.Count; i++) {
							Vector2 NewPosition = Camera.main.ScreenToWorldPoint (MagicTouches [i]);
							TouchEffects [i].transform.position = NewPosition;
							Vector2 Percent = Vector2.zero;
							Percent.x = (MagicTouches [i].x - (JoystickArea)) / (Screen.height);
							Percent.y = (MagicTouches[i].y / (Screen.height));
							Vector2 PixelPos = new Vector2 (Mathf.RoundToInt (Percent.x * 100), Mathf.RoundToInt (Percent.y * 100));
							for (int j = 0; j < Brush.Count; j++) {
								if ((int)(PixelPos.y + Brush [j].y) < CastingTexture.height && (int)(PixelPos.x + Brush [j].x) < CastingTexture.width)
								if ((int)(PixelPos.y + Brush [j].y) > 0 && (int)(PixelPos.x + Brush [j].x) > 0)
									Pixels [(int)(PixelPos.y + Brush [j].y) * CastingTexture.width + (int)(PixelPos.x + Brush [j].x)] = Color.black;
							}
						}
						CastingTexture.SetPixels (Pixels);
					} else if(MagicTouches.Count == 0){
						if (Casting && ResetCasting == false) {
							Casting = false;
                            Wire.CmdSetCasting(false);
                            SpellFromCast(Spells.PlaceCast (CastingTexture), TimeForSpell);
                            TimeForSpell = 0;
                        }
						if (ResetCasting == true) {
							ResetCasting = false;
							Casting = false;
                            Wire.CmdSetCasting(false);
						}
					}
					if (Input.GetKeyUp (KeyCode.V)) {
						Wire.CmdCastSpell ("Blast", Random.value * 0.1f + 0.9f, 1.3f);
					}
					if (Input.GetKeyUp (KeyCode.C)) {
						Wire.CmdCastSpell ("Lightning", Random.value * 0.1f + 0.9f, 1.3f);
					}
					if (Input.GetKeyUp (KeyCode.B)) {
						Wire.CmdCastSpell ("Deflect", Random.value * 0.1f + 0.9f, 1.3f);
					}
				}
			}
		} else {
            if (SetJoystick) {
                SetJoystick = false;
                GameObject.Destroy(Joystick);
                Wire.CmdSetMovement(Direction, Speed);
            }
		}
	}

	void SpellFromCast (CastingInfo Cast, float Speed)
	{
		Wire.CmdCastSpell (Cast.Name, Cast.Accuracy, Speed);
	}
}
