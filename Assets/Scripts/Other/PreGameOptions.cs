using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreGameOptions : MonoBehaviour
{
	public bool ServerPlayer;
    public bool NoEnd;
	public string[] StandardDeck;
    public int[] StandardLayout;
    public bool ServerPlayerLobby;
	public bool ClearWithOnePlayer;
	public string ServerPlayerName;
    public bool ClientUI;
    public bool ConsoleStatus;
	void Update (){
		if (Input.GetKeyUp (KeyCode.UpArrow)) {
			ServerPlayer = true;
		}
		if (Input.GetKeyUp (KeyCode.DownArrow)) {
			ServerPlayer = false;
		}
	}
}
