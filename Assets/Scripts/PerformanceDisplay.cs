using UnityEngine;
using TMPro;
using UnityEngine.Profiling;

public class PerformanceDisplay : MonoBehaviour
{
    public TextMeshProUGUI performanceText;
    private float[] fpsBuffer;
    private int bufferIndex;
    private float deltaTime;
    private float avgFPS;
    private float minFPS = float.MaxValue;
    private float maxFPS = 0;
    private int timeWindowInSeconds = 5; // Zeitfenster in Sekunden für den FPS-Gleitschnitt

    void Start()
    {
        // Initialisiere den Puffer mit der Anzahl der Frames basierend auf dem Zeitfenster (z.B. 5 Sekunden bei 60 FPS)
        fpsBuffer = new float[timeWindowInSeconds * 60]; // Annahme: 60 FPS als Ziel
        bufferIndex = 0;
    }

    void Update()
    {
        // FPS für den aktuellen Frame berechnen
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;

        // Tiefst- und Höchstwert der Session aktualisieren
        if (fps < minFPS)
            minFPS = fps;
        if (fps > maxFPS)
            maxFPS = fps;

        // FPS zum Puffer hinzufügen
        fpsBuffer[bufferIndex] = fps;
        bufferIndex = (bufferIndex + 1) % fpsBuffer.Length;

        // Durchschnitt über das Zeitfenster (z.B. 5 Sekunden) berechnen
        float totalFPS = 0;
        foreach (float f in fpsBuffer)
        {
            totalFPS += f;
        }
        avgFPS = totalFPS / fpsBuffer.Length;

        // CPU-Auslastung (Dummy-Wert als Beispiel)
        float cpuUsage = SystemInfo.processorCount * 10; // Beispielwert

        // Speichernutzung in MB
        long memoryUsage = Profiler.GetTotalAllocatedMemoryLong() / (1024 * 1024);

        // Ausgabe der gemittelten FPS, CPU und Memory sowie Tiefst- und Höchstwert
        if (performanceText != null)
        {
            performanceText.text = string.Format(
                "FPS (5s Schnitt): {0:0.} | Min: {1:0.} | Max: {2:0.} | CPU: {3}% | Memory: {4} MB",
                avgFPS, minFPS, maxFPS, cpuUsage, memoryUsage
            );
        }
        else
        {
            Debug.LogError("Performance TextMeshProUGUI is not assigned!");
        }
    }
}
