using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

public class ConfigLoader : MonoBehaviour {
    private Dictionary<string, string> configValues;
    private string configFilePath;

    void Start() {

        //Debug.Log("Application.persistentDataPath: " + Path.Combine(Application.persistentDataPath, "bonehost.conf"));
        Debug.Log("Application.streamingAssetsPath: " + Path.Combine(Application.streamingAssetsPath, "bonehost.conf"));
        //Debug.Log("Application.dataPath: " + Path.Combine(Application.dataPath, "bonehost.conf"));
        //Debug.Log("Application.absoluteURL: " + Path.Combine(Application.absoluteURL, "bonehost.conf"));

        // Setze den Pfad für die Konfigurationsdatei im StreamingAssets-Ordner
        configFilePath = Path.Combine(Application.streamingAssetsPath, "bonehost.conf");

        // Lade die Konfiguration
        StartCoroutine(LoadConfig());
    }

    IEnumerator LoadConfig() {
        // Initialisiere das Dictionary für die Konfiguration
        configValues = new Dictionary<string, string>();

        // Überprüfe, ob die Datei im StreamingAssets-Ordner vorhanden ist
        if (File.Exists(configFilePath)) {
            Debug.Log("Konfigurationsdatei gefunden, lese lokal...");
            string[] lines = File.ReadAllLines(configFilePath);
            ParseConfigLines(lines);
        } else {
            // Wenn die Datei nicht existiert, versuche, sie über UnityWebRequest zu laden
            Debug.LogWarning("Lokal nicht gefunden, versuche über UnityWebRequest...");
            UnityWebRequest request = UnityWebRequest.Get(configFilePath);
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success) {
                Debug.LogError("Fehler beim Abrufen der Datei: " + request.error);
            } else {
                Debug.Log("Datei erfolgreich über UnityWebRequest geladen.");
                string[] lines = request.downloadHandler.text.Split('\n');
                ParseConfigLines(lines);
            }
        }
    }

    private void ParseConfigLines(string[] lines) {
        foreach (string line in lines) {
            if (!string.IsNullOrWhiteSpace(line) && line.Contains("=")) {
                string[] keyValue = line.Split('=');
                if (keyValue.Length == 2) {
                    string key = keyValue[0].Trim();
                    string value = keyValue[1].Trim();
                    configValues[key] = value;
                    Debug.Log($"Gelesen: {key} = {value}");
                }
            }
        }
    }

    public string GetConfigValue(string key)
    {
        if (configValues.TryGetValue(key, out string value))
        {
            return value;
        }
        else
        {
            Debug.LogError("Schlüssel " + key + " nicht gefunden!");
            return null;
        }
    }

    public Dictionary<string, string> GetConfigValues()
    {
        return configValues;
    }
}
