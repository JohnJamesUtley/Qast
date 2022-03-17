using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CustomDebugLog : MonoBehaviour {
    public string output = "";
    public string stack = "";
    public Text Console;
    public void Start() {
        Console.enabled = GameObject.Find("PreGameManager").GetComponent<PreGameOptions>().ConsoleStatus;
    }
    void OnEnable() {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable() {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type) {
        output = logString;
        stack = stackTrace;
        Console.text = "-" + logString + "\n+" + stackTrace + "\n" + Console.text;
    }
    public void Update() {
        if (Input.GetKeyUp(KeyCode.H)) {
            Console.enabled = !Console.enabled;
        }
    }
}