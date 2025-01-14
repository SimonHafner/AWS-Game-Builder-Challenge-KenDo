using UnityEngine;
using TMPro;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; } // Singleton-Instance

    private TextMeshProUGUI textMesh;
    private BarController barController;

    private float timeLimit; // Gesamtzeit in Sekunden
    private float[] timeLimitQuartiles; // Speichert die Quartilswerte
    private bool[] quartileReached; // Speichert, welche Quartile bereits erreicht wurden
    private float startTime;
    private float pauseTime; // Speichert die verstrichene Zeit beim Pausieren
    private bool isPaused = false; // Variable, um zu überprüfen, ob der Timer pausiert ist

    public StarController star1Controller;
    public StarController star2Controller;
    public StarController star3Controller;

    private int starsCount = 3;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this; // Setze die Singleton-Instanz
        }
        else
        {
            Destroy(gameObject); // Verhindert mehrere Instanzen des TimeManagers
            return;
        }

        star1Controller = GameObject.Find("Star-1").GetComponent<StarController>();
        star2Controller = GameObject.Find("Star-2").GetComponent<StarController>();
        star3Controller = GameObject.Find("Star-3").GetComponent<StarController>();

        // Dynamically load TimeLimit from LevelData
        PlayerData.Instance.LoadPlayerData();
        Level.Instance.LoadLevelData(PlayerData.Instance.level);
        timeLimit = Level.Instance.GetTimeLimit();

        // Initialisiere timeLimitQuartiles und quartileReached
        timeLimitQuartiles = new float[] {timeLimit / 4f, timeLimit / 2f, 3 * timeLimit / 4f};
        quartileReached = new bool[3]; // Standardmäßig sind alle Werte false
    }

    void Start()
    {
        // Dynamically load TimeLimit from LevelData
        PlayerData.Instance.LoadPlayerData();
        Level.Instance.LoadLevelData(PlayerData.Instance.level);
        timeLimit = Level.Instance.GetTimeLimit();
        textMesh = GetComponent<TextMeshProUGUI>();
        barController = FindObjectOfType<BarController>();
        Debug.Log("Current Stars Count: " + getStarsCount());
        
        if (barController == null) 
        {
            Debug.LogError("BarController wurde nicht gefunden.");
        }
        barController.setFillAmount(0.5f); // Setze den Anfangswert für die Füllmenge
        if(textMesh == null)
        {
            Debug.LogError("Text Mesh Pro Komponente nicht gefunden!");
        }
        else
        {
            textMesh.text = "00:00"; // Initialisiere den Text auf 00:00 bei Start
        }
        startTime = Time.time; // Speichere die Startzeit

        if(PlayerData.Instance.level == 1){
            PauseTimer();
        }
    }
    
    private void Update() 
    {
        if (isPaused) return; // Stoppt das Update, wenn der Timer pausiert ist

        if(textMesh != null && barController != null)
        {
            float elapsedTime = Time.time - startTime;
            float fillValue = 0.5f - (elapsedTime / timeLimit) / 2;
            barController.setFillAmount(fillValue);

            int minutes = (int)elapsedTime / 60;
            int seconds = (int)elapsedTime % 60;
            textMesh.text = minutes <= 99 ? string.Format("{0:00}:{1:00}", minutes, seconds) : string.Format("{0}:{1:00}", minutes, seconds);

            // Überprüfe die Quartile und starte die Animationen entsprechend
            for (int i = 0; i < timeLimitQuartiles.Length; i++)
            {
                if (!quartileReached[i] && elapsedTime > timeLimitQuartiles[i])
                {
                    // Markiere das Quartil als erreicht
                    quartileReached[i] = true;

                    // Starte die Animation basierend auf dem erreichten Quartil
                    switch (i)
                    {
                        case 0: // 1/4 Zeit erreicht
                            star3Controller.StartFadeAnimation();
                            removeStar();
                            Debug.Log("Current Stars Count: " + getStarsCount());
                            GameManager.Instance.ActivateStars(2);
                            break;
                        case 1: // 2/4 Zeit erreicht
                            star2Controller.StartFadeAnimation();
                            removeStar();
                            Debug.Log("Current Stars Count: " + getStarsCount());
                            GameManager.Instance.ActivateStars(1);
                            break;
                        case 2: // 3/4 Zeit erreicht
                            star1Controller.StartFadeAnimation();
                            removeStar();
                            Debug.Log("Current Stars Count: " + getStarsCount());
                            GameManager.Instance.ActivateStars(0);
                            break;
                    }
                }
            }
        }
    }

    public void PauseTimer()
    {
        if (!isPaused)
        {
            isPaused = true; // Setzt den Timer auf Pause
            pauseTime = Time.time - startTime; // Speichert die verstrichene Zeit
        }
    }

    public void ResumeTimer()
    {
        if (isPaused)
        {
            isPaused = false; // Timer fortsetzen
            startTime = Time.time - pauseTime; // Passe die Startzeit an, um die Pausierung zu berücksichtigen
        }
    }

    public void removeStar()
    {
        starsCount -= 1;
    }

    public int getStarsCount()
    {
        return starsCount;
    }

    public string getTextMesh()
    {
        return textMesh.text;
    }

}
