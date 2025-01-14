using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    [SerializeField] int value;
    [SerializeField] int x;
    [SerializeField] int y;
    // creates an indexed list of all background elements for each tile
    [SerializeField] private List<GameObject> backgroundElements = new List<GameObject>();


    public (int, int) GetPosition(){
        return (this.x, this.y);
    }
    public GameObject GetBackgroundElement(int index)
{
    if (index >= 0 && index < backgroundElements.Count)
    {
        return backgroundElements[index];
    }
    else
    {
        // Debug.LogError("Index out of range for backgroundElements list!");
        return null;
    }
}
    public void DeactivateAllBackgroundElements()
{
    foreach (GameObject obj in backgroundElements)
    {
        obj.SetActive(false);
    }
}


    public int GetValue(){
        return value;
    }

}
