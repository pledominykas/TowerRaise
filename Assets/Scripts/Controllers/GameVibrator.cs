using UnityEngine;
using System.Collections;

public class GameVibrator : MonoBehaviour
{
    private bool vibrationsEnabled = true;

    private bool isAiming = false;

    public void StartAimVibration() { if (!vibrationsEnabled) { return; } isAiming = true; StartCoroutine("VibrateWhileAiming"); }
    public void EndAimVibration() { if (!vibrationsEnabled) { return; } isAiming = false; }
    public void ShootVibration() { if (!vibrationsEnabled) { return; } StartCoroutine("VibrateWhileShooting"); }
    public void LandingVibration() { if (!vibrationsEnabled) { return; } Vibration.Vibrate(50); }
    public void BrickCreateVibration() { if (!vibrationsEnabled) { return; } StartCoroutine("VibrateOnBrickCreate"); }
    public void LevelCompleteVibration() { if (!vibrationsEnabled) { return; } Vibration.Vibrate(100); }
    public void GameOverVibrate() { if (!vibrationsEnabled) { return; } StartCoroutine("VibrateOnGameOver"); }
    public void TowerBuiltVibration() { if (!vibrationsEnabled) { return; } Vibration.Vibrate(60); }
    public void Vibrate(long _time) { if (!vibrationsEnabled) { return; } Vibration.Vibrate(_time); }

    private IEnumerator VibrateWhileAiming()
    {
        for (float i = 0f; (i < Mathf.Infinity) && isAiming; i += Time.deltaTime)
        {
            Vibration.Vibrate(5);
            yield return new WaitForSecondsRealtime(0.06f);
        }
    }

    private IEnumerator VibrateWhileShooting()
    {
        yield return new WaitForSecondsRealtime(0.05f);
        Vibration.Vibrate(50);
    }

    private IEnumerator VibrateOnBrickCreate()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        Vibration.Vibrate(50);
    }

    private IEnumerator VibrateOnGameOver()
    {
        isAiming = false;
        yield return new WaitForSecondsRealtime(0.1f);
        Vibration.Vibrate(400);
    }

    public void DisableVibrations() { StopAllCoroutines(); vibrationsEnabled = false; }
    public void EnableVibrations() { vibrationsEnabled = true; }
}
