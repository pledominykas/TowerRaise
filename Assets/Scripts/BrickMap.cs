using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BrickMap : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI CurrentLevelText;
    [SerializeField] TextMeshProUGUI NextLevelText;
    [SerializeField] Image ProgressBar;
    private float ProgressTargetFill = 0f;
    private float ProgressFillSpeed = 8f;

    public void OnBrickMapEnable()
    {
        CurrentLevelText.text = GameController.GetCurrentLevel().ToString();
        NextLevelText.text = (GameController.GetCurrentLevel() + 1).ToString();
        UpdateBrickMap();
        ProgressBar.fillAmount = ProgressTargetFill;
    }

    public void UpdateBrickMap()
    {
        ProgressTargetFill = (float)GameController.pc.GetShotBrickCount() / GameController.tc.GetAllBrickCount();
    }

    private void Update()
    {
        ProgressBar.fillAmount = Mathf.Lerp(ProgressBar.fillAmount, ProgressTargetFill, Time.deltaTime * ProgressFillSpeed);
    }

    public int GetPercentageLeft() { return (int)((1f - ProgressTargetFill) * 100); }
}
