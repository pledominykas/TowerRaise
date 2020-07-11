using UnityEngine;

public class Bolt : MonoBehaviour
{
    private const float ROTATION_SPEED = 40f;
    private const float Y_BOB_FREQ = 3f;
    private const float Y_BOB_MAGNITUDE = 0.25f;

    private float time = 0f;
    private Vector3 startPos;

    [SerializeField] GameObject OnCollectParticles;

    private void Start()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        time += Time.deltaTime;
        transform.Rotate(Vector3.up * ROTATION_SPEED * Time.deltaTime);
        transform.position = startPos + Vector3.up * Mathf.Sin(time * Y_BOB_FREQ) * Y_BOB_MAGNITUDE;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Brick"))
        {
            GameObject collectPar = Instantiate(OnCollectParticles, transform.position, Quaternion.identity);
            Destroy(collectPar, 2f);
            GameController.fc.OnBoltPickup();
            GameController.gv.Vibrate(30);
            Destroy(this.gameObject);
        }
    }

}
