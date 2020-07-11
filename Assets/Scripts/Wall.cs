using UnityEngine;

public class Wall : MonoBehaviour
{
    private bool canMove = true;
    [SerializeField] Vector3 MovementAmplitude;
    [SerializeField] Vector3 MovementSpeed;
    private Vector3 startPos;
    [SerializeField] Rigidbody rb;
    private float time = 0f;

    private void Start()
    {
        startPos = transform.position;
    }

    private void FixedUpdate()
    {
        if (canMove)
        {
            Vector3 movement = new Vector3(
            Mathf.Sin(time * MovementSpeed.x) * MovementAmplitude.x,
            Mathf.Sin(time * MovementSpeed.y) * MovementAmplitude.y,
            Mathf.Sin(time * MovementSpeed.z) * MovementAmplitude.z
            ) + startPos;
            time += Time.fixedDeltaTime;

            rb.MovePosition(movement);
        }
    }

    public void StartWallMovement()
    {
        canMove = true;
    }

}
