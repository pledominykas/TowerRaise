using System.Collections.Generic;
using UnityEngine;

public class BrickManager : MonoBehaviour
{
    [SerializeField] List<BrickBuy> Bricks;
    private List<int> BoughtBricks = new List<int>() { 0 };
    private int CurrentBrick = 0;

    public void SetBoughtBricks(List<int> _bricks)
    {
        BoughtBricks = _bricks;
    }

    public void BuyBrick(int _id, int _price)
    {
        if(GameController.gemc.GetGemCount() >= _price)
        {
            GameController.gemc.RemoveGems(_price);
            BoughtBricks.Add(_id);
            GameController.uc.DisableBrickChoosePriceText();
            GameController.uc.CreateLevelCompleteParticles();
            GameController.am.PlaySound(AudioManager.SoundType.OnBrickBuy);
        }
        else
        {
            Debug.Log("Not enough gems.");
            GameController.gv.Vibrate(120);
        }
    }

    public void CheckForUnlockedBricks()
    {
        //foreach(int i in BoughtBricks) { Debug.Log(i); }
        for (int i = 0; i < Bricks.Count; i++)
        {
            //Debug.Log(BoughtBricks.Contains(i));
            //Debug.Log(i + "  " + BoughtBricks.Contains(i));
            if (Bricks[i].isBuyable == false && Bricks[i].UnlockLevel < GameController.GetCurrentLevel() && BoughtBricks.Contains(i) == false)
            {
                BoughtBricks.Add(i);
                GameController.am.PlaySound(AudioManager.SoundType.OnBrickBuy);
                CurrentBrick = i;
                GameController.sc.LoadScene("BrickScene");
                GameController.uc.DisableLevelCompleteUI();
                GameController.uc.EnableBrickChooseUI();
                GameController.uc.SetBrickChoosePriceText("New brick!", new Vector3(0f, 175f));
                GameController.uc.EnableBrickChoosePriceText();
                GameController.uc.SetBrickChoosePriceTextFontSize(50);
            }
        }
    }

    public List<int> GetBoughtBricks() { return BoughtBricks; }
    public List<BrickBuy> GetBricks() { return Bricks; }
    public GameObject GetCurrentBrick() { return Bricks[CurrentBrick].BrickPrefab; }
    public void SetSelectedBrick(int _id) { CurrentBrick = _id; }
    public int GetSelectedBrickId() { return CurrentBrick; }
    public Material GetBackgroundMaterial(int _id) { return Bricks[_id].BackgroundMaterial; }
}

[System.Serializable]
public class BrickBuy
{
    public GameObject BrickPrefab;
    public bool isBuyable;
    public int Price;
    public int UnlockLevel;
    public Material BackgroundMaterial;
}
