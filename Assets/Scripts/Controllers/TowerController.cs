using UnityEngine;
using System.Collections.Generic;

public class TowerController : MonoBehaviour {

    [SerializeField] List<Tower> Towers = new List<Tower>();
    [SerializeField] Transform CameraTargetTransform;
    private int CurrentTower = 1;


    public void OnTowerBuilt()
    {
        if(CurrentTower >= Towers.Count-1)
        {
            GameController.gc.OnLevelComplete();
        }
        else
        {
            //Debug.Log(Towers[CurrentTower].transform.position + "  " + Towers[CurrentTower + 1].transform.position);
            GameController.cc.MoveCamera(Towers[CurrentTower], Towers[CurrentTower + 1]);
            RaiseNextTower();
            CurrentTower++;
            GameController.pc.OnTowerBuilt();
            Towers[CurrentTower-1].SetCurrentTower(false);
            Towers[CurrentTower].SetCurrentTower(true);
            if (Towers[CurrentTower-1].GetIsAdFeverTower()) { GameController.fc.EnableAdFever(); } else { GameController.fc.DisableAdFever(); }
            Towers[CurrentTower - 2].DescendIfDouble();
        }
    }

    private void RaiseNextTower() { Towers[CurrentTower+1].RaiseTower(); }
    public void RaiseFirstTower() { Towers[1].RaiseTower(); Towers[1].SetCurrentTower(true); }
    public void RaiseStartTower() { Towers[0].RaiseTower(); }
    public void AddTowerHeight(Transform _brick) { Towers[CurrentTower].AddHeight(_brick); }
    public void SetBrickParentTower(Transform _brick) { _brick.SetParent(Towers[CurrentTower - 1].transform); }
    public void PaintTowers(Color _color) { for (int i = 0; i < Towers.Count; i++) { Towers[i].SetTowerColor(_color); } }
    public Vector3 GetCurrentTowerPos() { return Towers[CurrentTower].GetTowerPosition(); }
    public Vector3 GetFeverSpawnPos() { return Towers[CurrentTower].GetFeverSpawnPos(); }
    public Tower GetStartTower() { return Towers[0]; }
    public Vector3 GetLastBrickPos() { return Towers[CurrentTower].GetLastBrickPos(); }
    public Vector3 GetBrickSpawnPoint() { if (CurrentTower == 1) { return Towers[CurrentTower - 1].transform.position; } else { return Towers[CurrentTower - 1].GetBrickSpawnPoint(); } }
    public List<Vector3> GetTowerPostions() { List<Vector3> pos = new List<Vector3>(); foreach (Tower t in Towers) { pos.Add(t.transform.position); } return pos; }
    public int GetNextTowerUnbuiltBrickCount() { if (CurrentTower + 1 >= Towers.Count) { return 1; } return Towers[CurrentTower+1].GetTowerUnbuiltBrickCount(); }
    public int GetCurrentTowerUnbuiltBrickCount() { return Towers[CurrentTower].GetTowerUnbuiltBrickCount(); }
    public int GetBrickNumber() { return Towers[CurrentTower].GetBrickNumber(); }
    public int GetTowerCount() { return Towers.Count; }
    public int GetTowerHeight(int _towerNum) { return Towers[_towerNum].GetHeight(); }
    public int GetCurrentTowerNum() { return CurrentTower; }
    public float GetLandingHeight() { return Towers[CurrentTower].GetLandingPoint(); }
    public bool isLastBrickOfLevel() { return (CurrentTower == Towers.Count - 1) ? Towers[CurrentTower].isLastBrick() : false; }
    public bool isLastTower() { return (CurrentTower == Towers.Count - 1); }
    public Transform GetCameraTargetTransform() { return CameraTargetTransform; }
    public int GetAllBrickCount() { int count = 0; foreach(Tower t in Towers) { count += t.GetUnbuiltHeight(); } return count; }
    public int GetCurrentTowerCurrentHeight() { return Towers[CurrentTower].GetCurrentHeight(); }
    public Rigidbody GetCurrentTowerRigidbody() { return Towers[CurrentTower].transform.GetChild(0).GetComponent<Rigidbody>(); }
    public void OnFeverAnimationComplete() { Towers[CurrentTower].OnFeverAnimationComplete(); OnTowerBuilt(); }
    public void OnFever() { Towers[CurrentTower].OnFever(); }
    public void SetLastBrickLandPosition(Vector3 _pos) { Towers[CurrentTower].SetLastBrickLandPosition(_pos); }
    public void OnSecondChanceClick() { for (int i = CurrentTower; i > 0; i--) { Towers[i].ResetTower(); } }
    public Vector3 GetSecondChancePostion() { return Towers[CurrentTower - 1].GetSecondChancePostion(); }
    public void AddBrick(Transform _brick) { Towers[CurrentTower].AddBrick(_brick); }
    public bool isAdFeverTower() { return Towers[CurrentTower - 1].GetIsAdFeverTower(); }
    public bool isLastBrickOfTower() { return Towers[CurrentTower].isLastBrick(); }
    public void AddTowerHeightManual() { Towers[CurrentTower].AddHeightManual(); }
    public Tower GetCurrentTower() { return Towers[CurrentTower]; }
    public void PauseAllTowers() { foreach(Tower t in Towers) { t.enabled = false; } }
    public void UnPauseAllTowers() { foreach (Tower t in Towers) { t.enabled = true; } }
}
