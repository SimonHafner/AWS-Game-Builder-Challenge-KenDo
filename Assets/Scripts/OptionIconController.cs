using UnityEngine;
using DG.Tweening;

public class OptionIconController : MonoBehaviour
{
    [SerializeField] private CanvasController canvasController;
    [SerializeField] private RectTransform imageIcon;
    
    [Header("Audio References")]
    [SerializeField] private AudioSource optionsOnSound;
    [SerializeField] private AudioSource optionsOffSound;
    
    [Header("Animation Settings")]
    [SerializeField] private float animationDuration = 0.6f;

    private bool buttonOn = false;
    private Tween currentTween;

    public void PlayAnimation()
    {
        if (currentTween != null)
        {
            currentTween.Kill();
        }

        buttonOn = !buttonOn;

        // Play appropriate sound effect
        if (buttonOn)
        {
            canvasController.MoveToForeground(6);
            if (optionsOnSound != null)
            {
                optionsOnSound.Play();
            }
        }
        else
        {
            canvasController.MoveToBackground();
            if (optionsOffSound != null)
            {
                optionsOffSound.Play();
            }
        }

        float startValue = buttonOn ? 0f : -180f;
        float endValue = buttonOn ? -180f : 0f;

        currentTween = DOTween.To(
            () => startValue,
            x => imageIcon.localRotation = Quaternion.Euler(0, 0, x),
            endValue,
            animationDuration
        ).SetEase(Ease.InOutSine);
    }

    private void OnDestroy()
    {
        if (currentTween != null)
        {
            currentTween.Kill();
        }
    }
}