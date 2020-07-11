using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SecondChanceController : MonoBehaviour
{
    private float SecondChanceTime = 5f;
    [SerializeField] Image SecondChanceImage;
    private bool secondChanceUsed = false;

    public void SecondChance()
    {
        if (!secondChanceUsed)
        {
            StartCoroutine(SecondChanceAnimation());
        }
    }

    private IEnumerator SecondChanceAnimation()
    {
        SecondChanceImage.gameObject.SetActive(true);
        SecondChanceImage.fillAmount = 1f;
        for (float i = 1f; i > -0.05f; i -= Time.deltaTime * (1f/SecondChanceTime))
        {
            SecondChanceImage.fillAmount = i;
            yield return null;
        }
        SecondChanceImage.gameObject.SetActive(false);
    }

    public void OnSecondChanceClick()
    {
        GameController.ads.SimulateAd(UseSecondChance);
    }

    private void UseSecondChance()
    {
        StopAllCoroutines();
        GameController.uc.DisableGameOverUI();
        GameController.uc.EnableBrickMapUI();
        GameController.uc.EnableScoreText();
        GameController.uc.DisableScoreTextChildren();
        GameController.pc.enabled = true;
        GameController.gc.OnSecondChanceClick();
        GameController.tc.OnSecondChanceClick();
        GameController.pc.OnSecondChanceClick();
        if (GameController.tc.isAdFeverTower()) { GameController.fc.EnableAdFever(); }
        secondChanceUsed = true;
        SecondChanceImage.gameObject.SetActive(false);
    }

    public void ResetSecondChance() { secondChanceUsed = false; StopAllCoroutines(); }
}
