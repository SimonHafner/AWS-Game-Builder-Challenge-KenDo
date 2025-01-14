using UnityEngine;
using UnityEngine.UI;

public class KeyInputHandler : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Button button1;
    [SerializeField] private UnityEngine.UI.Button button2;
    [SerializeField] private UnityEngine.UI.Button button3;
    [SerializeField] private UnityEngine.UI.Button buttonDelete;
    [SerializeField] private UnityEngine.UI.Button buttonOption;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            button1.onClick.Invoke();
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            button2.onClick.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            button3.onClick.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            buttonDelete.onClick.Invoke();
        }
        
        if (Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.RightAlt))
        {
            buttonOption.onClick.Invoke();
        }
    }
}
