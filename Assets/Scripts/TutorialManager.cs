using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Ricimi;

public class TutorialManager : MonoBehaviour
{
    [SerializeField]
    private GameObject canvas;

    [SerializeField]
    public List<ShineController> shineControllerElements;

    [SerializeField]
    public GameObject tutorialBackground;

    [SerializeField]
    public GameObject UIElement;

    [SerializeField]
    public TextboxController TextboxController;
    
    [SerializeField]
    public UnityEngine.UI.Button backgroundButton;

    [SerializeField]
    public List<CanvasController> firstTableRow;

    [SerializeField]
    public List<CanvasController> lowerTableRow;

    [SerializeField]
    public CanvasController CanvasControllerButtonOne;

    [SerializeField]
    public CanvasController CanvasControllerButtonThree;

    [SerializeField]
    public List<GameObject> UIElementGameObjects; // all game objects using Canvas components 

    [SerializeField]
    private AudioSource audioTutorialEnd;

    [SerializeField]
    private AudioSource audioTimer;


    private bool tutorialEnd = false;

    public void HighlightFirstRow()
    {
        foreach (CanvasController i in firstTableRow)
        {
            i.MoveToForeground(0);
        }

        if (firstTableRow.Count > 1)
        {
            firstTableRow[1].MoveToForeground(1);
        }
        SetBackgroundButtonActive(false);
    }

    public void HighlightLowerRow(){
        lowerTableRow[0].MoveToForeground(0);
        lowerTableRow[1].MoveToForeground(0);
        lowerTableRow[2].MoveToForeground(1);
        SetBackgroundButtonActive(false);
    }

    public void SelectUpperField(){
        if(!tutorialEnd){
            TextboxController.clickPointerController[0].FadeOutSprite();
            shineControllerElements[0].FadeOut();
            TextboxController.clickPointerController[1].ActivateParent();
            shineControllerElements[1].ActivateCanvasGroupObject();
            firstTableRow[1].MoveToForeground(0);
            CanvasControllerButtonOne.MoveToForeground(1);
        }
    }

    public void SelectLowerField(){
        if(!tutorialEnd){
            TextboxController.clickPointerController[2].FadeOutSprite();
            shineControllerElements[2].FadeOut();
            TextboxController.clickPointerController[3].ActivateParent();
            shineControllerElements[3].ActivateCanvasGroupObject();
            lowerTableRow[2].MoveToForeground(0);
            CanvasControllerButtonThree.MoveToForeground(1);
        }
    }

    public void SelectButtonOne(){
        if(!tutorialEnd){
            TextboxController.clickPointerController[1].FadeOutSprite();
            shineControllerElements[1].FadeOut();
            CanvasControllerButtonOne.MoveToBackground();
            foreach (CanvasController i in firstTableRow)
            {
                i.MoveToBackground();
            }
            TextboxController.GoToNextText();
            SetBackgroundButtonActive(true);
        }
    }

    public void SelectButtonThree(){
        if(!tutorialEnd){
            TextboxController.clickPointerController[3].FadeOutSprite();
            shineControllerElements[3].FadeOut();
            CanvasControllerButtonThree.MoveToBackground();
            foreach (CanvasController i in lowerTableRow)
            {
                i.MoveToBackground();
            }
            TextboxController.GoToNextText();
            Fader fader = tutorialBackground.GetComponent<Fader>();
            fader.FadeOut();
            RemoveCanvasComponents();
            tutorialEnd = true;
            audioTutorialEnd.Play();
            audioTimer.Play();
            TimeManager.Instance.ResumeTimer();
        }
    }

    public void SetBackgroundButtonActive(bool isActive)
{
    if (backgroundButton != null)
    {
        backgroundButton.interactable = isActive;
    }
}
    public void DeactivateUIElement(){
        UIElement.SetActive(false);
    }

    public void RemoveCanvasComponents() // This function is called by the end of the tutorial
{
    foreach (GameObject element in UIElementGameObjects)
    {
        Canvas canvas = element.GetComponent<Canvas>();
        if (canvas != null)
        {
            // Remove the Canvas component
            Destroy(canvas);
        }
    }
}


}
