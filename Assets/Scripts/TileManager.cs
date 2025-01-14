using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TileManager : MonoBehaviour
{
    public static TileManager Instance { get; private set; }
    private int[,] tileMatrix;
    private const int matrixSize = 6;
    private List<(int, int)> errorList;
    private List<(int, int)> errorListPrev;

    private int mistakesCount = 0;

    [SerializeField] private AudioClip winSound;
    [SerializeField] private AudioSource mistakeSound;
    private AudioSource audioSource;

    public int GetMistakeCount()
{
    return mistakesCount;
}


    private void Awake() 
    {
        if (Instance == null) 
        {
            Instance = this;
            InitializeTileMatrix();
            audioSource = gameObject.AddComponent<AudioSource>(); // Initialize the AudioSource component
        } 
        else 
        {
            Debug.LogWarning("Multiple instances of TileManager detected! Destroying duplicate.");
            Destroy(gameObject);
        }
    }

    // Initialize the tile matrix with empty values
    private void InitializeTileMatrix() 
    {
        tileMatrix = new int[matrixSize, matrixSize];
        errorList = new List<(int, int)>();
        errorListPrev = new List<(int, int)>();

        for (int x = 0; x < matrixSize; x++) 
        {
            for (int y = 0; y < matrixSize; y++) 
            {
                tileMatrix[x, y] = 0;
            }
        }

        PrintTileMatrix();
    }

    // Replace the image on the specified tile and update its value in the matrix
    public void ReplaceItemSourceImage(int x, int y, Sprite sprite, int value) 
    {
        var tile = FindTile(x, y);
        if (tile == null) return;

        var image = GetTileImage(tile);
        if (image != null) 
        {
            image.sprite = sprite;
            tileMatrix[x, y] = value - 1;

            ValidateTile(x, y);
            PrintTileMatrix();
            PrintErrorList();
            CheckWin();
        }
    }

    // Check for new errors compared to the previous error list
    private bool CheckForNewErrors()
    {
        foreach (var error in errorList)
        {
            if (!errorListPrev.Contains(error)) // If there's a new error that wasn't part of the previous error list
            {
                return true;
            }
        }
        return false;
    }
    public void PlayMistakeSound()
{
    if (mistakeSound != null)
    {
        mistakeSound.PlayOneShot(mistakeSound.clip);  // Explizit AudioSource.PlayOneShot verwenden
    }
    else
    {
        Debug.LogWarning("Mistake sound AudioSource not assigned in the inspector.");
    }
}

    // Validate the state of the tile after updating it
    private void ValidateTile(int x, int y) 
    {
        errorListPrev = new List<(int, int)>(errorList); // Update the previous error list with a copy of the current error list
        errorList.Clear();

        for (int i = 0; i < matrixSize; i++) 
        {
            for (int j = 0; j < matrixSize; j++) 
            {
                if (tileMatrix[i, j] == 0) continue;

                ValidateRow(i);
                ValidateColumn(j);
                ValidateRowPairs(i);
                ValidateColumnPairs(j);
            }
        }

        RemoveDuplicateErrors();

        if (CheckForNewErrors())
        {
            ActivateAllRedShinesFromErrorList();
            mistakesCount += 1;
            if(mistakesCount < Level.Instance.mistakesLimit){
                PlayMistakeSound();
            }
            ScoreController.Instance.UpdateMistakeCount(mistakesCount);
            Debug.Log("New error detected! Current error count: " + mistakesCount);
        }
        DeactivateRedShineForSolvedErrors();
        PrintErrorList();
    }

    // Validate a specific row for rule violations
    private void ValidateRow(int row) 
    {
        int[] count = CountElementsInRow(row);
        CheckRowOrColumnErrors(row, count, true);
    }

    // Validate a specific column for rule violations
    private void ValidateColumn(int col) 
    {
        int[] count = CountElementsInColumn(col);
        CheckRowOrColumnErrors(col, count, false);
    }

    // Validate row-wise pairs in the matrix
    private void ValidateRowPairs(int row) 
    {
        for (int k = 0; k < matrixSize - 2; k++) 
        {
            if (IsThreeInARow(tileMatrix[row, k], tileMatrix[row, k + 1], tileMatrix[row, k + 2])) 
            {
                AddErrorsToList((row, k), (row, k + 1), (row, k + 2));
            }
        }
    }

    // Validate column-wise pairs in the matrix
    private void ValidateColumnPairs(int col) 
    {
        for (int k = 0; k < matrixSize - 2; k++) 
        {
            if (IsThreeInARow(tileMatrix[k, col], tileMatrix[k + 1, col], tileMatrix[k + 2, col])) 
            {
                AddErrorsToList((k, col), (k + 1, col), (k + 2, col));
            }
        }
    }

    // Check for errors in a row or column and update the error list
    private void CheckRowOrColumnErrors(int index, int[] count, bool isRow) 
    {
        if (count[0] > 1) AddErrorsToList(index, 1, isRow);
        if (count[1] > 2) AddErrorsToList(index, 2, isRow);
        if (count[2] > 3) AddErrorsToList(index, 3, isRow);
    }

    // Count occurrences of each value in a specific row
    private int[] CountElementsInRow(int row) 
    {
        int[] count = new int[3];
        for (int k = 0; k < matrixSize; k++) 
        {
            IncrementCountIfValid(tileMatrix[row, k], count);
        }
        return count;
    }

    // Count occurrences of each value in a specific column
    private int[] CountElementsInColumn(int col) 
    {
        int[] count = new int[3];
        for (int k = 0; k < matrixSize; k++) 
        {
            IncrementCountIfValid(tileMatrix[k, col], count);
        }
        return count;
    }

    // Helper method to increment the count array if the value is valid
    private void IncrementCountIfValid(int value, int[] count) 
    {
        if (value > 0 && value <= 3) 
        {
            count[value - 1]++;
        }
    }

    // Check if three consecutive values are the same
    private bool IsThreeInARow(int v1, int v2, int v3) 
    {
        return v1 == v2 && v2 == v3 && v1 > 0;
    }

    // Add errors to the list, avoiding duplicates
    private void AddErrorsToList(params (int, int)[] positions) 
    {
        foreach (var pos in positions) 
        {
            errorList.Add(pos);
        }
    }

    // Add errors to the list based on row/column rules
    private void AddErrorsToList(int index, int value, bool isRow) 
    {
        for (int i = 0; i < matrixSize; i++) 
        {
            if (isRow && tileMatrix[index, i] == value) 
            {
                errorList.Add((index, i));
            } 
            else if (!isRow && tileMatrix[i, index] == value) 
            {
                errorList.Add((i, index));
            }
        }
    }

    // Remove duplicate errors from the error list
    private void RemoveDuplicateErrors() 
    {
        errorList = new List<(int, int)>(new HashSet<(int, int)>(errorList));
    }

    // Check if the game is won by evaluating the current matrix and error list
    
    // Method to play the win sound
    public void PlayWinSound()
    {
        if (winSound != null)
        {
            audioSource.PlayOneShot(winSound);
        }
        else
        {
            Debug.LogWarning("Win sound not assigned in the inspector.");
        }
    }
    
    public void Win(){
        Debug.Log("Game won!");
        PlayerData.Instance.level += 1;
        PlayerData.Instance.stars += TimeManager.Instance.getStarsCount();
        PlayerData.Instance.SavePlayerData(); // Save on database layer
        TileManager.Instance.ActivateBlockersForAllTiles();
        TileManager.Instance.UpdateAllButtonBackgrounds();
        TimeManager.Instance.PauseTimer();
        GameManager.Instance.setPlayTime();
        BoosterButtonsController.Instance.SetAllButtonsInteractable(false);
        GameManager.Instance.setPopupBackgroundActivation(true);
        GameManager.Instance.puzzleSolvedPopup.SetActive(true);
        //PlayWinSound(); // Play winning sound
        // Show celebration screen ...
    }
    public void CheckWin() 
    {
        int zeroCount = CountZerosInMatrix();

        if (errorList.Count == 0 && zeroCount == 0) // Winning condition
        {
            Win();
        } 
        else 
        {
            Debug.Log("Game not yet won.");
        }
    }

    // Count the number of zeros in the matrix
    private int CountZerosInMatrix() 
    {
        int zeroCount = 0;
        for (int x = 0; x < matrixSize; x++) 
        {
            for (int y = 0; y < matrixSize; y++) 
            {
                if (tileMatrix[x, y] == 0) 
                {
                    zeroCount++;
                }
            }
        }
        return zeroCount;
    }

    // Update the visual state of a tile based on its block index
    public void UpdateTileBlockState(int x, int y, int blockIndex) 
    {
        var tile = FindTile(x, y);
        if (tile == null) return;

        var button = GetTileButton(tile);
        if (button != null) 
        {
            UpdateButtonBackground(button, blockIndex);
            UpdateBlockerState(tile, blockIndex);
        }
    }

    // Toggle the selection block of a tile
    public void ToggleSelectionBlock(int x, int y) 
    {
        var tile = FindTile(x, y);
        if (tile == null) return;

        ToggleGameObjectState(tile, "Button/SelectionBlock");
    }

    // Private helper methods

    // Find a tile based on its coordinates
    private GameObject FindTile(int x, int y) 
    {
        string tileName = $"Tile - {x}-{y}";
        var tile = GameObject.Find(tileName);
        if (tile == null) 
        {
            Debug.LogError($"Tile '{tileName}' not found.");
        }
        return tile;
    }

    // Retrieve the Image component from a tile's button
    private Image GetTileImage(GameObject tile) 
    {
        var button = tile.transform.Find("Button");
        if (button != null) 
        {
            return button.Find("Item").GetComponent<Image>();
        }
        Debug.LogError("Image component not found.");
        return null;
    }

    // Retrieve the Button component from a tile
    private Button GetTileButton(GameObject tile) 
    {
        var button = tile.transform.Find("Button").GetComponent<Button>();
        if (button == null) 
        {
            Debug.LogError("Button component not found.");
        }
        return button;
    }

    // Update the background elements of a button based on block index
    private void UpdateButtonBackground(Button button, int blockIndex) 
    {
        if (blockIndex >= 0) 
        {
            button.GetBackgroundElement(blockIndex).SetActive(true);
        } 
        else 
        {
            button.DeactivateAllBackgroundElements();
        }
    }

    public void UpdateAllButtonBackgrounds()
{
    for (int x = 0; x < matrixSize; x++)
    {
        for (int y = 0; y < matrixSize; y++)
        {
            var tile = FindTile(x, y);
            if (tile != null)
            {
                var button = GetTileButton(tile);
                if (button != null)
                {
                    int blockIndex = tileMatrix[x, y];
                    UpdateButtonBackground(button, blockIndex-1);
                }
            }
        }
    }
}



    // Update the state of the blocker element on a tile
    private void UpdateBlockerState(GameObject tile, int blockIndex) 
    {
        var blocker = tile.transform.Find("Blocker")?.gameObject;
        if (blocker != null) 
        {
            blocker.SetActive(blockIndex >= 0);
        } 
        else 
        {
            Debug.LogError("Blocker not found.");
        }
    }

    public void ActivateBlockersForAllTiles()
{
    for (int x = 0; x < matrixSize; x++)
    {
        for (int y = 0; y < matrixSize; y++)
        {
            var tile = FindTile(x, y);
            if (tile != null)
            {
                UpdateBlockerState(tile, 1); // 1 aktiviert den Blocker
            }
        }
    }
}



    // Toggle the active state of a GameObject
    private void ToggleGameObjectState(GameObject tile, string path) 
    {
        var obj = tile.transform.Find(path)?.gameObject;
        if (obj != null) 
        {
            obj.SetActive(!obj.activeSelf);
        } 
        else 
        {
            Debug.LogError($"{path} not found.");
        }
    }

    // Print the current tile matrix to the console
    private void PrintTileMatrix() 
    {
        Debug.Log("Current Tile Matrix:");
        for (int x = 0; x < matrixSize; x++) 
        {
            string row = "";
            for (int y = 0; y < matrixSize; y++) 
            {
                row += $"{tileMatrix[x, y]} ";
            }
            Debug.Log(row);
        }
    }

    // Print the current error list to the console
    private void PrintErrorList() 
    {
        Debug.Log("ErrorList = " + string.Join(", ", errorList));
    }

    private Dictionary<(int, int), Tween> activeFadeOuts = new Dictionary<(int, int), Tween>();

    public void ActivateRedShine(int x, int y, bool activate)
    {
        var tile = FindTile(x, y);
        
        if (tile != null)
        {
            var shineRed = tile.transform.Find("Shine_Red")?.gameObject;

            if (shineRed != null)
            {
                // Wenn eine Fade-Out Animation läuft, diese abbrechen
                if (activeFadeOuts.TryGetValue((x, y), out var existingTween))
                {
                    existingTween.Kill();
                    activeFadeOuts.Remove((x, y));
                }

                var canvasGroup = shineRed.GetComponent<CanvasGroup>();
                if (activate)
                {
                    shineRed.SetActive(true);
                    canvasGroup.alpha = 1f;
                }
                else
                {
                    // Fade-Out Animation starten
                    var fadeOut = canvasGroup.DOFade(0f, 0.5f)
                        .OnComplete(() => 
                        {
                            shineRed.SetActive(false);
                            activeFadeOuts.Remove((x, y));
                        });
                    
                    activeFadeOuts.Add((x, y), fadeOut);
                }
            }
            else
            {
                Debug.LogError("Shine_Red not found on tile.");
            }
        }
        else
        {
            Debug.LogError($"Tile at coordinates ({x}, {y}) not found.");
        }
    }

    public void DeactivateRedShine(int x, int y) // Deactivates complete object, without any fade
{
    var tile = FindTile(x, y);
    if (tile != null)
    {
        var shineRed = tile.transform.Find("Shine_Red")?.gameObject;
        if (shineRed != null)
        {
            shineRed.SetActive(false);
        }
        else
        {
            Debug.LogError("Shine_Red not found on tile.");
        }
    }
    else
    {
        Debug.LogError($"Tile at coordinates ({x}, {y}) not found.");
    }
}


public void ActivateAllRedShinesFromErrorList()
{
    foreach (var error in errorList)
    {
        int x = error.Item1;
        int y = error.Item2;
        
        // Activate the red shine for each tile in the errorList
        DeactivateRedShine(x,y);
        ActivateRedShine(x, y,true);
    }
}

public void DeactivateRedShineForSolvedErrors()
{
    // Iteriere durch alle Werte von errorListPrev
    foreach (var errorPrev in errorListPrev)
    {
        int x = errorPrev.Item1;
        int y = errorPrev.Item2;

        // Prüfe, ob der aktuelle Wert in errorList vorhanden ist
        if (!errorList.Contains(errorPrev))
        {
            // Wenn nicht vorhanden, deaktiviere den Shine für das entsprechende Tile
            ActivateRedShine(x, y, false);
        }
    }
}

}
