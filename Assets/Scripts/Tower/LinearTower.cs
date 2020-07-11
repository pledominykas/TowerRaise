using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearTower : Tower
{
    [SerializeField] Vector3 MovementDestination;
    [SerializeField] float MovementSpeed;
    private Vector3 startPos;
    protected Rigidbody rb;
    protected BoxCollider towerCollider;
    private Vector3 towerColliderOffset;

    public override void Start()
    {
        base.Start();
        rb = transform.Find("Platform").GetComponent<Rigidbody>();
    }

    public override void OnTowerRisen()
    {
        base.OnTowerRisen();
        towerCollider = GetComponent<BoxCollider>();
        startPos = transform.position - Vector3.up * rb.transform.lossyScale.y;
        towerColliderOffset = towerCollider.center - rb.transform.localPosition;
    }

    protected void FixedUpdate()
    {
        if (isRisen && canMove)
        {
            Vector3 movement = Vector3.Lerp(startPos, MovementDestination + startPos, time * MovementSpeed);
            time += Time.fixedDeltaTime;

            rb.MovePosition(movement);
            towerCollider.center = towerColliderOffset + rb.transform.localPosition;
        }
    }

    public override void ResetTower()
    {
        time = 0f;
        rb.transform.position = startPos;
        //SetLastBrickLandPosition(Position);
        SetLastBrickLandPosition(GetSecondChancePostion());
        Brick[] towerBricks = transform.GetComponentsInChildren<Brick>();

        for (int i = 0; i < towerBricks.Length; i++)
        {
            towerBricks[i].transform.rotation = Quaternion.identity;
            towerBricks[i].transform.position = platfrom.position + Vector3.up * (platfrom.lossyScale.y + (towerBricks[i].transform.lossyScale.y * 0.5f + i * towerBricks[i].transform.lossyScale.y));
            towerBricks[i].OnSecondChance();
        }
    }

    public override Vector3 GetFeverSpawnPos() { if (CurrentHeight == 0) { return Position; } else { return GetLastBrickPos(); } }
    public override void OnFever() { canMove = false; }
    public override void OnFeverAnimationComplete() { canMove = true; }
    public override float GetLandingPoint() { float brHeight = 0; foreach (Transform br in Bricks) { brHeight += br.lossyScale.y; } return transform.position.y + ((MovementDestination.y >= 0f) ? MovementDestination.y:0f) + brHeight; }
}
