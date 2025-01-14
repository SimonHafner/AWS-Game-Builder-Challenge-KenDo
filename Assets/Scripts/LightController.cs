using UnityEngine;

public class LightController : MonoBehaviour
{
    [Header("References")]
    public CanvasGroup targetCanvasGroup;  // Referenz zur CanvasGroup über Inspector

    [Header("Light Settings")]
    public float minAlpha = 0.3f;    // Minimale Helligkeit
    public float maxAlpha = 1.0f;    // Maximale Helligkeit
    public float cycleSpeed = 0.5f;  // Geschwindigkeit der Helligkeitsänderung
    public float phaseShift = 0.0f;  // Erlaubt Verschiebung der Sinuskurve
    
    private float timeElapsed;

    private void Start()
    {
        // Prüfen, ob eine CanvasGroup zugewiesen wurde
        if (targetCanvasGroup == null)
        {
            Debug.LogError("Bitte weise eine CanvasGroup im Inspector zu!");
            enabled = false;
            return;
        }
    }

    private void Update()
    {
        if (targetCanvasGroup == null) return;  // Sicherheitscheck

        // Zeit aktualisieren
        timeElapsed += Time.deltaTime;
        
        // Sinus-Wert zwischen -1 und 1 berechnen
        float sinValue = Mathf.Sin(timeElapsed * cycleSpeed + phaseShift);
        
        // Sinus-Wert auf den gewünschten Alpha-Bereich mappen
        float normalizedSin = Mathf.Lerp(minAlpha, maxAlpha, (sinValue + 1f) * 0.5f);
        
        // Alpha-Wert der CanvasGroup setzen
        targetCanvasGroup.alpha = normalizedSin;
    }
}