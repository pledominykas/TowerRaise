using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FeverController : MonoBehaviour
{
    [SerializeField] GameObject FeverButton;
    [SerializeField] GameObject AdFeverButton;
    [SerializeField] GameObject FeverGlow;
    [SerializeField] GameObject AdFeverGlow;
    private bool isFeverActive = false;
    private bool isAdFeverActive = false;
    private bool isFeverAnimationActive = false;

    public void OnFeverButtonClick()
    {
        GameController.am.PlaySound(AudioManager.SoundType.OnButtonClick);
        if (GameController.gc.GetCurrentState() == GameController.GameState.Playing)
        {
            GameController.cc.ResizeCamera(CameraController.DEFAULT_CAM_FOV);
            GameController.cc.MoveCamera(GameController.tc.GetCurrentTowerPos(), 0.2f);
            DisableFeverButton();
            GameController.pc.OnFeverButtonClick();
            isFeverActive = false;
            isFeverAnimationActive = true;
        }
    }

    public void EnableFeverButton()
    {
        if (isFeverActive && !isFeverAnimationActive) { FeverButton.SetActive(true); FeverGlow.SetActive(true); }
    }
    public void DisableFeverButton()
    {
        isFeverAnimationActive = false;
        FeverButton.SetActive(false);
        FeverGlow.SetActive(false);
    }
    public void OnPerfectHit()
    {
        isFeverActive = true;
        EnableFeverButton();
        DisableAdFever();
    }
    public void OnDeath()
    {
        isFeverActive = false;
        DisableFeverButton();
        DisableAdFever();
    }

    public void EnableAdFever()
    {
        if (!isFeverActive)
        {
            isAdFeverActive = true;
            AdFeverButton.SetActive(true);
            AdFeverGlow.SetActive(true);
        }
    }
    public void DisableAdFever()
    {
        if (isAdFeverActive)
        {
            AdFeverButton.SetActive(false);
            AdFeverGlow.SetActive(false);
        }
    }

    public void OnAdFeverClick()
    {
        DisableAdFever();
        GameController.ads.SimulateAd(OnFeverButtonClick);
    }

    public void OnFeverAnimationOver() { DisableFeverButton(); }
    public bool GetIsFeverActive() { return isFeverActive; }
    public void OnBoltPickup() { isFeverActive = true; EnableFeverButton(); DisableAdFever(); }
}
