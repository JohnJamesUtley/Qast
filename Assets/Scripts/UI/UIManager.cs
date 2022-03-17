using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIManager : MonoBehaviour {
	SceneManager SceneManager;
	SpellManager SpellManager;
    GameBehaviors Game;
    public Text CenterText;
	public Animator TransitionUI;
	public GameObject[] PlayerBoxes;
    public GameObject[] PlayerInfos;
    public List<PlayerWire> Players;
    public Text ReadyCount;
	public Text[] CodeTexts;
    public bool AllReady = false;
    public Text MapSet;
    public Image MapImg;
	int ActiveBoxes;
    int ActiveInfos;
    public PlayerUI SceneUI;
    public void Awake(){
        SceneManager = GameObject.Find ("SceneManager").GetComponent<SceneManager> ();
		SpellManager = GameObject.Find ("SpellManager").GetComponent<SpellManager> ();
        Game = GameObject.Find("SceneManager").GetComponent<GameBehaviors>();
        SceneUI = GameObject.Find("UIManager").GetComponent<PlayerUI>();
        ClearLobbyPlayers();
    }
    public void ClearTopBar(){
		foreach (GameObject x in PlayerBoxes) {
			for (int i = 0; i < 3; i++)
				x.transform.GetChild (1).GetChild (i).gameObject.SetActive (false);
			for (int i = 0; i < 8; i++)
				x.transform.GetChild (3).GetChild (i).GetChild (2).gameObject.GetComponent<CooldownTimer> ().Initialize();
			for (int i = 0; i < 8; i++)
				x.transform.GetChild (3).GetChild (i).gameObject.SetActive (false);
			x.SetActive (false);
		}
		ActiveBoxes = 0;
		Players = new List<PlayerWire> ();
	}
	public void AddTopBar(PlayerWire Player){
        RegisterPlayer(Player);
        PlayerBoxes [ActiveBoxes].SetActive (true);
		PlayerBoxes [ActiveBoxes].transform.GetChild (0).GetChild (0).GetComponentInChildren<Image>().sprite = SceneManager.Skins[Player.SkinID];
		PlayerBoxes [ActiveBoxes].transform.GetChild (2).gameObject.GetComponent<Text> ().text = Player.PlayerID;
		PlayerBoxes[ActiveBoxes].transform.GetChild(2).gameObject.GetComponent<Text>().color = SceneManager.SkinColors[Player.SkinID];
		for (int i = 0; i < Player.RoundsWon; i++) {
			if(i < 3)
				PlayerBoxes [ActiveBoxes].transform.GetChild (1).GetChild (i).gameObject.SetActive (true);
		}
        int Pos = 0;
        for (int i = 0; i < Player.Deck.Length; i++) {
            if (!Player.Deck[i].Equals("")) {
                  GameObject Glyph = PlayerBoxes[ActiveBoxes].transform.GetChild(3).GetChild(Pos).gameObject;
                  Glyph.SetActive(true);
                  Glyph.transform.GetChild(1).gameObject.SetActive(false);
                if (Player.DeckLayout[i] != -1)
                    Glyph.transform.GetChild(0).GetComponent<Text>().text = Player.DeckLayout[i].ToString();
                else
                    Glyph.transform.GetChild(0).GetComponent<Text>().text = "\u221E";
                  Glyph.GetComponent<Image>().sprite = SpellManager.GetGlyph(Player.Deck[i]);
                  Pos++;
            }
		}
		ActiveBoxes++;
	}
    public void SetTopBarSpellCount(PlayerWire Player, int DeckPos, int Count) {
        int PlayerPos = FindPlayerLobbyPos(Player);
        GameObject Glyph = PlayerBoxes[PlayerPos].transform.GetChild(3).GetChild(DeckPos).gameObject;
        if (Count != -1)
            Glyph.transform.GetChild(0).GetComponent<Text>().text = Count.ToString();
        else
            Glyph.transform.GetChild(0).GetComponent<Text>().text = "\u221E";

        if(Count == 0)
            Glyph.transform.GetChild(1).gameObject.SetActive(true);
    }
    public void ClearLobbyPlayers() {
		foreach (GameObject x in PlayerInfos) {
			x.SetActive (false);
		}
        ActiveInfos = 0;
        Players = new List<PlayerWire>();
    }
	public void EnableCooldown (PlayerWire Player, float Time, int Index){
		int PlayerPos = FindPlayerLobbyPos (Player);
		PlayerBoxes [PlayerPos].transform.GetChild (3).GetChild (Index).GetChild (2).gameObject.GetComponent<CooldownTimer> ().Enable (Time);
	}
	public void DisableCooldown(PlayerWire Player, int Index){
		int PlayerPos = FindPlayerLobbyPos (Player);
		PlayerBoxes [PlayerPos].transform.GetChild (3).GetChild (Index).GetChild (2).gameObject.GetComponent<CooldownTimer> ().Disable ();
	}
	public void UpdateCooldown(PlayerWire Player, int Index, float Time){
		int PlayerPos = FindPlayerLobbyPos (Player);
		PlayerBoxes [PlayerPos].transform.GetChild (3).GetChild (Index).GetChild (2).gameObject.GetComponent<CooldownTimer> ().UpdatePercent (Time);
	}
    public void AddLobbyPlayer(PlayerWire Player) {
        RegisterPlayer(Player);
        PlayerInfos[ActiveInfos].SetActive(true);
        PlayerInfos[ActiveInfos].transform.GetChild(0).GetChild(0).GetComponentInChildren<Image>().sprite = SceneManager.Skins[Player.SkinID];
        PlayerInfos[ActiveInfos].transform.GetChild(1).gameObject.GetComponent<Text>().color = SceneManager.SkinColors[Player.SkinID];
		PlayerInfos[ActiveInfos].transform.GetChild(1).gameObject.GetComponent<Text>().text = Player.PlayerID;
        UpdateLobbyDeck(Player);
        UpdateLobbyReady(Player);
        ActiveInfos++;
    }
	public void UpdateLobbyIDs(){
		for(int i = 0; i < Players.Count; i++)
			PlayerInfos[i].transform.GetChild(1).gameObject.GetComponent<Text>().text = Players[i].PlayerID;

	}
    public void UpdateLobbyDeck(PlayerWire Player) {
        int PlayerPos = FindPlayerLobbyPos(Player);
        for (int i = 0; i < 8; i++) {
            PlayerInfos[PlayerPos].transform.GetChild(2).GetChild(i).gameObject.SetActive(false);
        }
        for (int i = 0; i < Player.Deck.Length; i++) {
            if (Player.DeckLayout[i] != 0) {
                GameObject Glyph = PlayerInfos[PlayerPos].transform.GetChild(2).GetChild(i).gameObject;
                Glyph.SetActive(true);
                if (!Player.Deck[i].Equals(""))
                    Glyph.GetComponent<Image>().sprite = SpellManager.GetGlyph(Player.Deck[i]);
                else
                    Glyph.GetComponent<Image>().sprite = SpellManager.GetGlyph("Empty");
                int Count = Player.DeckLayout[i];
                if (Count != -1)
                    Glyph.transform.GetChild(0).GetComponent<Text>().text = Count.ToString();
                else
                    Glyph.transform.GetChild(0).GetComponent<Text>().text = "\u221E";
            }
        }
    }
    public void UpdateLobbyReady(PlayerWire Player) {
        PlayerInfos[FindPlayerLobbyPos(Player)].transform.GetChild(3).gameObject.SetActive(Player.Ready);
        UpdateReadyCount();
    }
    void UpdateReadyCount() {
        int ReadyTotal = 0;
        foreach (PlayerWire x in Players)
            if (x.Ready)
                ReadyTotal++;
        ReadyCount.text = ReadyTotal + "/" + Players.Count + " Ready";
        AllReady = ReadyTotal == Players.Count;
    } 
    int FindPlayerLobbyPos(PlayerWire Player) {
        int PlayerPos = -1;
        for (int i = 0; i < Players.Count; i++) {
            if (Players[i].Equals(Player))
                PlayerPos = i;
        }
        return PlayerPos;
    }
    public void RegisterPlayer(PlayerWire Player) {
        bool PlayerRegistered = false;
        for (int i = 0; i < Players.Count; i++) {
            if (Players[i].Equals(Player))
                PlayerRegistered = true;
        }
        if (!PlayerRegistered) {
            Players.Add(Player);
        }
    }
    public void CrossTopBar(PlayerWire Player){
		for (int i = 0; i < Players.Count; i++) {
			if(Player.SkinID == Players[i].SkinID){
				PlayerBoxes [i].transform.GetChild (0).GetChild (1).GetComponent<Animator>().SetTrigger("Cross");
                StartCoroutine("CrossOutAudio");
			}
		}
	}
    IEnumerator CrossOutAudio() {
        yield return new WaitForSeconds(0.7f);
        Game.PlayAudio("CrossOut", 0.3f, 0.05f, 1f);
    }
    public void SetCenter(string Input){
		CenterText.text = Input;
	}
	public void SetCenter(Color Input){
		CenterText.color = Input;
	}
	public void Transition(){
		TransitionUI.SetTrigger("Transition");
        Game.PlayAudio("Rumble", 0.5f, 0f, 1f);
        StartCoroutine("RumbleTransition");
	}
    public IEnumerator RumbleTransition() {
        yield return new WaitForSeconds(1.2f);
        Game.PlayAudio("Rumble", 0.5f, 0f, 1f, 0.7f, 0f);
    }

    public void SetCodeTexts(string Code){
		foreach (Text x in CodeTexts)
			x.text = Code;
	}

    int MapToSet = 0;
    public void MapSetSelect(bool Left) {
        if (Left) {
            if (MapToSet != 0)
                MapToSet--;
        } else {
            if (MapToSet != SceneManager.MapSets.Length - 1)
                MapToSet++;
        }
        UpdateMapSet();
    }
    public void UpdateMapSet() {
        Debug.Log(SceneManager.MapSets[MapToSet]);
        MapSet.text = SceneManager.MapSets[MapToSet].Name;
        SceneManager.Maps = SceneManager.MapSets[MapToSet].Maps;
        MapImg.sprite = SceneManager.MapSets[MapToSet].Img;
    }
}
