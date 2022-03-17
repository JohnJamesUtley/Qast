using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class PlayerWire : NetworkBehaviour {
	public string PlayerID;
	SceneManager SceneManager;
	SpellManager SpellManager;
    AudioManager AudioManager;
    UIManager GameUI;
    PlayerUI LocalUI;
    Animator Spiral;
	public GameObject WiredWizard;
	Wizard WizardScript;
	public GameObject WizardPrefab;
	Vector2 MovementDirection = Vector2.zero;
    bool Casting;
	float Speed = 0;
	public bool IsDead;
	public int SkinID;
	public float SpellWait = 0;
	public bool IsWaiting;
    public bool Ready;
	public int RoundsWon;
    public string[] Deck;
    public int[] DeckLayout;
    public int[] SpellsRemaining;
    public float[] SpellCooldowns;
    PreGameOptions Options;
	void Start(){
        AudioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        Options = GameObject.Find("PreGameManager").GetComponent<PreGameOptions>();
        SpellManager = GameObject.Find ("SpellManager").GetComponent<SpellManager> ();
        LocalUI = GameObject.Find("UIManager").GetComponent<PlayerUI>();
		IsDead = true;
		IsWaiting = false;
		if (isLocalPlayer) {
			if (isServer) {
				if (Options.ServerPlayer) {
					SetPlayerID (Options.ServerPlayerName);
				}
			} else {
				SetPlayerID ();
			}
		}
		if(isServer){
			SceneManager = GameObject.Find ("SceneManager").GetComponent<SceneManager> ();
            GameUI = GameObject.Find("SceneManager").GetComponent<UIManager>();
			if(!isLocalPlayer || SceneManager.ServerPlayer){
				SceneManager.Players.Add(this);
				int TakenID = Random.Range (0, SceneManager.SkinIDAvaliable.Count);
				SkinID = SceneManager.SkinIDAvaliable [TakenID];
				SceneManager.SkinIDAvaliable.RemoveAt (TakenID);
                if (SceneManager.ServerPlayer && isLocalPlayer) {
                    this.Deck = SceneManager.StandardDeck;
                    this.DeckLayout = SceneManager.StandardLayout;
                }
				SceneManager.AddPlayerLobby(this);
			}
			
            //if(SceneManager.CurrentStage.Equals("Lobby"))
			//SceneManager.AddPlayerLobby(this);
		}
		if (isLocalPlayer) {
			Spiral = GameObject.Find ("SpiralGlow").GetComponent<Animator>();
            LocalUI.LocalPlayerWire = this;
            if (!isServer || Options.ServerPlayerLobby) {
                LocalUI.SpellInfoAccess = GameObject.Find("SpellManager").GetComponent<SpellInfoAccess>();
                LocalUI.SwitchPlayerLobbyUI();
            }
		}
	}
	void Update(){
		if(IsWaiting && isServer){
			SpellWait -= Time.deltaTime;
			if (SpellWait <= 0) {
				IsWaiting = false;
				RpcSetWait (false);
				if(WizardScript.WandLight != null)
					WizardScript.WandLight.SetTrigger ("Light");
			}
		}
        for (int i = 0; i < SpellCooldowns.Length; i++) {
            if (SpellCooldowns[i] > 0) {
                SpellCooldowns[i] -= Time.deltaTime;
				GameUI.UpdateCooldown (this,i,SpellCooldowns[i]);
                if (SpellCooldowns[i] <= 0) {
                    SpellCooldowns[i] = 0;
					GameUI.DisableCooldown (this, i);
                    if(SpellsRemaining[i] != 0)
                    	RpcLoadSpell(Deck[i]);
                }
            }
        }
    }
	public void SetPlayerID(){
		CmdSetPlayerID (LocalUI.EnteredID());
		CmdUpdateLobbyIDs ();
	}
	public void SetPlayerID(string ID){
		CmdSetPlayerID (ID);
	}
	public void SpawnWizard(){
		WiredWizard = GameObject.Instantiate (WizardPrefab);
		WizardScript = WiredWizard.GetComponent<Wizard> ();
		WizardScript.Wire = gameObject;
		SceneManager.Wizards.Add (WiredWizard);
		SceneManager.Targets.Add (WiredWizard);
		WiredWizard.GetComponentInChildren<SpriteRenderer> ().sprite = SceneManager.Skins [SkinID];
		RpcLoadSpells ();
        GrabSpells();
        ResetSpellCooldowns();
	}
    void ResetSpellCooldowns() {
        for(int i = 0; i < SpellCooldowns.Length; i++) {
            SpellCooldowns[i] = 0;
        }
    }
    public void CreateCooldown(string Spell, float Cooldown) {
        RpcUnloadSpell(Spell);
        for (int i = 0; i < Deck.Length; i++) {
			if (Deck[i].Equals(Spell)) {
                SpellCooldowns[i] += Cooldown;
				if(SpellsRemaining[i] > 1 || SpellsRemaining[i] == -1)
					GameUI.EnableCooldown (this, Cooldown, i);
            }
        }
    }

    public void Activate(){
		IsDead = false;
		RpcSetDead (false);
	}
	void FixedUpdate(){
		if (isServer && !IsDead) {
			MoveWizard ();
			WizardScript.ActiveSpellEffects ();
		}
	}
	void MoveWizard(){
		WizardScript.Speed = Speed;
		WizardScript.MovementDirection = MovementDirection;
        WizardScript.Casting = Casting;
		WizardScript.Move ();
	}
    public void GrabSpells() {
        for(int i = 0; i < DeckLayout.Length; i++) {
            SpellsRemaining[i] = DeckLayout[i];
        }
    }
    public void SubtractInventory(string Spell) {
        for (int i = 0; i < Deck.Length; i++) {
            if (Deck[i].Equals(Spell)) {
                if (SpellsRemaining[i] > 0) {
                    SpellsRemaining[i]--;
                    GameUI.SetTopBarSpellCount(this, i, SpellsRemaining[i]);
                }
                if(SpellsRemaining[i] == 0) {
                    RpcRemoveSpell(Spell);
                }
                break;
            }
        }
    }
    public override bool Equals(object other) {
        PlayerWire RealOther = (PlayerWire)other;
        if (RealOther.SkinID == this.SkinID)
            return true;
        return false;
    }
    [ClientRpc]
    public void RpcRemoveSpell(string Spell) {
        if (isLocalPlayer)
            SpellManager.RemoveSpell(Spell);
    }
    [ClientRpc]
	public void RpcLoadSpells(){
		if(isLocalPlayer)
			SpellManager.LoadSpells (Deck);
	}
    [ClientRpc]
    public void RpcUnloadSpell(string Spell) {
        if (isLocalPlayer)
            SpellManager.UnloadSpell(Spell);
    }
    [ClientRpc]
    public void RpcLoadSpell(string Spell) {
        if (isLocalPlayer)
            SpellManager.LoadSpell(Spell);
    }
    [ClientRpc]
    public void RpcSwitchPlayerUI() {
        if(isLocalPlayer)
            LocalUI.SwitchPlayerUI();
    }
    [ClientRpc]
    public void RpcSwitchPlayerLobbyUI() {
		if (isLocalPlayer && !isServer)
            LocalUI.SwitchPlayerLobbyUI();
    }
    [ClientRpc]
	public void RpcSetDead (bool IsDead){
		this.IsDead = IsDead;
		if (isLocalPlayer && !isServer) {
			if (IsDead) {
				LocalUI.SwitchPlayerEventUI ();
			} else
				LocalUI.SwitchPlayerUI ();
		}
	}
	[ClientRpc]
	public void RpcSetWait (bool IsWaiting){
		this.IsWaiting = IsWaiting;
		if (IsWaiting == false && isLocalPlayer) {
			gameObject.GetComponent<TouchManager> ().ResetCasting = true;
			Spiral.SetTrigger ("Light");
		}
	}
    [ClientRpc]
    public void RpcSetReady(bool Value) {
        if(isLocalPlayer)
            LocalUI.ToggleReady(Value);
    }
	[ClientRpc]
	public void RpcRecieveEvent(string Name, int Cooldown, int TimeCost) {
		if (isLocalPlayer) {
			LocalUI.Events.Add (Name);
			LocalUI.Cooldowns.Add (Cooldown);
			LocalUI.TimeCosts.Add (TimeCost);
			LocalUI.RealTimeCosts.Add (TimeCost);
		}
	}
	[ClientRpc]
	public void RpcSetEventRan(string Name){
		if (isLocalPlayer)
			LocalUI.EventRan (Name);
	}
	[ClientRpc]
	public void RpcSetupEvents(){
		if (isLocalPlayer)
			LocalUI.SetupEvents ();
	}
	[ClientRpc]
	public void RpcAdjustTimeCosts(int NumDead){
		if (isLocalPlayer)
			LocalUI.UpdateTimeCosts (NumDead);
	}
	[Command]
	public void CmdRunEvent(string EventName) {
		MapInfo Map = SceneManager.CurrentMap.GetComponent<MapInfo> ();
		foreach (Event x in Map.Events) {
			if (x.Name == EventName)
				x.CallEvent ();
		}
	}
	[Command]
	public void CmdSetPlayerID(string ID) {
		PlayerID = ID;
	}
	[Command]
	public void CmdUpdateLobbyIDs() {
		GameUI.UpdateLobbyIDs ();
	}
	[Command]
	public void CmdGetEvents() {
		MapInfo Map = SceneManager.CurrentMap.GetComponent<MapInfo> ();
		foreach (Event x in Map.Events)
			RpcRecieveEvent (x.Name,x.Cooldown,x.TimeCost);
	}
    [Command]
    public void CmdSetCasting(bool Casting) {
        this.Casting = Casting;
    }
    [Command]
    public void CmdSetDeckLayoutPos(int Value, int Pos) {
        DeckLayout[Pos] = Value;
    }
    [Command]
    public void CmdSetSpellSlot(string Name, int Slot) {
        Deck[Slot] = Name;
    }
	[Command]
	public void CmdSetMovement (Vector2 Direction, float Speed){
		this.Speed = Speed;
		this.MovementDirection = Direction;
	}
	[Command]
	public void CmdCastSpell (string SpellType,float Accuracy,float Speed){
		Spell Basic = SpellManager.RetrieveSpell (SpellType);
		Spell ToCast = Basic.Copy ();
		ToCast.SetInstance (Accuracy,Speed);
		WizardScript.CastSpell (ToCast);
	}
    [Command]
    public void CmdUpdateLobbyDeck() {
        GameUI.UpdateLobbyDeck(this);
    }
    [Command]
    public void CmdSetReady(bool Value) {
        Ready = Value;
        GameUI.UpdateLobbyReady(this);
        if(Value)
            AudioManager.PlayAudio("Click", 0.7f , 1.5f);
        else
            AudioManager.PlayAudio("Click", 0.7f, 0.5f);
    }
}
