// TextboxController.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading.Tasks;

public class TextboxController : MonoBehaviour
{
    [SerializeField] 
    private TextMeshProUGUI tutorialInterfaceText;
    [SerializeField]
    public TutorialManager TutorialManager;
    private List<string> tutorialTextList;
    private int index = 0;
    [SerializeField]
    public List<ClickPointerController> clickPointerController;

    [SerializeField]
    private float fadeDuration = 0.15f;
    private bool fadeActive = false;

    [Header("Additional UI Elements")]
    [SerializeField] 
    private GameObject objectToFadeIn;
    [SerializeField] 
    private float waitTimeBeforeFade = 1f;
    [SerializeField] 
    private float pulseFrequency = 1f;
    private CanvasGroup fadeInCanvasGroup;
    private Coroutine pulseCoroutine;
    private Coroutine fadeInCoroutine;

    private async void Start()
    {
        // Asynchrones Initialisieren des Tutorial-Texts
        await InitializeText();

        // Zusätzliche UI-Objekte vorbereiten
        if (objectToFadeIn != null)
        {
            fadeInCanvasGroup = objectToFadeIn.GetComponent<CanvasGroup>();
            if (fadeInCanvasGroup != null)
            {
                fadeInCanvasGroup.alpha = 0f;
                fadeInCoroutine = StartCoroutine(FadeInAdditionalObject());
            }
        }
    }

    public int GetIndex()
    {
        return index;
    }

    private async Task InitializeText()
    {
        // Holt asynchron den Text aus der TutorialText-Klasse
        tutorialTextList = await TutorialText.Instance.GetTextListAsync(PlayerData.Instance.gameSettings.language);

        if (tutorialTextList != null && tutorialTextList.Count > 0)
        {
            UpdateText(index);
        }
        else
        {
            Debug.LogWarning("No tutorial text found for the selected language.");
        }
    }

    public void UpdateText(int idx)
    {
        if (tutorialInterfaceText != null 
            && tutorialTextList != null 
            && idx >= 0 
            && idx < tutorialTextList.Count)
        {
            tutorialInterfaceText.text = tutorialTextList[idx];

            // Bestimmte Indizes triggern Aktionen im TutorialManager
            if (idx == 2)
            {
                TutorialManager.HighlightFirstRow();
            }
            if (idx == 4)
            {
                TutorialManager.HighlightLowerRow();
            }
        }
        else
        {
            Debug.LogWarning("Invalid tutorial text index or tutorialInterfaceText is not assigned.");
        }
    }

    public void FadeOutText()
    {
        if (tutorialInterfaceText != null)
        {
            StartCoroutine(FadeOutCoroutine());
        }
        else
        {
            Debug.LogWarning("tutorialInterfaceText is not assigned.");
        }
    }

    public void FadeInText()
    {
        if (tutorialInterfaceText != null)
        {
            StartCoroutine(FadeInCoroutine());
        }
        else
        {
            Debug.LogWarning("tutorialInterfaceText is not assigned.");
        }
    }

    private IEnumerator FadeOutCoroutine()
    {
        fadeActive = true;
        float elapsedTime = 0f;
        Color originalColor = tutorialInterfaceText.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Cos((elapsedTime / fadeDuration) * Mathf.PI / 2);
            tutorialInterfaceText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        tutorialInterfaceText.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        fadeActive = false;
    }

    private IEnumerator FadeInCoroutine()
    {
        fadeActive = true;
        float elapsedTime = 0f;
        Color originalColor = tutorialInterfaceText.color;
        originalColor.a = 0f;
        tutorialInterfaceText.color = originalColor;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = 1.0f - Mathf.Cos((elapsedTime / fadeDuration) * Mathf.PI / 2);
            tutorialInterfaceText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        tutorialInterfaceText.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1f);
        fadeActive = false;
    }

    public void GoToNextText()
    {
        // Beispielhafte Aktionen beim Weiterschalten
        if (index == 1)
        {
            clickPointerController[0].ActivateParent();
            TutorialManager.shineControllerElements[0].ActivateCanvasGroupObject();
        }
        if (index == 3)
        {
            clickPointerController[2].ActivateParent();
            TutorialManager.shineControllerElements[2].ActivateCanvasGroupObject();
        }

        if (!fadeActive)
        {
            StartCoroutine(TransitionToNextText());
        }
    }

    private IEnumerator TransitionToNextText()
    {
        fadeActive = true;

        // Eventuelle laufende Koroutinen stoppen
        if (pulseCoroutine != null)
        {
            StopCoroutine(pulseCoroutine);
            pulseCoroutine = null;
        }
        if (fadeInCoroutine != null)
        {
            StopCoroutine(fadeInCoroutine);
            fadeInCoroutine = null;
        }

        // Sowohl Text als auch zusätzliches Objekt ausfaden
        yield return StartCoroutine(FadeOutBothCoroutine());

        // Nächstes Element in der Liste
        index = (index + 1) % tutorialTextList.Count;
        UpdateText(index);

        // Einfaden des neuen Textes
        yield return StartCoroutine(FadeInCoroutine());
        fadeActive = false;

        // Für bestimmte Indizes das Objekt erneut einblenden
        if (objectToFadeIn != null && fadeInCanvasGroup != null)
        {
            if (index != 2 && index != 4 && index != tutorialTextList.Count - 1)
            {
                fadeInCoroutine = StartCoroutine(FadeInAdditionalObject());
            }
        }
    }

    private IEnumerator FadeOutBothCoroutine()
    {
        float elapsedTime = 0f;
        Color originalColor = tutorialInterfaceText.color;
        float originalAlpha = (fadeInCanvasGroup != null) ? fadeInCanvasGroup.alpha : 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Cos((elapsedTime / fadeDuration) * Mathf.PI / 2);

            tutorialInterfaceText.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            if (fadeInCanvasGroup != null)
            {
                fadeInCanvasGroup.alpha = alpha * originalAlpha;
            }
            yield return null;
        }

        tutorialInterfaceText.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        if (fadeInCanvasGroup != null)
        {
            fadeInCanvasGroup.alpha = 0f;
        }
    }

    private IEnumerator FadeInAdditionalObject()
    {
        fadeInCanvasGroup.alpha = 0f;

        // Kurz warten, bevor das Objekt eingeblendet wird
        yield return new WaitForSeconds(waitTimeBeforeFade);

        if (fadeActive)
        {
            fadeInCoroutine = null;
            yield break;
        }

        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            if (fadeActive)
            {
                fadeInCoroutine = null;
                yield break;
            }

            elapsedTime += Time.deltaTime;
            float alpha = 1.0f - Mathf.Cos((elapsedTime / fadeDuration) * Mathf.PI / 2);
            fadeInCanvasGroup.alpha = alpha;
            yield return null;
        }

        // Nach erfolgreichem Einblenden beginnt die Puls-Animation
        if (!fadeActive)
        {
            fadeInCanvasGroup.alpha = 1f;
            pulseCoroutine = StartCoroutine(PulseObject());
        }

        fadeInCoroutine = null;
    }

    private IEnumerator PulseObject()
    {
        while (true)
        {
            float elapsed = 0f;
            while (elapsed < pulseFrequency)
            {
                elapsed += Time.deltaTime;
                float normalizedTime = elapsed / pulseFrequency;
                float alpha = 0.75f + 0.25f * Mathf.Sin(normalizedTime * 2f * Mathf.PI);
                fadeInCanvasGroup.alpha = alpha;
                yield return null;
            }
        }
    }
}
