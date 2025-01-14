using System.Collections;
using UnityEngine;

public class ShineController : MonoBehaviour
{
    // Referenz auf die CanvasGroup Component
    [SerializeField]
    private CanvasGroup canvasGroup;

    // Dauer des Fade-Out Effekts
    private float fadeDuration = 0.3f;

    // Methode, um den Fade-Out zu starten
    public void FadeOut()
    {
        if (canvasGroup != null)
        {
            StartCoroutine(FadeOutCoroutine());
        }
        else
        {
            Debug.LogWarning("CanvasGroup Component ist nicht zugewiesen!");
        }
    }

    // Coroutine, um den Alpha-Wert der CanvasGroup mit einer Cosinus-Kurve zu faden
    private IEnumerator FadeOutCoroutine()
    {
        float elapsedTime = 0f;
        float initialAlpha = canvasGroup.alpha;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Cos((elapsedTime / fadeDuration) * Mathf.PI / 2) * initialAlpha; // Cosinus-Kurve für sanftes Ausblenden
            canvasGroup.alpha = alpha;
            yield return null;
        }

        // Sicherstellen, dass der Alpha-Wert am Ende 0 ist
        canvasGroup.alpha = 0f;
    }

    // Methode, um das GameObject, das die CanvasGroup enthält, zu aktivieren
    public void ActivateCanvasGroupObject()
    {
        if (canvasGroup != null)
        {
            // Aktiviere das GameObject, das die CanvasGroup enthält
            canvasGroup.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("CanvasGroup Component ist nicht zugewiesen!");
        }
    }
}
