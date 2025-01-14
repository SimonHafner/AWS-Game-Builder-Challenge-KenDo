// TutorialText.cs
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using System.Threading.Tasks;

#if UNITY_WEBGL
using UnityEngine.Networking;
#endif

public class TutorialText
{
    private static TutorialText instance;
    private bool isLoading = false;
    private TaskCompletionSource<bool> loadingComplete;
    private Dictionary<string, List<string>> translations;

    public static TutorialText Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new TutorialText();
            }
            return instance;
        }
    }

    private TutorialText()
    {
        translations = new Dictionary<string, List<string>>();
        loadingComplete = new TaskCompletionSource<bool>();
        LoadTranslations();
    }

    private async void LoadTranslations()
    {
        if (isLoading) return;
        isLoading = true;

        // Pfad zum "tutorialText.json" innerhalb des Ordners "Data" im StreamingAssets
        string folderPathData = $"{Application.streamingAssetsPath}/Data/";
        string filePath = Path.Combine(folderPathData, "tutorialText.json");

#if UNITY_WEBGL && !UNITY_EDITOR
        // Nur in WebGL-Builds (nicht im Editor) wird über UnityWebRequest geladen
        using (UnityWebRequest www = UnityWebRequest.Get(filePath))
        {
            var operation = www.SendWebRequest();

            while (!operation.isDone)
            {
                await Task.Yield();
            }

            if (www.result == UnityWebRequest.Result.Success)
            {
                string jsonData = www.downloadHandler.text;
                translations = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(jsonData);
            }
            else
            {
                Debug.LogError($"Error loading JSON from {filePath}. Error: {www.error}");
            }
        }
#else
        // In allen anderen Fällen wird direkt von der Festplatte gelesen
        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);
            translations = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(jsonData);
        }
        else
        {
            Debug.LogError($"File not found at: {filePath}");
        }
#endif

        isLoading = false;
        loadingComplete.SetResult(true);
    }

    /// <summary>
    /// Synchrone Variante für bestehenden Code. Blockiert, bis das Laden abgeschlossen ist.
    /// Achtung: In WebGL kann dies (im echten Build) zu Hängern führen, weil
    /// threading dort nur eingeschränkt verfügbar ist!
    /// </summary>
    public List<string> GetTextList(string languageCode)
    {
        if (isLoading)
        {
            // Wartet synchron auf das TaskCompletionSource
            loadingComplete.Task.Wait();
        }

        if (translations.ContainsKey(languageCode))
        {
            return translations[languageCode];
        }
        else
        {
            Debug.LogWarning($"Language not found: {languageCode}");
            return new List<string>();
        }
    }

    /// <summary>
    /// Asynchrone Variante für neue Implementierungen.
    /// </summary>
    public async Task<List<string>> GetTextListAsync(string languageCode)
    {
        if (isLoading)
        {
            // Wartet asynchron auf das TaskCompletionSource
            await loadingComplete.Task;
        }

        if (translations.ContainsKey(languageCode))
        {
            return translations[languageCode];
        }
        else
        {
            Debug.LogWarning($"Language not found: {languageCode}");
            return new List<string>();
        }
    }
}
