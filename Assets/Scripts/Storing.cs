using UnityEngine;
using System.Collections.Generic;

public class Storing : MonoBehaviour
{
    private void OnApplicationQuit()
    {
        SaveAllData();
        //PlayerPrefs.DeleteAll();
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause) { SaveAllData(); }
        //PlayerPrefs.DeleteAll();
    }

    private void SaveAllData()
    {
        PlayerPrefs.SetInt("CurrentLevel", GameController.GetCurrentLevel());
        PlayerPrefs.SetInt("GemCount", GameController.gemc.GetGemCount());
        PlayerPrefs.SetInt("VibrationStatus", GameController.oc.GetVibrationStatus());
        PlayerPrefs.SetInt("MusicStatus", GameController.oc.GetMusicStatus());
        string bricksString = "";
        List<int> boughtBricks = GameController.bm.GetBoughtBricks();
        for (int i=0; i<boughtBricks.Count; i++)
        {
            if(i != boughtBricks.Count - 1) { bricksString += boughtBricks[i] + "*"; }
            else { bricksString += boughtBricks[i]; }
        }
        PlayerPrefs.SetString("BoughtBricks", bricksString);
        PlayerPrefs.SetInt("CurrentBrick", GameController.bm.GetSelectedBrickId());
        PlayerPrefs.SetInt("BestScore", GameController.scorec.GetBestScore());
    }

    public int GetCurrentLevel() { return PlayerPrefs.GetInt("CurrentLevel", 1); }
    public int GetBestScore() { return PlayerPrefs.GetInt("BestScore", 0); }
    public int GetGemCount() { return PlayerPrefs.GetInt("GemCount", 0); }
    public int GetVibrationStatus() { return PlayerPrefs.GetInt("VibrationStatus", 1); }
    public int GetMusicStatus() { return PlayerPrefs.GetInt("MusicStatus", 1); }
    public List<int> GetBoughtBricks()
    {
        List<int> bricks = new List<int>();
        string[] tempArray = PlayerPrefs.GetString("BoughtBricks", "0").Split("*".ToCharArray());
        foreach(string s in tempArray) { bricks.Add(int.Parse(s)); }

        return bricks;
    }
    public int GetCurrentBrick() { return PlayerPrefs.GetInt("CurrentBrick", 0); }
}
