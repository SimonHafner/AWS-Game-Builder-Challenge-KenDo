using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Threading.Tasks;
#if UNITY_WEBGL
using UnityEngine.Networking;
#endif

public class Level
{
    private static Level instance;
    private bool isLoading = false;
    private TaskCompletionSource<bool> loadingComplete;

    private Level() 
    {
        loadingComplete = new TaskCompletionSource<bool>();
    }

    public static Level Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new Level();
            }
            return instance;
        }
    }

    public int level;
    public int timeLimit;
    public int mistakesLimit;
    public int tableSize;
    private List<List<int>> data;

    // Getter und Setter bleiben unverändert
    public int GetLevel() { return level; }
    public void SetLevel(int value) { level = value; }

    public int GetTimeLimit() { return timeLimit; }
    public void SetTimeLimit(int value) { timeLimit = value; }

    public int GetMistakesLimit() { return mistakesLimit; }
    public void SetMistakesLimit(int value) { mistakesLimit = value; }

    public int GetTableSize() { return tableSize; }
    public void SetTableSize(int value) { tableSize = value; }

    public List<List<int>> GetData() { return data; }
    public void SetData(List<List<int>> value) { data = value; }

    // Synchrone Version für bestehenden Code
    public void LoadLevelData(int levelNumber)
    {
        // Wenn noch geladen wird, warten wir synchron
        if (isLoading)
        {
            loadingComplete.Task.Wait();
        }

        string folderPathData = $"{Application.streamingAssetsPath}/Data/";
        string jsonFilePath = Path.Combine(folderPathData, "levels.json");

#if UNITY_WEBGL && !UNITY_EDITOR
        using (UnityWebRequest www = UnityWebRequest.Get(jsonFilePath))
        {
            var asyncOp = www.SendWebRequest();
            while (!asyncOp.isDone) { } // Blockierend, aber nur für Legacy-Support

            if (www.result == UnityWebRequest.Result.Success)
            {
                string jsonString = www.downloadHandler.text;
                ParseLevelData(jsonString, levelNumber);
            }
            else
            {
                Debug.LogError("Fehler beim Laden der levels.json unter WebGL: " + www.error);
            }
        }
#else
        if (File.Exists(jsonFilePath))
        {
            string jsonString = File.ReadAllText(jsonFilePath);
            ParseLevelData(jsonString, levelNumber);
        }
        else
        {
            Debug.LogError("Die Datei levels.json wurde nicht gefunden unter: " + jsonFilePath);
        }
#endif
    }

    // Neue asynchrone Version für WebGL
    public async Task LoadLevelDataAsync(int levelNumber)
    {
        if (isLoading)
        {
            await loadingComplete.Task;
            return;
        }

        isLoading = true;

        try
        {
            string folderPathData = $"{Application.streamingAssetsPath}/Data/";
            string jsonFilePath = Path.Combine(folderPathData, "levels.json");

#if UNITY_WEBGL && !UNITY_EDITOR
            using (UnityWebRequest www = UnityWebRequest.Get(jsonFilePath))
            {
                var operation = www.SendWebRequest();

                while (!operation.isDone)
                {
                    await Task.Yield();
                }

                if (www.result == UnityWebRequest.Result.Success)
                {
                    string jsonString = www.downloadHandler.text;
                    ParseLevelData(jsonString, levelNumber);
                }
                else
                {
                    Debug.LogError("Fehler beim Laden der levels.json unter WebGL: " + www.error);
                }
            }
#else
            if (File.Exists(jsonFilePath))
            {
                string jsonString = File.ReadAllText(jsonFilePath);
                ParseLevelData(jsonString, levelNumber);
            }
            else
            {
                Debug.LogError("Die Datei levels.json wurde nicht gefunden unter: " + jsonFilePath);
            }
#endif
        }
        finally
        {
            isLoading = false;
            loadingComplete.SetResult(true);
            loadingComplete = new TaskCompletionSource<bool>(); // Reset für nächsten Load
        }
    }
    
    private void ParseLevelData(string jsonString, int levelNumber)
    {
        // Manuelles Parsen des JSON-Inhalts
        int levelsIndex = jsonString.IndexOf("\"levels\"");
        if (levelsIndex >= 0)
        {
            int startIndex = jsonString.IndexOf("[", levelsIndex);
            int endIndex = jsonString.LastIndexOf("]");

            if (startIndex >= 0 && endIndex >= 0)
            {
                string levelsArrayString = jsonString.Substring(startIndex + 1, endIndex - startIndex - 1);

                int currentIndex = 0;
                bool levelFound = false;

                while (currentIndex < levelsArrayString.Length)
                {
                    int levelStartIndex = levelsArrayString.IndexOf("{", currentIndex);
                    if (levelStartIndex < 0) break;
                    int levelEndIndex = levelsArrayString.IndexOf("}", levelStartIndex);
                    if (levelEndIndex < 0) break;

                    string levelObjectString = levelsArrayString.Substring(levelStartIndex, levelEndIndex - levelStartIndex + 1);

                    // Extrahieren des "level"-Attributs
                    int parsedLevelNumber = ExtractIntValue(levelObjectString, "\"level\"");
                    if (parsedLevelNumber == levelNumber)
                    {
                        // Attribute extrahieren
                        this.level = parsedLevelNumber;
                        string timeLimitString = ExtractStringValue(levelObjectString, "\"timeLimit\"");
                        this.timeLimit = ConvertTimeLimitToSeconds(timeLimitString); // Umwandlung zu Sekunden
                        this.mistakesLimit = ExtractIntValue(levelObjectString, "\"mistakesLimit\"");
                        this.tableSize = ExtractIntValue(levelObjectString, "\"tableSize\"");

                        // Parsen des 'data'-Feldes
                        int dataIndex = levelObjectString.IndexOf("\"data\"");
                        if (dataIndex >= 0)
                        {
                            int dataStartIndex = levelObjectString.IndexOf("[", dataIndex);
                            int dataEndIndex = levelObjectString.LastIndexOf("]");
                            if (dataStartIndex >= 0 && dataEndIndex >= 0)
                            {
                                string dataArrayString = levelObjectString.Substring(dataStartIndex, dataEndIndex - dataStartIndex + 1);
                                data = ParseDataArray(dataArrayString);
                            }
                        }

                        levelFound = true;
                        break;
                    }

                    currentIndex = levelEndIndex + 1;
                }

                if (!levelFound)
                {
                    Debug.LogError("Level " + levelNumber + " wurde nicht in levels.json gefunden.");
                }
            }
        }
    }

    // Hilfsmethoden zum Parsen und Umwandeln des Zeitlimits
    private int ConvertTimeLimitToSeconds(string timeLimit)
    {
        if (TimeSpan.TryParseExact(timeLimit, "mm\\:ss", null, out TimeSpan result))
        {
            return (int)result.TotalSeconds;
        }
        Debug.LogError("Ungültiges Zeitlimit-Format: " + timeLimit);
        return 0;
    }

    private int ExtractIntValue(string source, string key)
    {
        int keyIndex = source.IndexOf(key);
        if (keyIndex >= 0)
        {
            int colonIndex = source.IndexOf(":", keyIndex);
            if (colonIndex >= 0)
            {
                int commaIndex = source.IndexOfAny(new char[] { ',', '}' }, colonIndex);
                if (commaIndex >= 0)
                {
                    string valueString = source.Substring(colonIndex + 1, commaIndex - colonIndex - 1).Trim();
                    if (valueString.EndsWith(","))
                        valueString = valueString.Substring(0, valueString.Length - 1);
                    if (int.TryParse(valueString, out int value))
                    {
                        return value;
                    }
                }
            }
        }
        return 0;
    }

    private string ExtractStringValue(string source, string key)
    {
        int keyIndex = source.IndexOf(key);
        if (keyIndex >= 0)
        {
            int colonIndex = source.IndexOf(":", keyIndex);
            if (colonIndex >= 0)
            {
                int quoteStartIndex = source.IndexOf("\"", colonIndex);
                int quoteEndIndex = source.IndexOf("\"", quoteStartIndex + 1);
                if (quoteStartIndex >= 0 && quoteEndIndex >= 0)
                {
                    return source.Substring(quoteStartIndex + 1, quoteEndIndex - quoteStartIndex - 1);
                }
            }
        }
        return "";
    }

    private List<List<int>> ParseDataArray(string dataArrayString)
    {
        List<List<int>> result = new List<List<int>>();

        // Entferne die äußeren Klammern
        dataArrayString = dataArrayString.Trim();
        if (dataArrayString.StartsWith("["))
            dataArrayString = dataArrayString.Substring(1);
        if (dataArrayString.EndsWith("]"))
            dataArrayString = dataArrayString.Substring(0, dataArrayString.Length - 1);

        // Teile den String in Zeilen auf
        string[] rowStrings = dataArrayString.Split(new string[] { "],", "]" }, StringSplitOptions.RemoveEmptyEntries);

        foreach (string rowString in rowStrings)
        {
            string trimmedRow = rowString.Trim();
            if (trimmedRow.StartsWith("["))
                trimmedRow = trimmedRow.Substring(1);

            string[] numberStrings = trimmedRow.Split(',');

            List<int> row = new List<int>();
            foreach (string numStr in numberStrings)
            {
                if (int.TryParse(numStr.Trim(), out int num))
                {
                    row.Add(num);
                }
            }
            result.Add(row);
        }

        return result;
    }

    // Methode zum Ausgeben der Attribute in JSON-Form
    public void Print()
    {
        string jsonString = "{\n";
        jsonString += $"    \"level\": {level},\n";
        jsonString += $"    \"timeLimit\": \"{timeLimit}\",\n"; // Ausgabe des Zeitlimits in Sekunden
        jsonString += $"    \"mistakesLimit\": {mistakesLimit},\n";
        jsonString += $"    \"tableSize\": {tableSize},\n";
        jsonString += $"    \"data\": [\n";

        if (data != null)
        {
            for (int i = 0; i < data.Count; i++)
            {
                jsonString += "        [";
                for (int j = 0; j < data[i].Count; j++)
                {
                    jsonString += data[i][j];
                    if (j < data[i].Count - 1)
                    {
                        jsonString += ", ";
                    }
                }
                jsonString += "]";
                if (i < data.Count - 1)
                {
                    jsonString += ",";
                }
                jsonString += "\n";
            }
        }
        else
        {
            jsonString += "        null\n";
        }

        jsonString += "    ]\n";
        jsonString += "}";
        Debug.Log(jsonString);
    }
}
