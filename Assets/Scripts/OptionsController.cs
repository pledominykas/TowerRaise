using UnityEngine;
using UnityEngine.UI;

public class OptionsController : MonoBehaviour
{
    [SerializeField] GameObject OptionsButtons;
    [SerializeField] Image MusicButton;
    [SerializeField] Sprite MusicOnImage;
    [SerializeField] Sprite MusicOffImage;
    [SerializeField] Image VibrationButton;
    [SerializeField] Sprite VibrationOnImage;
    [SerializeField] Sprite VibrationOffImage;

    private int optionsButtonClickCount = 1;
    private int musicButtonClickCount = 1;
    private int vibrationButtonClickCount = 1;


    public void OnOptionsButtonClick()
    {
        GameController.am.PlaySound(AudioManager.SoundType.OnButtonClick);
        if (optionsButtonClickCount % 2 == 1)
        {
            OptionsButtons.SetActive(true);
        }
        else
        {
            OptionsButtons.SetActive(false);
        }
        optionsButtonClickCount++;
    }

    public void OnMusicButtonClick()
    {
        GameController.am.PlaySound(AudioManager.SoundType.OnButtonClick);
        if (musicButtonClickCount % 2 == 1)
        {
            MusicButton.sprite = MusicOffImage;
            AudioListener.pause = true;
        }
        else
        {
            MusicButton.sprite = MusicOnImage;
            AudioListener.pause = false;
        }
        musicButtonClickCount++;
    }

    public void OnVibrationButtonClick()
    {
        GameController.am.PlaySound(AudioManager.SoundType.OnButtonClick);
        if (vibrationButtonClickCount % 2 == 1)
        {
            VibrationButton.sprite = VibrationOffImage;
            GameController.gv.DisableVibrations();
        }
        else
        {
            VibrationButton.sprite = VibrationOnImage;
            GameController.gv.EnableVibrations();
        }
        vibrationButtonClickCount++;
    }

    public int GetMusicStatus() { return musicButtonClickCount % 2; }
    public int GetVibrationStatus() { return vibrationButtonClickCount % 2;  }

    public void SetVibrationStatus(int _status) { vibrationButtonClickCount = _status + 1; OnVibrationButtonClick(); }
    public void SetVMusicStatus(int _status) { musicButtonClickCount = _status + 1; OnMusicButtonClick(); }
}
