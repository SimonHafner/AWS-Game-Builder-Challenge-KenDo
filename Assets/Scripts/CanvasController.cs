using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
    [SerializeField] private GameObject parent;
    [SerializeField] private GameObject button;

    // Moves the parent and button GameObjects to the foreground with specified sorting layer
    public void MoveToForeground(int layer)
    {
        // Adding a Canvas to the parent if not already present
        Canvas parentCanvas = parent.GetComponent<Canvas>();
        if (parentCanvas == null)
        {
            parentCanvas = parent.AddComponent<Canvas>();
        }
        parentCanvas.overrideSorting = true;
        parentCanvas.sortingOrder = layer;

        // Adding a CanvasGroup to the parent for fading effect
        CanvasGroup canvasGroup = parent.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = parent.AddComponent<CanvasGroup>();
        }
        canvasGroup.alpha = 0.3f; // Initial alpha value

        // Start the fade-in effect
        StartCoroutine(FadeInCanvasGroup(canvasGroup, 0.3f));

        // Adding a Canvas and GraphicRaycaster to the button if not already present
        Canvas buttonCanvas = button.GetComponent<Canvas>();
        if (buttonCanvas == null)
        {
            buttonCanvas = button.AddComponent<Canvas>();
        }
        buttonCanvas.overrideSorting = true;
        buttonCanvas.sortingOrder = layer;

        // Adding GraphicRaycaster to the button if not already present
        if (button.GetComponent<GraphicRaycaster>() == null)
        {
            button.AddComponent<GraphicRaycaster>();
        }
    }

    // Coroutine for a smooth fade-in effect using a sine curve
    private IEnumerator FadeInCanvasGroup(CanvasGroup canvasGroup, float duration)
    {
        float elapsedTime = 0f;
        float startAlpha = canvasGroup.alpha;
        float targetAlpha = 1f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Sin((elapsedTime / duration) * Mathf.PI / 2) * (targetAlpha - startAlpha) + startAlpha; // Sinusförmige Kurve für sanftes Einblenden
            canvasGroup.alpha = alpha;
            yield return null;
        }

        // Ensure the final alpha is set to 1
        canvasGroup.alpha = targetAlpha;
    }

    // Moves the parent and button GameObjects back to the background by removing Canvas and GraphicRaycaster components
    public void MoveToBackground()
    {
        // Komplettes Entfernen der Canvas-Komponente vom parent
        Canvas parentCanvas = parent.GetComponent<Canvas>();
        if (parentCanvas != null)
        {
            DestroyImmediate(parentCanvas); // DestroyImmediate entfernt die Komponente vollständig
        }

        // Entfernen der CanvasGroup-Komponente vom parent
        CanvasGroup canvasGroup = parent.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            DestroyImmediate(canvasGroup);
        }

        // Zuerst den GraphicRaycaster, dann die Canvas-Komponente vom button entfernen
        GraphicRaycaster buttonRaycaster = button.GetComponent<GraphicRaycaster>();
        if (buttonRaycaster != null)
        {
            DestroyImmediate(buttonRaycaster); // Entfernt den GraphicRaycaster vollständig
        }

        Canvas buttonCanvas = button.GetComponent<Canvas>();
        if (buttonCanvas != null)
        {
            DestroyImmediate(buttonCanvas); // Entfernt die Canvas vollständig
        }
    }
}
