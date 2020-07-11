using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class AdSimulation : MonoBehaviour
{
    [SerializeField] GameObject AdUI;
    [SerializeField] Image RingImage;
    [SerializeField] TextMeshProUGUI SecondText;

    public void SimulateAd(Action CallWhenAdOver)
    {
        StartCoroutine(AdSimulate(CallWhenAdOver));
    }

    private IEnumerator AdSimulate(Action CallWhenAdOver)
    {
        GameController.gc.OnAdPlay();
        RingImage.fillAmount = 1f;
        AdUI.SetActive(true);
        for(float i=3f; i >= -0.3f; i -= Time.deltaTime)
        {
            SecondText.text = Mathf.CeilToInt(i).ToString();
            RingImage.fillAmount = i/3f;
            yield return null;
        }
        GameController.gc.OnAdOver();
        CallWhenAdOver();
        AdUI.SetActive(false);
    }
}
