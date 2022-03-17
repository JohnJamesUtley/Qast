using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class SceneManager : NetworkBehaviour {
    public string CurrentStage;
    public List<GameObject> Targets;
    public List<GameObject> Wizards;
    public List<GameObject> Disposables;
    public List<PlayerWire> Players;
    public GameObject[] Maps;
    public MapSet[] MapSets;
    public Sprite[] Skins;
    public string[] SkinNames;
    public Color[] SkinColors;
    public List<int> SkinIDAvaliable;
    public bool ServerPlayer;
	public string[] StandardDeck;
    public int[] StandardLayout;
    public bool AutoRound;
    public List<Spell> LegacySpells;
    GameBehaviors Game; 
    UIManager UI;
    bool RoundEnding;
	bool Won;
    public GameObject CurrentMap;
    MapInfo CurrentInfo;
    List<GameObject> OpenSpawns;
    public PreGameOptions Options;
	public int DeadPlayers;
    public void RunStarted() {
        Options = GameObject.Find("PreGameManager").GetComponent<PreGameOptions>();
        Game = GameObject.Find("SceneManager").GetComponent<GameBehaviors>();
        ServerPlayer = Options.ServerPlayer;
		StandardDeck = Options.StandardDeck;
        StandardLayout = Options.StandardLayout;
        UI = gameObject.GetComponent<UIManager>();
        if(LegacySpells != null)
            foreach (Spell x in LegacySpells)
                x.Destroy();
        LegacySpells = new List<Spell>();
		UI.SceneUI.SwitchServerCodeHostingUI ();
        //StartLobby();
        //StartGame();
    }
    void Update() {
        if (Input.GetKeyUp(KeyCode.R))
            StartCoroutine("EndRound");
        if (Input.GetKeyUp(KeyCode.Q))
            ForceReturnLobby();
        if (Input.GetKeyUp(KeyCode.LeftShift))
            if (ServerPlayer)
                foreach (PlayerWire x in Players)
                    if (x.isLocalPlayer) {
                        x.Ready = !x.Ready;
                        x.CmdSetReady(x.Ready);
                    }
        if (Input.GetKeyUp(KeyCode.End)) {
            foreach (PlayerWire x in Players)
                if (x.isLocalPlayer) {
                    x.WiredWizard.GetComponent<Wizard>().Kill();
                }
        }
        if (Input.GetKeyUp(KeyCode.C)) {
            PlayerPrefs.DeleteAll();
        }
    }
    void FixedUpdate() {
        foreach (Spell x in LegacySpells)
            x.WhileActive();
    }
    public void ForceReturnLobby() {
        StartCoroutine("ForceQuit");
    }
    public void StartLobby() {
        CurrentStage = "Lobby";
        UI.SceneUI.SwitchLobbyUI();
        UI.UpdateMapSet();
    }
    public void AddPlayerLobby(PlayerWire Player) {
        UI.AddLobbyPlayer(Player);
    }
    public void StartGame() {
        StartCoroutine("ProperStart");
    }
    IEnumerator ProperStart() {
        UI.Transition();
        yield return new WaitForSeconds(0.7f);
        foreach (PlayerWire x in Players) {
            if (!x.isLocalPlayer) {
                x.RpcSwitchPlayerUI();
            }
        }
        CurrentStage = "Game";
        UI.SceneUI.SwitchGameUI();
        ClearRound();
        CreateRound();
    }
    IEnumerator ForceQuit() {
        RoundEnding = true;
        UI.Transition();
        yield return new WaitForSeconds(0.7f);
        ClearRound();
        ReturnLobby();
    }
    IEnumerator EndRound(){
		RoundEnding = true;
		Won = false;
		yield return new WaitForSeconds (3);
        PlayerWire Winner = null;
        if (Wizards.Count == 1) {
            Winner = Wizards [0].GetComponent<Wizard> ().Wire.GetComponent<PlayerWire> ();
			int SkinID = Winner.SkinID;
			UI.SetCenter (SkinNames[SkinID] + " wins!");
			UI.SetCenter (SkinColors [SkinID]);
			Winner.RoundsWon++;
			if (Winner.RoundsWon == 4)
				Won = true;
		} else {
			UI.SetCenter ("Tie!");
		}
		yield return new WaitForSeconds (0.7f);
		UI.Transition ();
		yield return new WaitForSeconds (0.9f);
		UI.SetCenter (Color.white);
		UI.SetCenter ("");
        ClearRound();
		if(!Won)
			CreateRound ();
		else 
			StartCoroutine("WinGame", Winner);
	}
    public void ClearRound() {
        List<GameObject> ToKill = CopyList<GameObject>(Wizards);
        foreach (GameObject x in ToKill)
            x.GetComponent<Wizard>().Kill();
        List<GameObject> ToDestroy = CopyList<GameObject>(Disposables);
        foreach (GameObject x in ToDestroy)
            GameObject.Destroy(x);
        Disposables.Clear();
        UI.ClearTopBar();
        LegacySpells.Clear();
        Game.StopAudio();
    }
    public void EliminateWizard(GameObject Wizard){
		DeadPlayers++;
		if (DeadPlayers >= 1)
			CurrentMap.GetComponent<MapInfo> ().EventAuthority = false;
		Targets.Remove (Wizard);
		Wizards.Remove (Wizard);
		UI.CrossTopBar (Wizard.GetComponent<Wizard> ().Wire.GetComponent<PlayerWire>());
		foreach (PlayerWire x in Players)
			x.RpcAdjustTimeCosts (DeadPlayers);
		if (AutoRound) {
			if ((Wizards.Count <= 1) && !RoundEnding) {
                if(!Options.NoEnd)
				    StartCoroutine ("EndRound");
			}
		}
	}
	public void CreateRound(){
		LoadMap (Random.Range (0, Maps.Length));
		List<GameObject> OpenSpawns = new List<GameObject> ();
		DeadPlayers = 0;
		foreach (GameObject x in CurrentInfo.Spawns)
			OpenSpawns.Add (x);
		foreach (PlayerWire x in Players) {
			x.SpawnWizard ();
			int SpawnNum = Random.Range (0, OpenSpawns.Count);
			x.WiredWizard.transform.position = OpenSpawns [SpawnNum].transform.position;
			x.WiredWizard.transform.rotation = OpenSpawns [SpawnNum].transform.rotation;
			OpenSpawns.RemoveAt (SpawnNum);
			UI.AddTopBar (x);
			x.RpcSetupEvents ();
		}
		StartCoroutine ("StartRound");
	}
	IEnumerator StartRound(){
		yield return new WaitForSeconds (0.5f);
		UI.SetCenter ("3");
        Game.PlayAudio("Click", 0.3f, 0f , 1f);
		yield return new WaitForSeconds (1);
		UI.SetCenter ("2");
        Game.PlayAudio("Click", 0.3f, 0f, 0.8f);
        yield return new WaitForSeconds (1);
		UI.SetCenter ("1");
        Game.PlayAudio("Click", 0.3f, 0f, 0.6f);
        yield return new WaitForSeconds (1);
		UI.SetCenter ("Fight!");
        Game.PlayAudio("Click", 0.3f, 0f, 1f);
        foreach (PlayerWire x in Players) {
			x.Activate ();
		}
		yield return new WaitForSeconds (0.5f);
		UI.SetCenter ("");
		RoundEnding = false;
	}
	List<T> CopyList<T>(List<T> Stuff){
		List<T> Copy = new List<T> ();
		foreach (T x in Stuff)
			Copy.Add (x);
		return Copy;
	}
	IEnumerator WinGame(PlayerWire Winner){
        UI.SetCenter(SkinColors[Winner.SkinID]);
        UI.SetCenter(SkinNames[Winner.SkinID] + " won the game!");
        yield return new WaitForSeconds(3.5f);
        UI.SetCenter("");
        yield return new WaitForSeconds(1f);
        StartCoroutine("ForceQuit");
	}
    void ReturnLobby() {
        UI.SceneUI.SwitchLobbyUI();
        foreach (PlayerWire x in Players) {
            x.RoundsWon = 0;
            x.IsDead = true;
            x.RpcSetDead(true);
            x.RpcSwitchPlayerLobbyUI();
            UI.RegisterPlayer(x);
            x.RpcSetReady(false);
        }
    }
	public void LoadMap(int Map){
		ClearMap ();
		CurrentMap = GameObject.Instantiate (Maps [Map]);
		CurrentInfo = CurrentMap.GetComponent<MapInfo> ();
		Camera.main.orthographicSize = CurrentInfo.CameraSize;
		CurrentMap.transform.position = new Vector3(0,CurrentInfo.YOffset,0);
	}
	void ClearMap(){
		if(CurrentMap != null) 
			GameObject.Destroy (CurrentMap);
	}
}
