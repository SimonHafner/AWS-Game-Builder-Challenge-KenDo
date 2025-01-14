using UnityEngine;
using TMPro;
using UnityEngine.Rendering;


public class HomeManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI currentLevel;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI playText;
    [SerializeField] private TextMeshProUGUI optionsText;
    [SerializeField] private TextMeshProUGUI starsCount;

    private void Awake()
    {
        // Framerate für alle unterstützten Plattformen setzen
        #if UNITY_EDITOR || UNITY_IOS || UNITY_WEBGL
        void SetTargetFramerate()
        {
            // GPU-Typ überprüfen und Ziel-Framerate entsprechend setzen
            if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLCore || 
                SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLES2 || 
                SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLES3)
            {
                Application.targetFrameRate = 30; // Für schwächere Geräte
            }
            else
            {
                Application.targetFrameRate = 60; // Für leistungsfähige Geräte
            }

            // VSync ausschalten für konsistente Framerate
            QualitySettings.vSyncCount = 0;
            }

            // Funktion beim Start ausführen
            SetTargetFramerate();
            #endif

        // Sicherstellen, dass PlayerData geladen wird
        PlayerData.Instance.LoadPlayerData();
        currentLevel.text = PlayerData.Instance.level.ToString();
        playText.text = InterfaceText.Instance.GetText(PlayerData.Instance.gameSettings.language, "Play");
        optionsText.text = InterfaceText.Instance.GetText(PlayerData.Instance.gameSettings.language, "Options");
        levelText.text = InterfaceText.Instance.GetText(PlayerData.Instance.gameSettings.language, "Level");
        starsCount.text = PlayerData.Instance.stars.ToString();
        Debug.Log("PlayerData wurde geladen.");
    }

    private void OnApplicationQuit()
    {
        // Speichern, wenn das Spiel beendet wird
        PlayerData.Instance.SavePlayerData();
        Debug.Log("PlayerData wurde gespeichert.");
    }
}