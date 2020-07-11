using UnityEngine;

public class SinMoveTower : Tower
{
    [SerializeField] Vector3 MovementAmplitude;
    [SerializeField] Vector3 MovementSpeed;
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
            Vector3 movement = new Vector3(
            Mathf.Sin(time * MovementSpeed.x) * MovementAmplitude.x,
            Mathf.Sin(time * MovementSpeed.y) * MovementAmplitude.y,
            Mathf.Sin(time * MovementSpeed.z) * MovementAmplitude.z
            ) + startPos;
            time += Time.fixedDeltaTime;

            rb.MovePosition(movement);
            towerCollider.center = towerColliderOffset + rb.transform.localPosition;
        }
    }

    public override Vector3 GetFeverSpawnPos() { if (isRisen) { if (CurrentHeight == 0) { return rb.transform.position + Vector3.up * rb.transform.lossyScale.y; } else { return GetLastBrickPos(); } } else { return Position; } }
    public override void OnFever() { canMove = false; }
    public override void OnFeverAnimationComplete() { canMove = true; }
    public override float GetLandingPoint() { float brHeight = 0; foreach (Transform br in Bricks) { brHeight += br.lossyScale.y; } return transform.position.y + MovementAmplitude.y + brHeight; }
}
