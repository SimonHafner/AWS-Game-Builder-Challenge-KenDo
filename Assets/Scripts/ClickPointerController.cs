using System.Collections;
using UnityEngine;

public class ClickPointerController : MonoBehaviour
{
    [SerializeField]
    GameObject parent;

    [SerializeField]
    public SpriteRenderer mySpriteRenderer;

    // Fade duration for the Cosine fade-out effect
    private float fadeDuration = 0.3f;

    public void FadeOutSprite()
    {
        if (mySpriteRenderer != null)
        {
            StartCoroutine(FadeOutCoroutine());
        }
        else
        {
            Debug.LogWarning("mySpriteRenderer is not assigned.");
        }
    }

    private IEnumerator FadeOutCoroutine()
    {
        float elapsedTime = 0f;
        Color originalColor = mySpriteRenderer.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Cos((elapsedTime / fadeDuration) * Mathf.PI / 2); // Cosine curve for smooth fading out
            mySpriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }

        // Ensure the final alpha is set to 0
        mySpriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);

        // Call the method to delete the parent object
        DeactivatePointer();
    }

    void DeactivatePointer()
    {
        if (parent != null)
        {
            Destroy(parent);
        }
        else
        {
            Debug.LogWarning("Parent GameObject is not assigned.");
        }
    }

    // Function to set the parent active
    public void ActivateParent()
    {
        if (parent != null)
        {
            parent.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Parent GameObject is not assigned.");
        }
    }
}
