using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class UIController : MonoBehaviour {

    [SerializeField] GameObject StartUI;
    [SerializeField] GameObject LevelCompleteUI;
    [SerializeField] TextMeshProUGUI LevelCompleteText;
    [SerializeField] TextMeshProUGUI LevelCompleteTextScore;
    [SerializeField] TextMeshProUGUI LevelCompleteTextBestScore;
    [SerializeField] BrickMap BrickMapUI;
    [SerializeField] GameObject ScoreText;
    [SerializeField] GameObject ScoreAddText;
    [SerializeField] GameObject PerfectHitText;
    [SerializeField] GameObject GameOverUI;
    [SerializeField] TextMeshProUGUI PercentageOfLevelLeftText;
    [SerializeField] TextMeshProUGUI GameOverTextScore;
    [SerializeField] TextMeshProUGUI GameOverTextBestScore;
    [SerializeField] GameObject OptionsButton;
    [SerializeField] GameObject GemCountImage;
    [SerializeField] GameObject BrickChooseUI;
    [SerializeField] TextMeshProUGUI BrickChooseMainText;
    [SerializeField] TextMeshProUGUI BrickChoosePriceText;

    [SerializeField] GameObject LevelCompleteParticles;


    public void OnStartButtonClick()
    {
        GameController.gc.StartGame();
        GameController.am.PlaySound(AudioManager.SoundType.OnButtonClick);
    }
    public void OnLevelCompleteButtonClick()
    {
        GameController.am.PlaySound(AudioManager.SoundType.OnButtonClick);
        if((GameController.GetCurrentLevel()-1) % 8 != 0)
        {
            GameController.LoadLevel();
        }
        else
        {
            OnMapButtonClick();
        }
    }
    public void OnRetryButtonClick()
    {
        GameController.am.PlaySound(AudioManager.SoundType.OnButtonClick);
        GameController.LoadLevel();
    }
    public void OnMapButtonClick()
    {
        GameController.am.PlaySound(AudioManager.SoundType.OnButtonClick);
        GameController.sc.LoadScene("LevelScene");
        DisableGameOverUI();
        DisableLevelCompleteUI();
        EnableStartUI();
    }
    public void OnBrickButtonClick()
    {
        GameController.am.PlaySound(AudioManager.SoundType.OnButtonClick);
        GameController.sc.LoadScene("BrickScene");
        DisableStartUI();
        DisableLevelCompleteUI();
        DisableGameOverUI();
        EnableBrickChooseUI();
    }
    public void OnBrickChooseMainClick()
    {
        GameController.am.PlaySound(AudioManager.SoundType.OnButtonClick);
        BrickChooseController bc = GameObject.Find("BrickChooseController").GetComponent<BrickChooseController>();
        BrickChooseController.MainButtonState _state = bc.GetCurrentState();
        int _activeBrick = bc.GetCurrentActiveBrick();
        int _price = bc.GetActiveBrickPrice();


        if (_state == BrickChooseController.MainButtonState.Use)
        {
            GameController.bm.SetSelectedBrick(_activeBrick);
            GameController.pc.SelectedBrick = GameController.bm.GetCurrentBrick();
            GameController.sc.LoadScene("LevelScene");
            DisableBrickChooseUI();
            EnableStartUI();
        }
        else if(_state == BrickChooseController.MainButtonState.Buy)
        {
            GameController.bm.BuyBrick(_activeBrick, _price);
        }
        else if(_state == BrickChooseController.MainButtonState.Locked)
        {
            GameController.gv.Vibrate(120);
        }
        bc.UpdateMainButton();
    }

    public void CreateLevelCompleteParticles()
    {
        Transform par = Instantiate(LevelCompleteParticles, transform).transform;
        Destroy(par.gameObject, 3f);
    }

    public void DisableStartUI() { StartUI.SetActive(false); }
    public void EnableStartUI() { StartUI.SetActive(true); }
    public void DisableLevelCompleteUI() { LevelCompleteUI.SetActive(false); }
    public void EnableLevelCompleteUI() { LevelCompleteUI.SetActive(true); LevelCompleteTextScore.text = "Score: " + GameController.scorec.GetScore().ToString(); LevelCompleteTextBestScore.text = "Best Score: " + GameController.scorec.GetBestScore().ToString(); LevelCompleteText.text = "Level " + (GameController.GetCurrentLevel()-1) + " complete!"; }
    public void DisableBrickMapUI() { BrickMapUI.gameObject.SetActive(false); }
    public void EnableBrickMapUI() { BrickMapUI.gameObject.SetActive(true); BrickMapUI.OnBrickMapEnable(); }
    public void UpdateBrickMap() { BrickMapUI.UpdateBrickMap(); }
    public void EnableScoreText() { ScoreText.SetActive(true); }
    public void DisableScoreText() { ScoreText.SetActive(false); }
    public void EnableGameOverUI() { GameOverUI.SetActive(true); PercentageOfLevelLeftText.text = BrickMapUI.GetPercentageLeft() + "% left."; GameOverTextScore.text = "Score: " + GameController.scorec.GetScore().ToString(); GameOverTextBestScore.text = "Best Score: " + GameController.scorec.GetBestScore().ToString(); }
    public void DisableGameOverUI() { GameOverUI.SetActive(false); }
    public void DisableBrickChooseUI() { BrickChooseUI.SetActive(false); }
    public void EnableBrickChooseUI() { BrickChooseUI.SetActive(true); }
    public void SetBrickChooseButtonText(string _text) { BrickChooseMainText.text = _text; }
    public void SetBrickChoosePriceText(string _text, Vector3 _pos) { BrickChoosePriceText.text = _text; BrickChoosePriceText.gameObject.GetComponent<RectTransform>().localPosition = _pos; }
    public void EnableBrickChoosePriceText() { BrickChoosePriceText.gameObject.SetActive(true); }
    public void DisableBrickChoosePriceText() { BrickChoosePriceText.gameObject.SetActive(false); }
    public void SetBrickChoosePriceTextFontSize(int _size) { BrickChoosePriceText.fontSize = _size; }
    public bool isMouseOverUIElements() { if (Input.touches.Length == 0) { return EventSystem.current.IsPointerOverGameObject(); } else { return EventSystem.current.IsPointerOverGameObject(Input.touches[0].fingerId); } }
    public void DisableScoreTextChildren() { PerfectHitText.SetActive(false); ScoreAddText.SetActive(false); }
}
