using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class AllButtons : MonoBehaviour
{
    PlayerUI SceneUI;
    SceneManager SceneManage;
    GameBehaviors Game;
    AudioManager AudioManager;
    UIManager ServerUI;
	CustomNetworkManager Network;
    PreGameOptions Options;
    public void Start() {
        AudioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        SceneUI = GameObject.Find("UIManager").GetComponent<PlayerUI>();
		Network = GameObject.Find ("NetworkManager").GetComponent<CustomNetworkManager> ();
        Options = GameObject.Find("PreGameManager").GetComponent<PreGameOptions>();
    }
    public void ToggleReady() {
        SceneUI.ToggleReady();
    }
    public void ChangeSpell(int Slot) {
        SceneUI.CurrentEditedSlot = Slot;
        SceneUI.SwitchSpellSelectorUI();
    }
    public void ClearSpell() {
        SceneUI.SwitchSpellSlot("");
        SceneUI.SwitchPlayerLobbyUI();
    }
    public void ChooseTemplate() {
        SceneUI.SwitchDeckTypeUI();
    }
    public void ReturnLobby() {
        SceneUI.SwitchPlayerLobbyUI();
    }
    public void ChooseDeckType(GameObject Button) {
        SceneUI.SwitchDeckType(Button.transform.GetChild(0).GetComponent<Text>().text);
        SceneUI.SwitchPlayerLobbyUI();
    }
    public void ChooseSpell(GameObject Button) {
        SceneUI.SwitchSpellSlot(Button.transform.GetChild(1).GetComponent<Text>().text);
        SceneUI.SwitchPlayerLobbyUI();
    }
    public void StartGame() {
        ServerUI = GameObject.Find("SceneManager").GetComponent<UIManager>();
        if (ServerUI.AllReady) {
            AudioManager.PlayAudio("Click", 0.7f, 1f);
            SceneManage = GameObject.Find("SceneManager").GetComponent<SceneManager>();
            SceneManage.StartGame();
        } else {
            AudioManager.PlayAudio("Click", 0.7f, 0.5f);
        }
    }
    public void OpenSavedDecks() {
        SceneUI.SwitchSavedDeckUI();
    }
    public void SaveDeckInSlot(int Slot) {
        SceneUI.SaveInDeckSlot(Slot);
    }
    public void GetDeckInSlot(int Slot) {
        SceneUI.GetLoadDeck(Slot);
        SceneUI.SwitchPlayerLobbyUI();
    }

	public void StartHost () {
        AudioManager.PlayAudio("Click", 0.7f, 1f);
        Network.StartHosting ();
	}
	public void StartClient (){
		SceneUI.SwitchPlayerMatchmakerUI ();
	}

	public void JoinMatch(){
		PlayerPrefs.SetString ("PlayerID", SceneUI.PlayerIDField.text);
        //Network = GameObject.Find("NetworkManager").GetComponent<CustomNetworkManager>();
        Network.JoinMatchWithCode (SceneUI.EnteredCode());
	}
	public void ReturnMainScreen(){
        if (Options.ClientUI)
            SceneUI.SwitchClientMainScreenUI();
        else {
            SceneUI.SwitchMainScreenUI();
            AudioManager.PlayAudio("Click", 0.7f, 1f);
        }
    }
	public void CancelHosting(){
		Network.EndHosting ();
		SceneUI.SwitchMainScreenUI ();
        AudioManager.PlayAudio("Click", 0.7f, 1f);
    }
    public void RunEvent(int EventID){
		SceneUI.RunEvent (EventID);
	}
	public void OpenHowTo(int Num){
		SceneUI.SwitchHowTo (Num);
	}
    public void MapSetSelect(bool Left) {
        ServerUI = GameObject.Find("SceneManager").GetComponent<UIManager>();
        ServerUI.MapSetSelect(Left);
        AudioManager.PlayAudio("Click", 0.7f, 1f);
    }
    public void ReturnSceneLobby() {
        SceneManage = GameObject.Find("SceneManager").GetComponent<SceneManager>();
        SceneManage.ForceReturnLobby();
        AudioManager.PlayAudio("Click", 0.7f, 1f);
    }
    public void OpenSpellBook() {
        SceneUI.SwitchSpellbookUI();
    }
}
