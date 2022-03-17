  using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PlayerUI : MonoBehaviour {
    public SpellInfoAccess SpellInfoAccess;
    public PlayerWire LocalPlayerWire;
    public Canvas[] PlayingUI;
    public Canvas[] GameUI;
    public Canvas[] LobbyUI;
    public Canvas[] PlayerLobbyUI;
    public Canvas[] SpellSelectorUI;
    public Canvas[] DeckTypeSelectorUI;
    public Canvas[] SavedDecksUI;
	public Canvas[] PlayerMatchMakerUI;
	public Canvas[] ServerCodeHostingUI;
	public Canvas[] PlayerEventUI;
	public Canvas[] MainScreenUI;
    public Canvas[] ClientMainScreenUI;
    public Canvas[] SpellBookUI;
    public GameObject[] SpellSlots;
    public GameObject[] SpellInfos;
    public int CurrentEditedSlot;
    public GameObject[] DeckTypes;
    public GameObject ReadySymbol;
    public Text ReadyText;
    public GameObject[] SavedDeckSlots;
	public Text CodeField;
	public Text PlayerIDField;
	public GameObject NoCodeObject;
	public GameObject[] PlayerEventButtons;
	public List<string> Events;
	public List<int> Cooldowns;
	public List<int> RealTimeCosts;
	public List<int> TimeCosts;
	public float[] EventsRanTimers;
	public Text TimeBankSlot;
    public Image Title;
    public Sprite[] TitleColors;
	HowToUI HWUI;
	bool[] TimersEnabled;
	float TimeBank;
	int PastNumDead = 0;
	bool InEventsMode;
	void Awake(){
		HWUI = gameObject.GetComponent<HowToUI> ();
	}
	void Update(){
		if (LocalPlayerWire != null) {
			RunEventCooldowns ();
			RunTimeBank ();
		}
	}
    public void SwitchSpellbookUI() {
        ChangeUI(SpellBookUI);
    }
    public void SwitchPlayerUI() {
		InEventsMode = false;
        ChangeUI(PlayingUI);
    }
    public void SwitchPlayerLobbyUI() {
        ChangeUI(PlayerLobbyUI);
        UpdateLobbyDeck();
        UpdateReady();
    }
    public void SwitchSpellSelectorUI() {
        ChangeUI(SpellSelectorUI);
        LoadSpellInfo();
    }
    public void SwitchGameUI() {
        ChangeUI(GameUI);
    }
    public void SwitchLobbyUI() {
        ChangeUI(LobbyUI);
    }
    public void SwitchDeckTypeUI() {
        ChangeUI(DeckTypeSelectorUI);
        LoadDeckTypes();
    }
    public void SwitchSavedDeckUI() {
        ChangeUI(SavedDecksUI);
        LoadSavedDecks();
    }
	public void SwitchPlayerMatchmakerUI() {
		ChangeUI(PlayerMatchMakerUI);
		NoCodeMessage (false, "");
		PlayerIDField.transform.parent.GetComponent<InputField>().text = PlayerPrefs.GetString("PlayerID","");
	}
	public void SwitchServerCodeHostingUI() {
		ChangeUI(ServerCodeHostingUI);
	}
	public void SwitchMainScreenUI() {
		ChangeUI(MainScreenUI);
	}
    public void SwitchClientMainScreenUI() {
        ChangeUI(ClientMainScreenUI);
        RandomizeTitle();
    }
    public void SwitchPlayerEventUI() {
		InEventsMode = true;
		ChangeUI(PlayerEventUI);
		LoadPlayerEvents ();
	}
	public void SwitchHowTo(int Num) {
		switch(Num){
		case 1:
			ChangeUI (HWUI.HowTo1);
			HWUI.LoadUI1 ();
			break;
		case 2:
			ChangeUI (HWUI.HowTo2);
			HWUI.LoadUI2 ();
			break;
		case 3:
			ChangeUI (HWUI.HowTo3);
			HWUI.LoadUI3 ();
			break;
		case 4:
			ChangeUI (HWUI.HowTo4);
			HWUI.LoadUI4 ();
			break;
		default:
			Debug.LogError ("No HowTo UI Canvas");
			break;
		}
	}
    void ChangeUI(Canvas[] ToBe) {
		Canvas[][] AllUIS = new Canvas[][] {PlayingUI, LobbyUI, GameUI, PlayerLobbyUI, SpellSelectorUI, DeckTypeSelectorUI, SavedDecksUI,
			PlayerMatchMakerUI, ServerCodeHostingUI, MainScreenUI, PlayerEventUI, HWUI.HowTo1, HWUI.HowTo2, HWUI.HowTo3, HWUI.HowTo4, ClientMainScreenUI, SpellBookUI};
		for (int i = 0; i < AllUIS.Length; i++)
			for (int j = 0; j < AllUIS [i].Length; j++)
				AllUIS [i] [j].enabled = false;
        for (int i = 0; i < ToBe.Length; i++)
            ToBe[i].enabled = true;
    }
    void RandomizeTitle() {
        Title.sprite = TitleColors[(int)Random.Range(0,TitleColors.Length)];
    }
	public void SetupEvents(){
		TimeBank = 0;
		TimersEnabled = new bool[] { false, false, false, false };
		for (int i = 0; i < Events.Count; i++) {
			if (EventsRanTimers [i] != -1) {
				ToggleEventTimers (i, false);
			}
		}
		EventsRanTimers = new float[] {-1,-1,-1,-1};
		Events = new List<string>();
		Cooldowns = new List<int>();
		TimeCosts = new List<int>();
		RealTimeCosts = new List<int> ();
		LocalPlayerWire.CmdGetEvents ();
	}
	public void EventRan(string Name){
		for (int i = 0; i < Events.Count; i++) {
			if (Events [i] == Name) {
				EventsRanTimers [i] = Cooldowns[i];
				ToggleEventTimers (i, true);
			}
		}
	}
	void RunEventCooldowns(){
		for (int i = 0; i < Events.Count; i++) {
			if (EventsRanTimers[i] != -1) {
				EventsRanTimers [i] -= Time.deltaTime;
				PlayerEventButtons [i].transform.GetChild (2).GetChild (0).GetComponent<CooldownTimer> ().UpdatePercent(EventsRanTimers [i]);
				if (EventsRanTimers [i] < 0) {
					EventsRanTimers [i] = -1;
					ToggleEventTimers (i, false);
				}
			}
		}
	}
	void RunTimeBank(){
		if (LocalPlayerWire.IsDead && InEventsMode) {
			TimeBank += Time.deltaTime;
			TimeBankSlot.text = ToTime ((int)TimeBank);
		}
	}
	void LoadPlayerEvents(){
		foreach (GameObject x in PlayerEventButtons)
			x.SetActive (false);
		for (int i = 0; i < Events.Count; i++) {
			PlayerEventButtons [i].SetActive (true);
			PlayerEventButtons [i].transform.GetChild (0).GetComponent<Text> ().text = Events[i];
			PlayerEventButtons [i].transform.GetChild (1).GetComponent<Text> ().text = "Time Cost: " + ToTime (TimeCosts[i]);
			if(EventsRanTimers[i] != -1)
				ToggleEventTimers (i, true);
			else
				ToggleEventTimers (i, false);
			
		}
	}
	void ToggleEventTimers(int Index, bool Enabled){
		if (Enabled) {
			if (!PlayerEventButtons [Index].transform.GetChild (2).gameObject.activeSelf) {
				PlayerEventButtons [Index].transform.GetChild (2).gameObject.SetActive (true);
				PlayerEventButtons [Index].transform.GetChild (2).GetChild (0).GetComponent<CooldownTimer> ().Initialize ();
				PlayerEventButtons [Index].transform.GetChild (2).GetChild (0).GetComponent<CooldownTimer> ().Enable ((float)Cooldowns [Index]);
			}
		} else {
			if (PlayerEventButtons [Index].transform.GetChild (2).gameObject.activeSelf) {
				PlayerEventButtons [Index].transform.GetChild (2).gameObject.SetActive (false);
			}
		}
		TimersEnabled [Index] = Enabled;
	}
	public void UpdateTimeCosts(int NumDead){
		if (PastNumDead < NumDead && PastNumDead != 0)
			TimeBank *= (float)NumDead / (float)PastNumDead;
		PastNumDead = NumDead;
		for (int i = 0; i < RealTimeCosts.Count; i++)
			TimeCosts [i] = RealTimeCosts [i] * NumDead;
		for (int i = 0; i < Events.Count; i++)
			PlayerEventButtons [i].transform.GetChild (1).GetComponent<Text> ().text = "Time Cost: " + ToTime (TimeCosts[i]);
	}
    public void UpdateLobbyDeck() {
        foreach (GameObject x in SpellSlots) {
            x.SetActive(false);
            x.transform.GetChild(1).gameObject.SetActive(false);
        }
        string[] Deck = LocalPlayerWire.Deck;
        int[] Layout = LocalPlayerWire.DeckLayout;
        for (int i = 0; i < Deck.Length; i++) {
            if (Layout[i] != 0) {
                SpellSlots[i].SetActive(true);
                if (Layout[i] > 0) {
                    SpellSlots[i].transform.GetChild(2).GetComponent<Text>().text = Layout[i] + "";
                    SpellSlots[i].transform.GetChild(2).GetComponent<Text>().fontSize = 40;
                } else {
                    SpellSlots[i].transform.GetChild(2).GetComponent<Text>().text = "\u221E";
                    SpellSlots[i].transform.GetChild(2).GetComponent<Text>().fontSize = 60;

                }
                if (!Deck[i].Equals("")) {
                    GameObject GlyphSpace = SpellSlots[i].transform.GetChild(1).gameObject;
                    GlyphSpace.SetActive(true);
                    GlyphSpace.GetComponent<Image>().sprite = SpellInfoAccess.GetInfo(Deck[i]).Glyph;
                }
            }
        }
    }
    public void LoadSpellInfo() {
        foreach (GameObject x in SpellInfos) {
            x.SetActive(false);
            x.transform.GetChild(3).gameObject.SetActive(false);
            x.GetComponent<Button>().enabled = true;
        }
        SpellInfo[] AllSpellInfo = SpellInfoAccess.AllSpellInfo;
        for (int i = 0; i < AllSpellInfo.Length; i++) {
            SpellInfos[i].SetActive(true);
            SpellInfos[i].transform.GetChild(0).GetComponent<Image>().sprite = AllSpellInfo[i].Glyph;
            SpellInfos[i].transform.GetChild(1).GetComponent<Text>().text = AllSpellInfo[i].Name;
            SpellInfos[i].transform.GetChild(2).GetChild(0).GetComponent<Text>().text = AllSpellInfo[i].Description;
            string[] Deck = LocalPlayerWire.Deck;
            for (int j = 0; j < Deck.Length; j++) {
                if (Deck[j].Equals(AllSpellInfo[i].Name)) {
                    SpellInfos[i].transform.GetChild(3).gameObject.SetActive(true);
                    SpellInfos[i].GetComponent<Button>().enabled = false;
                    break;
                }
            }
        }
    }
    public void SwitchSpellSlot(string Name) {
        LocalPlayerWire.Deck[CurrentEditedSlot] = Name;
        LocalPlayerWire.CmdSetSpellSlot(Name, CurrentEditedSlot);
        LocalPlayerWire.CmdUpdateLobbyDeck();
        //CleanUpDeck ();
    }
    public void SwitchSpellSlot(string Name, int Slot) {
        LocalPlayerWire.Deck[Slot] = Name;
        LocalPlayerWire.CmdSetSpellSlot(Name, Slot);
        LocalPlayerWire.CmdUpdateLobbyDeck();
        //CleanUpDeck ();
    }
    public void CleanUpDeck() {
        string[] Deck = LocalPlayerWire.Deck;
        for (int i = 0; i < Deck.Length; i++) {
            if (Deck[i] != "") {
                for (int j = 0; j < i; j++) {
                    if (Deck[j] == "") {
                        string Name = Deck[i];
                        Deck[i] = "";
                        LocalPlayerWire.CmdSetSpellSlot("", i);
                        Deck[j] = Name;
                        LocalPlayerWire.CmdSetSpellSlot(Name, j);
                        break;
                    }
                }
            }
        }
    }
    public void LoadDeckTypes() {
        foreach (GameObject x in DeckTypes) {
            x.SetActive(false);
            for (int i = 0; i < 8; i++)
                x.transform.GetChild(1).GetChild(i).gameObject.SetActive(false);
        }
        DeckType[] Decks = SpellInfoAccess.DeckTypes;
        for (int i = 0; i < Decks.Length; i++) {
            DeckTypes[i].SetActive(true);
            DeckTypes[i].transform.GetChild(0).GetComponent<Text>().text = Decks[i].Name;
            for (int j = 0; j < Decks[i].Layout.Length; j++) {
                if (Decks[i].Layout[j] != 0) {
                    GameObject Slot = DeckTypes[i].transform.GetChild(1).GetChild(j).gameObject;
                    Slot.SetActive(true);
                    if(Decks[i].Layout[j] > 0) {
                        Slot.transform.GetChild(0).GetComponent<Text>().text = Decks[i].Layout[j] + "";
                    } else {
                        Slot.transform.GetChild(0).GetComponent<Text>().text = "\u221E";
                    }
                }
            }
        }
    }
    public void SwitchDeckType(string Name) {
        PlayerLobbyUI[0].transform.GetChild(4).GetComponent<InputField>().text = "";
        DeckType ToBe = null;
        foreach (DeckType x in SpellInfoAccess.DeckTypes) {
            if (Name.Equals(x.Name)) {
                ToBe = x;
                break;
            }
        }
        SetDeckLayout(ToBe.Layout);
    }
    void SetDeckLayout(int[] Layout) {
        LocalPlayerWire.DeckLayout = Layout;
        for (int i = 0; i < LocalPlayerWire.Deck.Length; i++) {
            LocalPlayerWire.CmdSetDeckLayoutPos(Layout[i], i);
            SwitchSpellSlot("", i);
        }
    }
    void UpdateReady() {
        ReadySymbol.SetActive(LocalPlayerWire.Ready);
        if (LocalPlayerWire.Ready) {
            ReadyText.text = "Unready";
        } else {
            ReadyText.text = "Ready";
        }
    }
    public void ToggleReady() {
        LocalPlayerWire.Ready = !LocalPlayerWire.Ready;
        LocalPlayerWire.CmdSetReady(LocalPlayerWire.Ready);
        UpdateReady();
    }
    public void ToggleReady(bool Value) {
        LocalPlayerWire.Ready = Value;
        LocalPlayerWire.CmdSetReady(LocalPlayerWire.Ready);
        UpdateReady();
    }
    void LoadSavedDecks() {
        for (int i = 0; i < SavedDeckSlots.Length; i++) {
            if (PlayerPrefs.GetString(i + "SavedDeck", "") != "") {
                string[] Deck = new string[8];
                int[] Layout = new int[8];
                string Name;
                Name = PlayerPrefs.GetString(i + "SavedName", "Unnamed");
                for (int j = 0; j < Deck.Length; j++) {
                    Deck[j] = PlayerPrefs.GetString(i + "SavedDeckSlot" + j, "");
                    Layout[j] = PlayerPrefs.GetInt(i + "SavedLayoutSlot" + j, 0);
                }
                SetSavedDeckSlot(Name, Deck, Layout, i);
            } else {
                string Name = "Not Used";
                string[] Deck = { "", "", "", "", "", "", "", ""};
                int[] Layout = { 0, 0, 0, 0, 0, 0, 0, 0 };
                SetSavedDeckSlot(Name, Deck, Layout, i);
            }
        }
    }
    public void GetLoadDeck(int Slot) {
        int[] Layout = new int[8];
        string Name;
        Name = PlayerPrefs.GetString(Slot + "SavedName", "Unnamed");
        for (int j = 0; j < 8; j++) {
            Layout[j] = PlayerPrefs.GetInt(Slot + "SavedLayoutSlot" + j, 0);
        }
        SetDeckLayout(Layout);
        for (int j = 0; j < 8; j++)
            SwitchSpellSlot(PlayerPrefs.GetString(Slot + "SavedDeckSlot" + j, ""), j);
        PlayerLobbyUI[0].transform.GetChild(4).GetComponent<InputField>().text = Name;
    }
    public void SaveInDeckSlot(int Slot) {
        string Name = PlayerLobbyUI[0].transform.GetChild(4).GetComponent<InputField>().text;
        PlayerPrefs.SetString(Slot + "SavedDeck", "Used");
        PlayerPrefs.SetString(Slot + "SavedName", Name);
        for (int i = 0; i < LocalPlayerWire.Deck.Length; i++) {
            PlayerPrefs.SetString(Slot + "SavedDeckSlot" + i, LocalPlayerWire.Deck[i]);
            PlayerPrefs.SetInt(Slot + "SavedLayoutSlot" + i, LocalPlayerWire.DeckLayout[i]);
        }
        SetSavedDeckSlot(Name, LocalPlayerWire.Deck, LocalPlayerWire.DeckLayout, Slot);
    }
    public void SetSavedDeckSlot(string Name, string[] Deck, int[] Layout, int Slot) {
        Transform TheParent = SavedDeckSlots[Slot].transform;
        if (Name.Trim().Equals(""))
            Name = "Unnamed";
        TheParent.GetChild(0).GetComponent<Text>().text = Name;
        for(int i = 0; i < Deck.Length; i++) {
            if (Deck[i] != "") {
                TheParent.GetChild(2).GetChild(i).gameObject.SetActive(true);
                TheParent.GetChild(2).GetChild(i).GetComponent<Image>().sprite = SpellInfoAccess.GetInfo(Deck[i]).Glyph;
                if(Layout[i] != -1)
                    TheParent.GetChild(2).GetChild(i).GetChild(0).GetComponent<Text>().text = Layout[i].ToString();
                else
                    TheParent.GetChild(2).GetChild(i).GetChild(0).GetComponent<Text>().text = "\u221E";
            } else {
                TheParent.GetChild(2).GetChild(i).gameObject.SetActive(false);
            }
        }
    }
	public string EnteredCode(){
		return CodeField.text;
	}
	public string EnteredID(){
		return PlayerIDField.text;
	}
	public void NoCodeMessage(bool OnOff, string Substance){
		NoCodeObject.SetActive (OnOff);
        NoCodeObject.GetComponentInChildren<Text>().text = Substance;
	}
	string ToTime(int Time){
		int Seconds = Time % 60;
		int Minutes = (Time - Seconds) / 60;
		if(Seconds < 10)
			return Minutes + ":0" + Seconds;
		else
			return Minutes + ":" + Seconds;
	}
	public void RunEvent(int EventID){
		if (EventsRanTimers [EventID] == -1)
			if (TimeBank > TimeCosts [EventID]) {
				TimeBank -= TimeCosts [EventID];
				LocalPlayerWire.CmdRunEvent (Events [EventID]);
			}
	}
}
