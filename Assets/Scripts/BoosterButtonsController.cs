using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoosterButtonsController : MonoBehaviour
{
    public static BoosterButtonsController Instance { get; private set; }
    [SerializeField] private List<UnityEngine.UI.Button> boosterButtons;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetAllButtonsInteractable(bool state)
{
    foreach (UnityEngine.UI.Button buttonComponent in boosterButtons)
    {
        if (buttonComponent != null)
        {
            buttonComponent.interactable = state;  // This will make the button interactable or non-interactable
        }
        else
        {
            Debug.LogWarning("Button component not found on the GameObject.");
        }
    }
}

}
