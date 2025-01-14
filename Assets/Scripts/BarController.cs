using UnityEngine;
using UnityEngine.UI; // Wichtig für den Zugriff auf UI-Komponenten

public class BarController : MonoBehaviour
{
    private Image imageComponent;

    [SerializeField]
    public GameObject barAnimationObject;

    void Awake()
    {
        // Versuche, die Image-Komponente bei Initialisierung zu finden
        imageComponent = GetComponent<Image>();
        if (imageComponent == null)
        {
            Debug.LogError("Image-Komponente nicht gefunden!");
        }
    }

    // Öffentliche Methode zum Einstellen der Füllmenge
    public void setFillAmount(float value)
    {
        if (imageComponent != null)
        {
            imageComponent.fillAmount = value;
        }
    }
    public void deactivateFill()
{
    if (barAnimationObject != null)
    {
        barAnimationObject.SetActive(false); // Disable the Image component
    }
}

}
