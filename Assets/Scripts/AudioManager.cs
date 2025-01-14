// AudioManager.cs
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioMixer UIaudioMixer;
    private const string VolumeParameterName = "UIVolume";

    // Singleton Pattern
    public static AudioManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetUIVolume(bool isOn)
    {
        if (UIaudioMixer != null)
        {
            UIaudioMixer.SetFloat(VolumeParameterName, isOn ? 0f : -80f);
        }
        else
        {
            Debug.LogError("UI-AudioMixer reference is missing!");
        }
    }
}