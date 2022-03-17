using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
public class CustomNetworkManager : NetworkManager
{
	int PlayerCount;
	public string Code;
	PlayerUI SceneUI;
	SceneManager SceneManager;
    UIManager UI;
	PreGameOptions Options;
	MatchInfo Match;
	bool ServerStarted;
	public void Start(){
		PlayerCount = 0;
		SceneUI = GameObject.Find("UIManager").GetComponent<PlayerUI>();
        Options = GameObject.Find("PreGameManager").GetComponent<PreGameOptions>();
        if (Options.ClientUI)
            SceneUI.SwitchClientMainScreenUI();
        else
            SceneUI.SwitchMainScreenUI();
		StartMatchMaker ();
		ServerStarted = false;
	}
	void CreateCode(){
		Code = "";
		for (int i = 0; i < 6; i++) {
			Code = Code + Random.Range (0, 10);
		}
	}
	public void EndHosting(){
		matchMaker.DestroyMatch (Match.networkId,0,OnDestroyMatch);
	}
	public override void OnDestroyMatch (bool success, string extendedInfo)
	{
		StopHost ();
		RestartHost ();
		base.OnDestroyMatch (success, extendedInfo);
	}
	void RestartHost(){
		if (SceneManager == null)
			SceneManager = GameObject.Find ("SceneManager").GetComponent<SceneManager> ();
		foreach (PlayerWire x in SceneManager.Players) {
			GameObject.Destroy (x.gameObject);
		}
		SceneManager.Players = new List<PlayerWire> ();
		PlayerCount = 0;
		StopMatchMaker ();
		StartMatchMaker ();
	}
	public void StartHosting ()
	{
		CreateCode ();
		matchMaker.CreateMatch (Code, 5, true, "", "", "", 0, 0, OnMatchCreate);
	}
	public override void OnMatchCreate (bool success, string extendedInfo, MatchInfo matchInfo)
	{
		//base.OnMatchCreate (success, extendedInfo, matchInfo);
		Match = matchInfo;
		base.StartHost (matchInfo);
	}

	public override void OnServerConnect (NetworkConnection conn)
	{
		if (SceneManager == null)
			SceneManager = GameObject.Find ("SceneManager").GetComponent<SceneManager> ();
		if (!ServerStarted) {
			PlayerCount++;
			if (PlayerCount == 1) {
				SceneManager.RunStarted ();
				UIManager UI = GameObject.Find ("SceneManager").GetComponent<UIManager> ();
				UI.SetCodeTexts (Code);
			}
			if (Options.ClearWithOnePlayer) {
				StartCoroutine ("LateLobbyStart");
				ServerStarted = true;
			} else {
				if (PlayerCount == 2) {
					SceneManager.StartLobby ();
					ServerStarted = true;
				}
			}
		}
		base.OnServerConnect (conn);
	}
	public IEnumerator LateLobbyStart(){
		yield return new WaitForSeconds (0.1f);
		SceneManager.StartLobby ();
	}
	public void JoinMatchWithCode(string Code){
		matchMaker.ListMatches (0, 1, Code, true, 0, 0, HandleListMatchesComplete);	
	}
	void HandleListMatchesComplete(bool Success, string ExtendedInfo, List<MatchInfoSnapshot> ResponseData){
        if (Success) {
            if (ResponseData.Count == 1)
                matchMaker.JoinMatch(ResponseData[0].networkId, "", "", "", 0, 0, OnMatchJoined);
            else
                SceneUI.NoCodeMessage(true, "No Game With Code Found");
        } else {
            SceneUI.NoCodeMessage(true, "Couldn't Connect To Match Maker");
            Debug.LogError("Couldn't Connect To Match Maker");
        }
	}
    void OnJoinMatch(bool Success, string ExtendedInfo, MatchInfo match) {
        if (Success) {
            MatchInfo hostInfo = match;
            NetworkManager.singleton.StartClient(hostInfo);
        } else {
            SceneUI.NoCodeMessage(true, "Join Match Failed");
            Debug.LogError("Join Match Failed");
        }
    }
    public override void OnServerDisconnect(NetworkConnection conn) {
        if (SceneManager == null)
            SceneManager = GameObject.Find("SceneManager").GetComponent<SceneManager>();
        if (UI == null)
            UI = GameObject.Find("SceneManager").GetComponent<UIManager>();
        List<PlayerWire> Players = SceneManager.Players;
        PlayerWire Kill = Players[0];
        bool ConnFound = false;
        foreach(PlayerWire x in Players) {
            if (x.connectionToClient.Equals(conn)) {
                Debug.Log("Connection Dropped");
                ConnFound = true;
                Kill = x;
                break;
            }
        }
        if (!ConnFound) {
            Debug.LogError("No Connection Found");
            return;
        }
        if (Kill.WiredWizard != null)
            Kill.WiredWizard.GetComponent<Wizard>().Kill();
        SceneManager.SkinIDAvaliable.Add(Kill.SkinID);
        Players.Remove(Kill);
        UI.ClearLobbyPlayers();
        foreach (PlayerWire x in Players) {
            UI.AddLobbyPlayer(x);
        }
        base.OnServerDisconnect(conn);
    }

}
