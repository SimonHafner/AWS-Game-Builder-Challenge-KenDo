using UnityEngine;
using UnityEngine.UI; // Make sure to include this if you're using UI elements.

public class PopupBackgroundController : MonoBehaviour
{
    // Variable to hold the Material that will be applied to the background
    public Material backgroundMaterial;

    private Image popupImage; // Reference to the Image component on the PopupBackground

    void Start()
    {
        // Get the Image component from the current GameObject
        popupImage = GetComponent<Image>();

        // Assign the material to the Image component
        if (popupImage != null && backgroundMaterial != null)
        {
            popupImage.material = backgroundMaterial;
        }
    }

    void OnEnable()
    {
        // Ensure that the material is reapplied whenever the object is re-enabled
        if (popupImage != null && backgroundMaterial != null)
        {
            popupImage.material = backgroundMaterial;
        }
    }
}
