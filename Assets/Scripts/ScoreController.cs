using UnityEngine;
using TMPro;

public class ScoreController : MonoBehaviour
{
    public static ScoreController Instance { get; private set; } // Singleton-Pattern

    [SerializeField] private TextMeshProUGUI scoreText; // Reference to the TextMeshPro component
    [SerializeField] private int mistakeLimit = 3;
    public int mistakeCount;

    private void Awake() 
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else 
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Set the initial color to #FFEC99
        scoreText.color = new Color32(0xFF, 0xEC, 0x99, 0xFF); // Equivalent to #FFEC99

        UpdateScore();
    }

    public void UpdateMistakeCount(int newMistakeCount)
    {
        mistakeCount = newMistakeCount;
        UpdateScore();

        // Check if the mistake count reaches or exceeds the limit
        if (mistakeCount >= mistakeLimit)
        {
            // If the mistake count reaches or exceeds the limit, set the color to
            scoreText.color = new Color32(0xFF, 0x9E, 0x8F, 0xFF); // Equivalent to Red
            GameManager.Instance.GameOver(); // Call GameOver once
        }
        else if ((float)mistakeCount / mistakeLimit > 0.5f)
        {
            // If the mistake ratio exceeds the threshold, set the color to
            scoreText.color = new Color32(0xFF, 0xC7, 0x88, 0xFF); // Equivalent to Orange
        }
        else
        {
            // Otherwise, set the color back to the default #FFEC99
            scoreText.color = new Color32(0xFF, 0xEC, 0x99, 0xFF); // Equivalent to Yellow
        }
    }

    private void UpdateScore()
    {
        scoreText.text = mistakeCount + "/" + mistakeLimit;
    }
}
