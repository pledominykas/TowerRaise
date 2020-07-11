using UnityEngine;

public class RotatingTower : SinMoveTower
{
    [SerializeField] Vector3 RotationAmplitude;
    [SerializeField] Vector3 RotationSpeed;
    [SerializeField] Vector3 RotationOffset;

    new private void FixedUpdate()
    {
        base.FixedUpdate();
        if (isRisen && canMove)
        {
            Vector3 rotation = new Vector3(
                Mathf.Sin(time * RotationSpeed.x) * RotationAmplitude.x,
                Mathf.Sin(time * RotationSpeed.y) * RotationAmplitude.y,
                Mathf.Sin(time * RotationSpeed.z) * RotationAmplitude.z
                ) + RotationOffset;
            rb.MoveRotation(Quaternion.Euler(rotation));
        }
    }
}
