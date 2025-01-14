using System.Collections;
using UnityEngine;

public class StarController : MonoBehaviour
{
    [SerializeField] private AnimationClip fadeAnimationClip; // Referenz auf den AnimationClip
    private Animation anim; // Lokale Variable für die Animation-Komponente
    private bool isPlaying = false; // Initialisiert, um zu verfolgen, ob eine Animation spielt
    private float timer = 0f; // Timer, um zu verfolgen, wie lange das Programm läuft
    [SerializeField] private AudioSource audioSourceFade;

    void Awake()
    {
        anim = GetComponent<Animation>();
    }

    void Start()
    {
        if (fadeAnimationClip != null)
        {
            anim.AddClip(fadeAnimationClip, fadeAnimationClip.name);
            anim[fadeAnimationClip.name].wrapMode = WrapMode.ClampForever;
        }
        else
        {
            Debug.LogError("Fade Animation Clip ist nicht im Inspektor zugewiesen.");
        }
    }

    void Update()
    {
    }

    // Methode, um die Animation zu starten
    public void StartFadeAnimation()
    {
        // Verwende direkt fadeAnimationClip.name, anstatt es als Parameter zu übergeben
        if (anim != null && fadeAnimationClip != null && !isPlaying)
        {
            audioSourceFade.Play(); // Play Fade Audio
            anim.Play(fadeAnimationClip.name);
            isPlaying = true;
            StartCoroutine(CheckAnimationEnd());
        }
    }

    // Coroutine, um das Ende der Animation zu prüfen
    IEnumerator CheckAnimationEnd()
    {
        yield return new WaitForSeconds(fadeAnimationClip.length);
        AnimationFinished();
    }

    // Diese Methode wird aufgerufen, wenn die Animation beendet ist
    void AnimationFinished()
    {
        isPlaying = false;
    }
}
