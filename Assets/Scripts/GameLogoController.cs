using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Ricimi;

public class GameLogoController : MonoBehaviour
{
    private CanvasGroup canvasGroup;
    
    [Header("Dependencies")]
    [SerializeField]
    private SceneTransition sceneTransition; // Referenz auf SceneTransition Component
    
    [Header("Timing Parameters")]
    [SerializeField]
    private float initialDelay = 0.5f; // Kurze Pause vor Start der Animation
    
    [SerializeField]
    private float fadeDuration = 1.5f; // Sanfte Ein-/Ausblendung über 1.5 Sekunden
    
    [SerializeField]
    private float displayDuration = 2.0f; // Logo für 2 Sekunden sichtbar
    
    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f; // Start vollständig transparent
    }
    
    private void Start()
    {
        // Überprüfung ob SceneTransition zugewiesen wurde
        if (sceneTransition == null)
        {
            Debug.LogError("SceneTransition reference is missing on GameLogoController!");
            return;
        }
        
        StartCoroutine(LogoSequence());
    }
    
    private IEnumerator LogoSequence()
    {
        // Initiale Wartezeit
        yield return new WaitForSeconds(initialDelay);
        
        // Einblenden
        yield return StartCoroutine(FadeCanvas(0f, 1f, fadeDuration));
        
        // Sichtbar halten
        yield return new WaitForSeconds(displayDuration);
        
        // Ausblenden
        //yield return StartCoroutine(FadeCanvas(1f, 0f, fadeDuration));
        
        // Szene mit SceneTransition wechseln
        sceneTransition.ClickOnLogoBackground();
    }
    
    private IEnumerator FadeCanvas(float startAlpha, float targetAlpha, float duration)
    {
        float elapsedTime = 0f;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = elapsedTime / duration;
            
            // Sinusförmiger Übergang für weiche Blende
            float smoothValue = Mathf.Sin(normalizedTime * Mathf.PI * 0.5f);
            
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, smoothValue);
            yield return null;
        }
        
        canvasGroup.alpha = targetAlpha; // Sicherstellen dass der Zielwert erreicht wird
    }
}