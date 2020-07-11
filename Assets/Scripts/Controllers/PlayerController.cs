using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{

    private enum PlayerState { Waiting, Aiming, Shooting }
    private const float BRICK_ROTATION = Mathf.PI * 2f;
    private const float MAX_ROTATION_SPEED = 12f;
    private const float START_SHOOT_FORCE_MAGNITUDE = 0.75f;
    private float ForwardForce = 3.8f;
    private float UpForce = 15.5f;
    private Rigidbody CurrentBrick;
    public GameObject SelectedBrick;
    private PlayerState CurrentState = PlayerState.Waiting;

    private PlayerTrajectory pt;
    private Vector3 shootForce;
    private Vector3 forceMagnitude = Vector3.zero;
    private Vector3 currentTowerPos;
    private Vector3 rotationDir;
    private bool isCurrentBrickInTower = false;
    private bool isLastBrick = false;
    private int ShotCount = 0;
    private int PerfectHitsInARow = 0;

    private bool FeverAnimationActive = false;
    private Vector3 brickTransformVel;

    private void Start()
    {
        pt = GetComponent<PlayerTrajectory>();
    }

    private void Update()
    {
        if (!GameController.uc.isMouseOverUIElements() && !FeverAnimationActive)
        {
            if (Input.GetKey(KeyCode.Mouse0) && CurrentState == PlayerState.Waiting) { OnAimStart(); }
            if (Input.GetKey(KeyCode.Mouse0) && CurrentState == PlayerState.Aiming) { Aim(); }
            if (Input.GetKeyUp(KeyCode.Mouse0) && CurrentState == PlayerState.Aiming) { Shoot(); }
        }
    }

    private void OnAimStart()
    {

        GameController.gv.StartAimVibration();
        CurrentState = PlayerState.Aiming;
        pt.EnableTrajcetory();
    }

    private void Aim()
    {
        Vector3 towerDir = (currentTowerPos - CurrentBrick.transform.position).normalized;
        forceMagnitude += new Vector3(ForwardForce, UpForce, ForwardForce) * Time.deltaTime;
        shootForce = new Vector3(towerDir.x * forceMagnitude.x, forceMagnitude.y, towerDir.z * forceMagnitude.z);
        transform.position = CurrentBrick.position;
        pt.DrawTrajectory(shootForce);
    }

    private void Shoot()
    {
        GameController.gv.EndAimVibration();
        pt.DisableTrajcetory();
        GameController.gv.ShootVibration();
        CurrentState = PlayerState.Shooting;
        isLastBrick = GameController.tc.isLastBrickOfLevel();
        CurrentBrick.velocity = Vector3.zero;
        CurrentBrick.AddForce(shootForce, ForceMode.Impulse);
        if (Mathf.Abs(shootForce.x) > Mathf.Abs(shootForce.z)) { rotationDir = -Vector3.forward; }
        else { rotationDir = Vector3.left; }
        StartCoroutine(RotateBrick(GetTimeUntilTower()));
        StartCoroutine(AddTowerHeight());
        ShotCount++;
        GameController.am.PlaySound(AudioManager.SoundType.OnShoot);
    }

    private float GetTimeUntilTower()
    {
        Vector2 shootForce2D = new Vector2(shootForce.z, shootForce.y);
        float aimAngle = Vector2.Angle(Vector2.right, shootForce2D) * Mathf.Deg2Rad;
        float towerHeight = GameController.tc.GetLandingHeight() + CurrentBrick.transform.localScale.y * 2f - CurrentBrick.transform.position.y;
        float t1 = -(-shootForce2D.magnitude * Mathf.Sin(aimAngle) + Mathf.Sqrt(shootForce2D.magnitude * shootForce2D.magnitude * Mathf.Sin(aimAngle) * Mathf.Sin(aimAngle) - 2 * Physics.gravity.magnitude * towerHeight)) / Physics.gravity.magnitude;
        float t2 = -(-shootForce2D.magnitude * Mathf.Sin(aimAngle) - Mathf.Sqrt(shootForce2D.magnitude * shootForce2D.magnitude * Mathf.Sin(aimAngle) * Mathf.Sin(aimAngle) - 2 * Physics.gravity.magnitude * towerHeight)) / Physics.gravity.magnitude;
        float timeUntilTower = (t2 > t1) ? t2 : t1;
        timeUntilTower = (float.IsNaN(timeUntilTower)) ? 2 * (shootForce2D.magnitude * Mathf.Sin(aimAngle)) / Physics.gravity.magnitude : timeUntilTower;

        return timeUntilTower;
    }

    private IEnumerator RotateBrick(float _timeUntilTower)
    {
        float rotationSpeed = Mathf.Clamp(BRICK_ROTATION / _timeUntilTower, 0f, MAX_ROTATION_SPEED);
        bool brickNumChanged = false;
        for (float i = 0f; i < _timeUntilTower; i += Time.deltaTime)
        {
            if (i >= _timeUntilTower / 2f && brickNumChanged == false) { CurrentBrick.GetComponent<Brick>().SetBrickNumber(GameController.tc.GetBrickNumber()); brickNumChanged = true; }
            CurrentBrick.MoveRotation(CurrentBrick.rotation * Quaternion.Euler(rotationDir * Mathf.Rad2Deg * rotationSpeed * Time.deltaTime));
            yield return null;
        }
    }

    private IEnumerator AddTowerHeight()
    {
        yield return new WaitForSeconds(0.2f);
        Rigidbody currentTowerRb = GameController.tc.GetCurrentTowerRigidbody();
        float timeInTower = 0f;
        for (float i = 0f; i < Mathf.Infinity; i += Time.deltaTime)
        {
            if (FeverAnimationActive) { CurrentState = PlayerState.Waiting; yield break; }
            if (CurrentState != PlayerState.Shooting) { yield break; }
            if (i >= GameController.MAX_SHOOT_TIME) { GameController.OnGameOver(); yield break; }
            if (isCurrentBrickInTower)
            {
                timeInTower += Time.deltaTime;
                if (timeInTower >= GameController.MAX_SHOOT_TIME / 2f) { Vector3 lastBrickPos = GameController.tc.GetLastBrickPos(); break; }
            }

            if (isLastBrick && CurrentBrick != null && Vector3.SqrMagnitude(CurrentBrick.velocity - currentTowerRb.velocity) <= 0.005f && isCurrentBrickInTower)
            {
                Vector3 lastBrickPos = GameController.tc.GetLastBrickPos();
                if (Vector2.SqrMagnitude(new Vector2(lastBrickPos.x, lastBrickPos.z) - new Vector2(CurrentBrick.position.x, CurrentBrick.position.z)) <= GameController.PERFECT_HIT_SQR_MAGNITUDE)
                {
                    PerfectHitsInARow++;
                    CurrentBrick.GetComponent<Brick>().OnPerfectHit(GameController.tc.isLastBrickOfTower() && !GameController.tc.isLastBrickOfLevel());
                    GameController.fc.OnPerfectHit();
                }
                else { PerfectHitsInARow = 0; }
                break;
            }
            else if (!isLastBrick && CurrentBrick != null && Vector3.SqrMagnitude(CurrentBrick.velocity - currentTowerRb.velocity) <= 2f && isCurrentBrickInTower)
            {
                Vector3 lastBrickPos = GameController.tc.GetLastBrickPos();
                if (Vector2.SqrMagnitude(new Vector2(lastBrickPos.x, lastBrickPos.z) - new Vector2(CurrentBrick.position.x, CurrentBrick.position.z)) <= GameController.PERFECT_HIT_SQR_MAGNITUDE)
                {
                    PerfectHitsInARow++;
                    CurrentBrick.GetComponent<Brick>().OnPerfectHit(GameController.tc.isLastBrickOfTower() && !GameController.tc.isLastBrickOfLevel());
                    GameController.fc.OnPerfectHit();
                }
                else { PerfectHitsInARow = 0; }
                break;
            }
            yield return null;
        }
        GameController.gv.LandingVibration();
        isCurrentBrickInTower = false;
        GameController.scorec.AddScore(GameController.tc.GetCurrentTowerCurrentHeight(), PerfectHitsInARow);
        GameController.tc.AddTowerHeight(CurrentBrick.transform);
        CurrentState = PlayerState.Waiting;
    }

    public void CreateNewBrick()
    {
        CurrentBrick = Instantiate(SelectedBrick, GameController.tc.GetBrickSpawnPoint(), transform.rotation).GetComponent<Rigidbody>();
        Brick cb = CurrentBrick.GetComponent<Brick>();
        cb.SetBrickNumber(GameController.tc.GetCurrentTowerUnbuiltBrickCount() - 1);
        cb.OnCreate();
        ResetAim();
    }

    public void OnTowerBuilt()
    {
        transform.position = CurrentBrick.transform.position;
        ResetAim();
        PerfectHitsInARow = 0;
    }

    public void SetUpPlayer(Transform _brick)
    {
        transform.position = _brick.transform.position;
        CurrentBrick = _brick.GetComponent<Rigidbody>();
        ResetAim();
        CurrentBrick.GetComponent<Brick>().SetBrickNumber(GameController.tc.GetTowerHeight(1)-1);
    }

    private void ResetAim()
    {
        currentTowerPos = GameController.tc.GetCurrentTowerPos();
        forceMagnitude = new Vector3(ForwardForce, UpForce, ForwardForce) * START_SHOOT_FORCE_MAGNITUDE;
    }

    private IEnumerator FeverAnimation()
    {
        FeverAnimationActive = true;
        GameController.tc.OnFever();
        Vector3 targetPos = GameController.tc.GetFeverSpawnPos() + Vector3.up * 4f;
        CurrentBrick.isKinematic = true;
        GameController.gv.EndAimVibration();
        pt.DisableTrajcetory();
        for(float i = 0f; i < Mathf.Infinity; i += Time.deltaTime)
        {
            CurrentBrick.transform.position = Vector3.SmoothDamp(CurrentBrick.transform.position, targetPos, ref brickTransformVel, 0.1f);
            CurrentBrick.transform.rotation = Quaternion.Slerp(CurrentBrick.transform.rotation, Quaternion.identity, 0.1f);
            if (Vector3.SqrMagnitude(CurrentBrick.transform.position - targetPos) <= 0.001f && Quaternion.Angle(CurrentBrick.transform.rotation, Quaternion.identity) <= 1f) { break; }
            yield return null;
        }
        int brickLeft = GameController.tc.GetCurrentTowerUnbuiltBrickCount();
        List<Rigidbody> feverBricks = new List<Rigidbody>();
        feverBricks.Add(CurrentBrick);
        CurrentBrick.GetComponent<Brick>().OnCreate();
        for (int i = 1; i < brickLeft; i++)
        {
            yield return new WaitForSeconds(0.1f);
            Rigidbody brick = Instantiate(SelectedBrick, targetPos + Vector3.up * 0.6f * i, Quaternion.identity).GetComponent<Rigidbody>();
            brick.isKinematic = true;
            brick.GetComponent<Brick>().OnCreate();
            brick.GetComponent<Brick>().SetBrickNumber(brickLeft - i-1);
            feverBricks.Add(brick);
        }
        yield return new WaitForSeconds(0.5f);

        GameController.tc.AddBrick(CurrentBrick.transform);

        foreach (Rigidbody rb in feverBricks)
        {
            GameController.tc.AddBrick(rb.transform);
            rb.isKinematic = false;
            rb.gameObject.GetComponent<Brick>().ClampYVelocity(true);
            rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
        }
        yield return new WaitForSeconds(0.3f);
        for(int i = 0; i<feverBricks.Count; i++)
        {
            yield return new WaitForSeconds(0.1f);
            PerfectHitsInARow++;
            feverBricks[i].GetComponent<Brick>().OnPerfectHit(GameController.tc.isLastBrickOfTower() && !GameController.tc.isLastBrickOfLevel());
            GameController.tc.AddTowerHeightManual();
            GameController.gv.LandingVibration();
            GameController.scorec.AddScore(i, i);
        }
        yield return new WaitForSeconds(0.2f);
        foreach (Rigidbody rb in feverBricks)
        {
            rb.mass *= Brick.BRICK_MASS_MULTIPLIER;
            rb.constraints = RigidbodyConstraints.None;
        }
        ShotCount += brickLeft;
        CurrentBrick = feverBricks[feverBricks.Count - 1];
        CurrentBrick.mass = 1f;
        GameController.tc.SetLastBrickLandPosition(CurrentBrick.transform.position);
        GameController.tc.OnFeverAnimationComplete();
        GameController.uc.UpdateBrickMap();
        GameController.fc.OnFeverAnimationOver();
        GameController.fc.EnableFeverButton();
        Brick cb = CurrentBrick.GetComponent<Brick>();
        if (!GameController.tc.isLastTower())
        {
            cb.SetBrickNumber(GameController.tc.GetBrickNumber());
        }
        cb.ClampYVelocity(false);
        FeverAnimationActive = false;
        isCurrentBrickInTower = false;
        CurrentState = PlayerState.Waiting;
    }

    public void OnSecondChanceClick()
    {
        pt.DisableTrajcetory();
        ShotCount--;
        CurrentBrick.transform.rotation = Quaternion.identity;
        CurrentBrick.transform.position = GameController.tc.GetSecondChancePostion();
        GameController.uc.UpdateBrickMap();
        ResetAim();
    }

    public void SetSelectedBrick(GameObject _brick) { SelectedBrick = _brick; }
    public void OnBrickEnterTower() { isCurrentBrickInTower = true; }
    public void OnGameOver() { CurrentState = PlayerState.Waiting; pt.DisableTrajcetory(); FeverAnimationActive = false; }
    public int GetShotBrickCount() { return ShotCount; }
    public void ResetShotCount() { ShotCount = 0; }
    public void OnFeverButtonClick() { StartCoroutine(FeverAnimation()); }
    public void DestroyCurrentBrick() { Destroy(CurrentBrick.gameObject); }
    public int GetPerfectHitsInARow() { return PerfectHitsInARow; }
    public void ResetPerfectHits() { PerfectHitsInARow = 0; }
    public void OnAdPlay() { if (CurrentBrick != null) { CurrentBrick.gameObject.SetActive(false); } }
    public void OnAdOver() { if (CurrentBrick != null) { CurrentBrick.gameObject.SetActive(true); } }
}
