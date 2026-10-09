using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class Study : MonoBehaviour
{
    // Referenzen auf UI-Elemente
    private Text txt;
    private Dropdown DatasetSelector, AnatomySelector, SideSelector, GenderSelector, EthnicGroupSelector, StudySelector;
    private ConfigLoader ConfigSelector;

    public void Start()
    {
        // UI-Elemente finden
        txt = GameObject.Find("/Canvas/Messages").GetComponent<Text>();
        DatasetSelector = GameObject.Find("DatasetSelector").GetComponent<Dropdown>();
        AnatomySelector = GameObject.Find("AnatomySelector").GetComponent<Dropdown>();
        SideSelector = GameObject.Find("SideSelector").GetComponent<Dropdown>();
        GenderSelector = GameObject.Find("GenderSelector").GetComponent<Dropdown>();
        EthnicGroupSelector = GameObject.Find("EthnicGroupSelector").GetComponent<Dropdown>();
        StudySelector = GameObject.Find("StudySelector").GetComponent<Dropdown>();

        // ConfigLoader-Instanz finden
        ConfigSelector = FindAnyObjectByType<ConfigLoader>();

        // Event Listener hinzufügen
        StudySelector.onValueChanged.AddListener(delegate { StartStudy(); });

    }

    // Methode, die den Studienstart initiiert
    public void StartStudy()
    {
        // Überprüfen, ob ein Dataset ausgewählt wurde
        if (DatasetSelector.value <= 0)
        {
            txt.text = "Select Dataset!";
            return;
        }

        txt.text = StudySelector.value.ToString();

        switch (StudySelector.value){

            case 0:
                txt.text = "Select study";
                break;
            case 1: // Thesis
                // UI-Aktualisierung und Start des Requests
                txt.text = "Please wait ...";
                StartCoroutine(BoneDocRequest());
                break;
            case 2: // Fitting
                // UI-Aktualisierung und Start des Requests
                txt.text = "Not available yet";
                break;
            default:
                break;
        }

    }

    // Die Anfrage an das Backend
    IEnumerator BoneDocRequest()
    {
        // Warten, bis die Konfiguration verfügbar ist
        while (ConfigSelector.GetConfigValues() == null)
        {
            yield return null;
        }

        // Die URL für das Backend abrufen
        string bonedoc_url = ConfigSelector.GetConfigValue("bonedoc_url");

        // Sicherstellen, dass die URL mit https beginnt
        if (string.IsNullOrEmpty(bonedoc_url) || !bonedoc_url.StartsWith("https://"))
        {
            txt.text = "Invalid or missing backend URL configuration (https:// required).";
            Debug.LogError("Invalid or missing URL in config.");
            yield break;
        }

        Debug.Log("Using backend URL: " + bonedoc_url);

        // Unity WebRequest erstellen und Header setzen
        UnityWebRequest request = UnityWebRequest.Get(bonedoc_url);
        SetRequestHeaders(request);

        // Anfrage senden und warten
        yield return request.SendWebRequest();

        // Antwort verarbeiten
        ProcessResponse(request);
    }

    // Setzt alle erforderlichen Header für die Anfrage
    private void SetRequestHeaders(UnityWebRequest request)
    {
        request.SetRequestHeader("Dataset", DatasetSelector.captionText.text);
        request.SetRequestHeader("Anatomy", AnatomySelector.captionText.text);
        request.SetRequestHeader("Side", SideSelector.captionText.text);
        request.SetRequestHeader("Gender", GenderSelector.captionText.text);
        request.SetRequestHeader("EthnicGroup", EthnicGroupSelector.captionText.text);
        request.SetRequestHeader("Study", StudySelector.captionText.text);
    }

    // Verarbeitet die Antwort von der Anfrage
    private void ProcessResponse(UnityWebRequest request)
    {
        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            txt.text = $"Error: {request.error}";
            Debug.LogError($"Error: {request.error}, Response Code: {request.responseCode}, Response: {request.downloadHandler.text}");
        }
        else
        {
            txt.text = request.downloadHandler.text;
            ParseEthnicGroups(txt.text);
        }

        // Garbage Collection für den Request
        request.Dispose();
    }

    // Beispiel für die Extraktion von ethnischen Gruppen aus dem Antworttext (optional)
    private void ParseEthnicGroups(string responseText)
    {
        int asianPos = responseText.IndexOf("asian");
        if (asianPos >= 0)
        {
            int percentPos = responseText.IndexOf("%", asianPos);
            if (percentPos >= 0)
            {
                string asianPercentage = responseText.Substring(asianPos + 7, percentPos - asianPos - 7);
                Debug.Log($"Asian Percentage: {asianPercentage}");
            }
        }

        int caucasianPos = responseText.IndexOf("caucasian");
        if (caucasianPos >= 0)
        {
            int percentPos = responseText.IndexOf("%", caucasianPos);
            if (percentPos >= 0)
            {
                string caucasianPercentage = responseText.Substring(caucasianPos + 11, percentPos - caucasianPos - 11);
                Debug.Log($"Caucasian Percentage: {caucasianPercentage}");
            }
        }
    }
}
