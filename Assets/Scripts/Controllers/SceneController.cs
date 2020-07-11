using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SceneController : MonoBehaviour
{
    private const float FADE_SPEED = 3f;
    [SerializeField] Image FadeTexture;
    [SerializeField] CanvasGroup FadeTextureGroup;
    [SerializeField] List<LevelThemeSettings> FadeTextureColors = new List<LevelThemeSettings>();

    [SerializeField] GameObject BonusLevel;
    [SerializeField] GameObject BoltLevel;

    public void LoadScene(string _scene)
    {
        SceneManager.LoadScene(_scene);
        GameController.scorec.ResetScore();
        GameController.cc.ResetCameraSize();
    }
    public void LoadLevel(string _scene)
    {
        StartCoroutine(LoadLevelScene(_scene));
        GameController.scorec.ResetScore();
    }

    private IEnumerator LoadLevelScene(string _scene)
    {
        float readTime = 0f;
        if (GameController.GetCurrentLevel() % 8 == 0) { BonusLevel.SetActive(true); readTime = 0.3f; }
        if (GameController.GetCurrentLevel() % 8 - 4 == 0) { BoltLevel.SetActive(true); readTime = 0.3f; }
        LevelThemeSettings lvlTheme = FadeTextureColors[GameController.GetCurrentEight()];
        FadeTexture.gameObject.SetActive(true);
        FadeTexture.color = new Color(lvlTheme.FadeTextureColor.r, lvlTheme.FadeTextureColor.g, lvlTheme.FadeTextureColor.b, 1f);
        for(float i = 0f; i <= 1.05f; i += Time.deltaTime * FADE_SPEED)
        {
            FadeTextureGroup.alpha = i;
            yield return null;
        }
        GameController.uc.DisableStartUI();
        GameController.uc.DisableLevelCompleteUI();
        GameController.uc.DisableGameOverUI();
        GameController.scc.ResetSecondChance();
        GameController.cc.ResetCameraSize();
        LoadScene(_scene);
        yield return null;
        RenderSettings.fog = lvlTheme.Fog;
        RenderSettings.fogMode = FogMode.Linear;
        RenderSettings.fogColor = lvlTheme.Fogcolor;
        RenderSettings.fogStartDistance = lvlTheme.FogStartDistance;
        RenderSettings.fogEndDistance = lvlTheme.FogEndDistance;
        GameController.gc.OnLevelLoad();
        yield return new WaitForSeconds(readTime);
        yield return null;
        for (float i = 1f; i >= 0f; i -= Time.deltaTime * FADE_SPEED)
        {
            FadeTextureGroup.alpha = i;
            yield return null;
        }
        FadeTexture.gameObject.SetActive(false);
        BonusLevel.SetActive(false);
        BoltLevel.SetActive(false);
    }
}

[System.Serializable]
public class LevelThemeSettings
{
    public Color FadeTextureColor;
    public bool Fog = false;
    public Color Fogcolor;
    public float FogStartDistance;
    public float FogEndDistance;
}

