using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialEndgame : MonoBehaviour
{
    [SerializeField]
    public GameObject UIElement;

    // Start is called before the first frame update
    void Start()
    {
        UIElement.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
