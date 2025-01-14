using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SoundButtonController : MonoBehaviour
{
    [SerializeField] private Image buttonImage;
    [SerializeField] private Sprite onSprite;
    [SerializeField] private Sprite offSprite;
    [SerializeField] private AudioMixer uiAudioMixer;
    private const string VolumeParameterName = "UIVolume";
    private bool buttonOn = true;

    void Start()
    {
        SetButtonConfig();
        UpdateButtonImage();
        UpdateUIAudio();
    }

    public void switchButton()
    {
        buttonOn = !buttonOn;
        UpdateUIAudio();
        UpdateButtonImage();
        SaveSettings();
        Debug.Log(buttonOn ? "Button is ON" : "Button is OFF");
    }

    private void UpdateUIAudio()
    {
        uiAudioMixer.SetFloat(VolumeParameterName, buttonOn ? 0f : -80f);
    }

    private void UpdateButtonImage()
    {
        if (buttonImage != null)
        {
            buttonImage.sprite = buttonOn ? onSprite : offSprite;
        }
        else
        {
            Debug.LogWarning("Button Image reference is missing.");
        }
    }

    private void SetButtonConfig()
    {
        if (PlayerData.Instance?.gameSettings != null)
        {
            buttonOn = PlayerData.Instance.gameSettings.sound;
        }
        else
        {
            Debug.LogWarning("PlayerData or gameSettings not initialized correctly.");
        }
    }

    private void SaveSettings()
    {
        if (PlayerData.Instance?.gameSettings != null)
        {
            PlayerData.Instance.gameSettings.sound = buttonOn;
            PlayerData.Instance.SavePlayerData();
        }
        else
        {
            Debug.LogWarning("PlayerData or gameSettings not initialized correctly.");
        }
    }
}
