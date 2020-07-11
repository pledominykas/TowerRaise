using System.Collections;
using UnityEngine;

public class GameController : MonoBehaviour {

    public enum GameState { Playing, GameOver, LevelCompleted, BrickChoose }
    public const float PLATFORM_ANIMATION_SPEED = 0.17f;
    public const float PERFECT_HIT_SQR_MAGNITUDE = 0.009f;
    public const float TOWER_SPAWN_Y_OFFSET = 30f;
    private const float GAME_OVER_WAIT_TIME = 0.9f;
    public const float MAX_SHOOT_TIME = 4f; // maximum amount of time brick can be shooting // to prevent game bugs if brick is stuck or smthg

    public static TowerController tc;
    public static CameraController cc;
    public static PlayerController pc;
    public static UIController uc;
    public static GameController gc;
    public static GameVibrator gv;
    public static SceneController sc;
    public static ScoreController scorec;
    public static GemController gemc;
    public static FeverController fc;
    public static BrickManager bm;
    public static SecondChanceController scc;
    public static ClaimGems cg;
    public static AdSimulation ads;
    public static OptionsController oc;
    public static AudioManager am;
    private Storing storing;

    private static int CurrentLevel = 1;

    private static GameState CurrentState = GameState.BrickChoose;

    private void Start()
    {
        gc = this;
        cc = GameObject.Find("CameraParentObject").GetComponent<CameraController>();
        pc = GameObject.Find("Player").GetComponent<PlayerController>();
        uc = GameObject.Find("Canvas").GetComponent<UIController>();
        gv = GetComponent<GameVibrator>();
        sc = GetComponent<SceneController>();
        cg = GameObject.Find("Canvas").GetComponent<ClaimGems>();
        scc = GameObject.Find("Canvas").GetComponent<SecondChanceController>();
        fc = GameObject.Find("Canvas").GetComponent<FeverController>();
        bm = GetComponent<BrickManager>();
        ads = GameObject.Find("Canvas").GetComponent<AdSimulation>();
        scorec = GameObject.Find("Canvas").GetComponent<ScoreController>();
        gemc = GameObject.Find("Canvas").GetComponent<GemController>();
        oc = GameObject.Find("OptionsButton").GetComponent<OptionsController>();
        am = GameObject.Find("CameraParentObject").GetComponent<AudioManager>();
        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(uc.gameObject);
        DontDestroyOnLoad(pc.gameObject);
        DontDestroyOnLoad(cc.gameObject);
        DontDestroyOnLoad(GameObject.Find("EventSystem"));
        DontDestroyOnLoad(GameObject.Find("Directional Light"));
        DontDestroyOnLoad(GameObject.Find("LiteFPSCounter"));
        pc.enabled = false;
        sc.LoadScene("LevelScene");
        storing = GetComponent<Storing>();
        LoadAllData();
        pc.SelectedBrick = bm.GetCurrentBrick();
    }

    private void LoadAllData()
    {
        CurrentLevel = storing.GetCurrentLevel();
        gemc.SetGemCount(storing.GetGemCount());
        scorec.SetBestScore(storing.GetBestScore());
        oc.SetVibrationStatus(storing.GetVibrationStatus());
        oc.SetVMusicStatus(storing.GetMusicStatus());
        bm.SetBoughtBricks(storing.GetBoughtBricks());
        bm.SetSelectedBrick(storing.GetCurrentBrick());
        //CurrentLevel = 8;
        //gemc.SetGemCount(999);
    }

    private IEnumerator SetUpLevel()
    {
        yield return null;
        tc = GameObject.Find("Map").GetComponent<TowerController>();
        tc.RaiseStartTower();
        cc.SetCameraPosition(tc.GetStartTower(), tc.GetCurrentTower());
        tc.RaiseFirstTower();
        pc.enabled = true;
        pc.ResetShotCount();
        uc.EnableBrickMapUI();
        uc.EnableScoreText();
        fc.EnableFeverButton();
    }

    public void StartGame()
    {
        CurrentState = GameState.Playing;
        sc.LoadLevel("Level" + CurrentLevel.ToString());
    }

    public void OnLevelLoad()
    {
        CurrentState = GameState.Playing;
        StartCoroutine(SetUpLevel());
    }

    public static void OnGameOver()
    {
        if(CurrentState == GameState.Playing)
        {
            CurrentState = GameState.GameOver;
            gc.StartCoroutine("GameOver");
        }
    }
    private IEnumerator GameOver()
    {
        am.PlaySound(AudioManager.SoundType.OnDeath);
        EZCameraShake.CameraShaker.Instance.Shake(EZCameraShake.CameraShakePresets.GameOver);
        pc.OnGameOver();
        pc.ResetPerfectHits();
        gv.GameOverVibrate();
        pc.enabled = false;
        yield return new WaitForSeconds(GAME_OVER_WAIT_TIME);
        uc.EnableGameOverUI();
        uc.DisableBrickMapUI();
        uc.DisableScoreText();
        scc.SecondChance();
        fc.OnDeath();
    }

    public void OnLevelComplete()
    {
        if (CurrentState == GameState.Playing)
        {
            CurrentLevel++;
            CurrentState = GameState.LevelCompleted;
            pc.enabled = false;
            cc.OnLevelCompleteAnimation();
            pc.ResetPerfectHits();
        }
    }
    public void OnLevelCompleteAnimationOver()
    {
        uc.DisableBrickMapUI();
        uc.EnableLevelCompleteUI();
        uc.DisableScoreText();
        bm.CheckForUnlockedBricks();
        fc.DisableFeverButton();
        fc.DisableAdFever();
        uc.CreateLevelCompleteParticles();
        gv.LevelCompleteVibration();
        cg.SetGemCountText();
        am.PlaySound(AudioManager.SoundType.OnLevelComplete);
    }

    public void OnBrickChooseStart()
    {
        CurrentState = GameState.BrickChoose;
    }

    public void OnAdPlay()
    {
        pc.OnAdPlay();
        pc.enabled = false;
        tc.PauseAllTowers();
        //Pause
    }
    public void OnAdOver()
    {
        tc.UnPauseAllTowers();
        if(CurrentState == GameState.Playing) { pc.enabled = true; }
        pc.OnAdOver();
        //continue
    }

    public static void LoadLevel() { sc.LoadLevel("Level" + CurrentLevel); }
    public static int GetCurrentLevel() { return CurrentLevel; }
    public static int GetCurrentEight() { return Mathf.FloorToInt((CurrentLevel-1) / 8); }
    public GameState GetCurrentState() { return CurrentState; }
    public void OnSecondChanceClick() { CurrentState = GameState.Playing; }
}