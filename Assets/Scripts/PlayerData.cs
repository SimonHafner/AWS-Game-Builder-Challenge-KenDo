using System;
using System.IO;
using UnityEngine;

[System.Serializable]
public class GameSettings
{
    public bool sound;
    public float musicVolume;
    public bool notificationsEnabled;
    public string language;
}

[System.Serializable]
public class PlayerData
{
    public string playerName;
    public int level;
    public int stars;
    public GameSettings gameSettings;
    public string lastSave;

    // Singleton-Instanz
    private static PlayerData instance;

    // Singleton-Zugriff
    public static PlayerData Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new PlayerData();
                Debug.Log("PlayerData Singleton instanziiert");
            }
            return instance;
        }
    }

    // Privater Konstruktor für Singleton
    private PlayerData()
{
    playerName = "Player";
    level = 1;
    stars = 0;

    // Systemsprache in unterstützte Sprache bzw. String umwandeln
    string languageToUse = GetSupportedLanguage(Application.systemLanguage);

    gameSettings = new GameSettings
    {
        sound = true,
        musicVolume = 0.8f,
        notificationsEnabled = true,
        language = languageToUse
    };
    
    lastSave = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
}

private string GetSupportedLanguage(UnityEngine.SystemLanguage language)
{
    switch (language)
    {
        case UnityEngine.SystemLanguage.German:
            return "de";
        case UnityEngine.SystemLanguage.English:
            return "en";
        case UnityEngine.SystemLanguage.French:
            return "fr";
        case UnityEngine.SystemLanguage.Italian:
            return "it";
        case UnityEngine.SystemLanguage.Spanish:
            return "es";
        case UnityEngine.SystemLanguage.Portuguese:
            return "pt";
        default:
            return "en"; // Fallback auf Englisch
    }
}

    // Methode zum Speichern von PlayerData
    public void SavePlayerData()
    {
        string directoryPath = Path.Combine(Application.persistentDataPath, "Data");
        string filePath = Path.Combine(directoryPath, "playerData.json");

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        lastSave = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
        string json = JsonUtility.ToJson(this);
        File.WriteAllText(filePath, json);
        Debug.Log("PlayerData erfolgreich gespeichert: " + json);
    }

    // Methode zum Laden von PlayerData
    public void LoadPlayerData()
    {
        string directoryPath = Path.Combine(Application.persistentDataPath, "Data");
        string filePath = Path.Combine(directoryPath, "playerData.json");

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            JsonUtility.FromJsonOverwrite(json, instance);
            Debug.Log("PlayerData erfolgreich geladen: " + json);
        }
        else
        {
            Debug.LogError("Fehler beim Laden der PlayerData! Datei nicht gefunden unter: " + filePath);
            // Standarddaten initialisieren und speichern, falls die Datei nicht existiert
            SavePlayerData();
        }
    }
}