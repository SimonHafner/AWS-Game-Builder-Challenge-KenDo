using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Ricimi;
using TMPro;
using DG.Tweening;
using UnityEngine.Audio;
using System.Threading.Tasks;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; } // Singleton-Pattern
    
    [SerializeField] private List<Sprite> spriteElements;
    [SerializeField] private List<GameObject> backgroundElements;

    private string folderPathData;
    private List<List<int>> indexValues;
    private int mistakesCount;
    public Button selectedButton;
    public GameObject tableObject;
    [SerializeField] private AudioClip gameOverSound;
    [SerializeField] private TextMeshProUGUI clickInstructionText;
    [SerializeField] private TextMeshProUGUI solvedText;
    [SerializeField] private TextMeshProUGUI continueText;
    [SerializeField] private TextMeshProUGUI levelInterfaceText;
    [SerializeField] private TextMeshProUGUI mistakesInterfaceText;
    [SerializeField] private TextMeshProUGUI currentLevel;
    [SerializeField] private TextMeshProUGUI playTimeWin;
    [SerializeField] private TextMeshProUGUI playTimeFail;
    private AudioSource audioSource;
    public GameObject puzzleSolvedPopup;
    public GameObject gameOverPopup;
    public GameObject popupBackground;
    private Fader fader;
    public GameObject starsBackground;
    private List<GameObject> starList = new List<GameObject>();

    [SerializeField] private AudioMixer uiAudioMixer;
    private const string UIVolumeParameterName = "UIVolume";

    private void Awake()
    {
        Debug.Log("GameManager Awake aufgerufen");
        if (Instance == null)
        {
            Instance = this;
            audioSource = gameObject.AddComponent<AudioSource>();
            DOTween.SetTweensCapacity(200, 125); 
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private async void Start()
{
    // 1. Basis-Initialisierungen (unverändert)
    foreach (Transform star in starsBackground.transform)
    {
        starList.Add(star.gameObject); 
    }
    folderPathData = $"{Application.streamingAssetsPath}/Data/"; 
    PlayerData.Instance.LoadPlayerData(); 
    SetInterfaceText(); 
    uiAudioMixer.SetFloat(UIVolumeParameterName, PlayerData.Instance.gameSettings.sound ? 0f : -80f);
    List<string> tutorialTextList = TutorialText.Instance.GetTextList(PlayerData.Instance.gameSettings.language);
    currentLevel.text = PlayerData.Instance.level.ToString();

    // 2. Asynchrones Laden der Level-Daten, keine Blockierung
    await LoadDataAsync();
    
    // 3. Anschließende Schritte (z.B. Spiellogik zurücksetzen)
    ResetGame();
}

        private void SetInterfaceText(){
    levelInterfaceText.text = InterfaceText.Instance.GetText(PlayerData.Instance.gameSettings.language, "Level");
    mistakesInterfaceText.text = InterfaceText.Instance.GetText(PlayerData.Instance.gameSettings.language, "Mistakes");
    clickInstructionText.text = InterfaceText.Instance.GetText(PlayerData.Instance.gameSettings.language, "ClickInstruction");
    solvedText.text = InterfaceText.Instance.GetText(PlayerData.Instance.gameSettings.language, "Solved");
    continueText.text = InterfaceText.Instance.GetText(PlayerData.Instance.gameSettings.language, "Continue");
}

    public void ActivateStars(int numberStars)
    {
        // Deaktiviere zuerst alle Sterne
        foreach (GameObject star in starList)
        {
            star.SetActive(false);
        }

        // Aktiviere die angegebenen Sterne, abhängig von numberStars
        for (int i = 0; i < numberStars && i < starList.Count; i++)
        {
            starList[i].SetActive(true);
        }
    }

    // Getters
    public List<List<int>> GetIndexValues() => indexValues;

    public int GetIndexValue(int x, int y) => indexValues[x][y];

    // Public method to update all item source images
    public void UpdateAllItemSourceImages() 
    {
        if (!IsDataValid()) return;

        for (int x = 0; x < indexValues.Count; x++) 
        {
            for (int y = 0; y < indexValues[x].Count; y++) 
            {
                UpdateTile(x, y);
            }
        }
    }

    // Button selection handler
    public void OnButtonSelect(Button button) 
    {
        if (selectedButton == button)
        {
            DeselectPreviousButton();
            return;
        }

        DeselectPreviousButton();
        SelectNewButton(button);
    }

    // Assignment button click handler
    public void OnAssignmentButtonClick(int i) 
    {
        if (selectedButton == null) return;

        var position = selectedButton.GetPosition();
        UpdateTileWithNewSprite(position.Item1, position.Item2, i);
        DeselectPreviousButton();
    }

    // Reset the game state
    public void ResetGame() 
    {
        UpdateAllItemSourceImages();
    }

    // Load data from files
    private void LoadData() 
    {
        
        Level levelInstance = Level.Instance;
        levelInstance.LoadLevelData(PlayerData.Instance.level);
        levelInstance.Print();
        indexValues = levelInstance.GetData();
    }
    private async Task LoadDataAsync()
{
    Debug.Log("LoadDataAsync() startet");
    Level levelInstance = Level.Instance;
    await levelInstance.LoadLevelDataAsync(PlayerData.Instance.level);
    Debug.Log("LoadDataAsync() fertig - Parse abgeschlossen");

    levelInstance.Print();
    indexValues = levelInstance.GetData();
    Debug.Log("Daten in indexValues übernommen");
}



    private bool IsDataValid() 
    {
        if (indexValues == null || indexValues.Count == 0) 
        {
            Debug.LogError("indexValues is null or empty. Ensure LoadData() is working correctly.");
            return false;
        }

        if (TileManager.Instance == null) 
        {
            Debug.LogError("TileManager.Instance is null. Ensure TileManager is present in the scene.");
            return false;
        }

        if (spriteElements == null || spriteElements.Count == 0) 
        {
            Debug.LogError("spriteElements is null or empty. Ensure sprites are assigned in the inspector.");
            return false;
        }

        return true;
    }

    private void UpdateTile(int x, int y) 
    {
        int index = indexValues[x][y];
        if (index < 0 || index >= spriteElements.Count) return;

        TileManager.Instance.ReplaceItemSourceImage(x, y, spriteElements[index], index + 1);
        TileManager.Instance.UpdateTileBlockState(x, y, index > 0 ? index - 1 : -1);
    }

    private void DeselectPreviousButton() 
    {
        if (selectedButton == null) return;

        var position = selectedButton.GetPosition();
        TileManager.Instance.ToggleSelectionBlock(position.Item1, position.Item2);
        selectedButton = null;
    }

    private void SelectNewButton(Button button) 
    {
        if (selectedButton == button) return;

        selectedButton = button;
        var position = button.GetPosition();
        TileManager.Instance.ToggleSelectionBlock(position.Item1, position.Item2);
    }

    private void UpdateTileWithNewSprite(int x, int y, int spriteIndex) 
    {
        TileManager.Instance.ReplaceItemSourceImage(x, y, spriteElements[spriteIndex], spriteIndex + 1);
        indexValues[x][y] = spriteIndex;
    }

    public void PlayGameOverSound()
    {
        if (gameOverSound != null)
        {
            audioSource.PlayOneShot(gameOverSound);
        }
        else
        {
            Debug.LogWarning("Game Over sound not assigned in the inspector.");
        }
    }

    public void GameOver()
    {
        Debug.Log("Game Over! The mistake limit has been reached.");
        TileManager.Instance.ActivateBlockersForAllTiles();
        TimeManager.Instance.PauseTimer();
        setPlayTime();
        BoosterButtonsController.Instance.SetAllButtonsInteractable(false);
        setPopupBackgroundActivation(true);
        activateGameOverPopup();
    }

    private void activateGameOverPopup()
    {
        if (gameOverPopup != null)
        {
            gameOverPopup.SetActive(true);
        }
        else
        {
            Debug.LogError("GameOverPopup is not assigned to the GameManager.");
        }
    }

    public void fadeInPopupBackground()
    {
        if (popupBackground != null)
        {
            popupBackground.SetActive(true);
            fader = popupBackground.GetComponent<Fader>();

            if (fader != null)
            {
                fader.FadeIn();
            }
            else
            {
                Debug.LogError("Fader component not found on popupBackground");
            }
        }
        else
        {
            Debug.LogError("popupBackground not assigned in the Inspector.");
        }
    }

    public void setPopupBackgroundActivation(bool value)
    {
        if (popupBackground != null)
        {
            popupBackground.SetActive(value);
        }
        else
        {
            Debug.LogError("popupBackground is not assigned to the GameManagerObject.");
        }
    }

    public void fadeOutPopupBackground()
    {
        if (popupBackground != null)
        {
            fader = popupBackground.GetComponent<Fader>();

            if (fader != null)
            {
                fader.FadeOut();
            }
            else
            {
                Debug.LogError("Fader component not found on popupBackground");
            }
        }
        else
        {
            Debug.LogError("popupBackground not assigned in the Inspector.");
        }
    }

    public void setPlayTime()
    {
        if (playTimeWin != null)
        {
            playTimeWin.text = TimeManager.Instance.getTextMesh();
        }
        else
        {
            Debug.LogError("playTimeWin TextMeshProUGUI not assigned in the Inspector.");
        }
        if (playTimeFail != null)
        {
            playTimeFail.text = TimeManager.Instance.getTextMesh();
        }
        else
        {
            Debug.LogError("playTimeFail TextMeshProUGUI not assigned in the Inspector.");
        }
    }
}