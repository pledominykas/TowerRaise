using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tower : MonoBehaviour {

    private const float ON_TOWER_BUILT_PARTICLE_WAIT_TIME = 0.12f;

    [SerializeField] int TowerHeight = 1;
    [SerializeField] int StartBrickCount = 0;
    [SerializeField] bool isStartTower = false;
    public Transform platfrom;
    [SerializeField] bool isAdFeverTower = false;
    [SerializeField] bool isDoubleTower = false;
    protected int CurrentHeight = 0;
    protected Vector3 Position;
    private Vector3 PlatformStartPosition;
    protected List<Transform> Bricks = new List<Transform>();
    protected float time = 0f;
    protected bool canMove = true;
    private bool towerIsBuilt = false;

    protected bool isRisen = false;
    public bool isCurrentTower = false;
    private Transform BrickSpawnPos;
    private Renderer platformRenderer;


    public virtual void Start()
    {
        platformRenderer = platfrom.GetComponent<Renderer>();
        Position = transform.position;
        PlatformStartPosition = platfrom.position;
        transform.position = transform.position - Vector3.up * GameController.TOWER_SPAWN_Y_OFFSET;
        CurrentHeight = StartBrickCount;
        BrickSpawnPos = transform.GetChild(0).transform.GetChild(0);
        CreateTower();
    }

    private void CreateTower()
    {
        for (int i = 0; i < StartBrickCount; i++)
        {
            Brick brick;
            brick = Instantiate(GameController.pc.SelectedBrick, transform).GetComponent<Brick>();
            brick.transform.position = transform.position + Vector3.up * ((GameController.pc.SelectedBrick.transform.localScale.y * (i+1)) - GameController.pc.SelectedBrick.transform.localScale.y/2f);
            Bricks.Add(brick.transform);
            if(CurrentHeight != TowerHeight)
            {
                if (isStartTower == false)
                {
                    brick.OnTowerAdd();
                    brick.SetBrickNumber(TowerHeight - CurrentHeight);
                }
            }
        }
    }

    public void AddHeight(Transform _brick)
    {
        CurrentHeight++;
        BrickSpawnPos.position = _brick.position;
        Bricks.Add(_brick);
        Brick br = Bricks[Bricks.Count - 1].GetComponent<Brick>();
        br.OnAddHeight();
        if (CurrentHeight != TowerHeight)
        {
            Bricks[Bricks.Count - 1].SetParent(transform);
            br.OnTowerAdd();
        }
        if (CurrentHeight >= TowerHeight)
        {
            towerIsBuilt = true;
            GameController.tc.OnTowerBuilt();
            if(TowerHeight > 1) { StartCoroutine(TowerBuiltParticles()); }
        }
        else
        {
            GameController.pc.CreateNewBrick();
        }
        GameController.uc.UpdateBrickMap();
    }

    private IEnumerator TowerBuiltParticles()
    {
        yield return new WaitForSeconds(0.3f);
        for (int i = 0; i < Bricks.Count; i++)
        {
            yield return new WaitForSeconds(ON_TOWER_BUILT_PARTICLE_WAIT_TIME);
            Bricks[i].GetComponent<Brick>().OnTowerBuilt(Bricks.Count-i-1);
            GameController.gv.TowerBuiltVibration();
        }
    }

    private IEnumerator RaiseTowerAnimation()
    {
        Vector3 vel = Vector3.zero;
        for(float i = 0f; i < Mathf.Infinity; i += Time.deltaTime)
        {
            transform.position = Vector3.SmoothDamp(transform.position, Position, ref vel, GameController.PLATFORM_ANIMATION_SPEED);
            if(Vector3.SqrMagnitude(transform.position - Position) <= 0.01f)
            {
                OnTowerRisen();
                break;
            }
            yield return null;
        }
    }

    private IEnumerator TowerDescendAnimation()
    {
        Vector3 vel = Vector3.zero;
        Vector3 targetPos = new Vector3(Position.x, -GameController.TOWER_SPAWN_Y_OFFSET, Position.z);
        for (float i = 0f; i < Mathf.Infinity; i += Time.deltaTime)
        {
            transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref vel, 1f);
            if (Vector3.SqrMagnitude(transform.position - targetPos) <= 0.01f)
            {
                break;
            }
            yield return null;
        }
    }

    private void SetUpStartTower()
    {
        transform.position = Position;
        Bricks[0].SetParent(transform.parent);
        GameController.pc.SetUpPlayer(Bricks[0]);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Brick") && isCurrentTower)
        {
            GameController.pc.OnBrickEnterTower();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(TowerHeight != CurrentHeight && other.CompareTag("Brick"))
        {
            Debug.Log("Brick fall");
        }
    }

    public virtual void OnTowerRisen() { isRisen = true; }
    public int GetTowerUnbuiltBrickCount() { return TowerHeight - CurrentHeight; }
    public void RaiseTower() { if (!isStartTower) { StartCoroutine(RaiseTowerAnimation()); } else { SetUpStartTower(); } }
    public void SetStartTower() { isStartTower = true; }
    public Vector3 GetTowerPosition() { return Position; }
    public Vector3 GetLastBrickPos() { if (Bricks.Count > 0) { return Bricks[Bricks.Count - 1].position; } else { return Vector3.zero; } }
    public int GetBrickNumber() { if (CurrentHeight < TowerHeight-1) { return TowerHeight - CurrentHeight - 1; } else { return GameController.tc.GetNextTowerUnbuiltBrickCount() - 1; } }
    public virtual float GetLandingPoint() { float brHeight = 0; foreach (Transform br in Bricks) { brHeight += br.lossyScale.y; } return transform.position.y + brHeight; }
    public Vector3 GetBrickSpawnPoint() { return BrickSpawnPos.position; }
    public void SetTowerColor(Color _color) { platfrom.GetComponent<MeshRenderer>().material.color = _color; }
    public void SetCurrentTower(bool _isCurrent) { isCurrentTower = _isCurrent; }
    public bool isLastBrick() { return (CurrentHeight == TowerHeight-1); }
    public int GetHeight() { return TowerHeight; }
    public int GetUnbuiltHeight() { return TowerHeight - StartBrickCount; }
    public int GetCurrentHeight() { return CurrentHeight; }
    public virtual Vector3 GetFeverSpawnPos() { if(CurrentHeight == 0) { return Position; } else { return GetLastBrickPos(); } }
    public virtual void OnFever() { return; }
    public virtual void OnFeverAnimationComplete() { return; }
    public void SetLastBrickLandPosition(Vector3 _pos) { BrickSpawnPos.position = _pos; }
    public bool GetIsAdFeverTower() { return isAdFeverTower; }
    public virtual void ResetTower()
    {
        SetLastBrickLandPosition(GetSecondChancePostion());
        Brick[] towerBricks = transform.GetComponentsInChildren<Brick>();

        for (int i = 0; i < towerBricks.Length; i++)
        {
            towerBricks[i].transform.rotation = Quaternion.identity;
            towerBricks[i].transform.position = platfrom.position + Vector3.up * (platfrom.lossyScale.y + (towerBricks[i].transform.lossyScale.y * 0.5f + i * towerBricks[i].transform.lossyScale.y));
            towerBricks[i].OnSecondChance();
        }
    }
    public void AddBrick(Transform _brick) { Bricks.Add(_brick); _brick.SetParent(transform); }
    public void DescendIfDouble() { if (isDoubleTower) { canMove = false; StartCoroutine(TowerDescendAnimation()); } }
    public void AddHeightManual() { CurrentHeight++; }
    public Bounds GetTowerBounds() { return platformRenderer.bounds; }
    public Vector3 GetSecondChancePostion() { if (Bricks.Count != 0) { return platfrom.position + Vector3.up * (platfrom.lossyScale.y + Bricks[Bricks.Count - 1].lossyScale.y * 0.5f + Bricks[Bricks.Count - 1].lossyScale.y * (CurrentHeight - 1)); } else { return platfrom.position + Vector3.up * (platfrom.lossyScale.y + 0.25f); } }
    public bool isBuilt() { return towerIsBuilt; }
}
