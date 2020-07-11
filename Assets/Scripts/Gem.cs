using UnityEngine;

public class Gem : MonoBehaviour
{
    [SerializeField] GameObject OnCollectParticles;

    private void OnCollection()
    {
        GameObject collectPar = Instantiate(OnCollectParticles, transform.position, Quaternion.identity);
        Destroy(collectPar, 2f);
        GameController.gv.Vibrate(30);
        Destroy(this.gameObject);
        GameController.gemc.OnGemAdd(GameController.cc.transform.GetChild(0).GetComponent<Camera>().WorldToScreenPoint(transform.position));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Brick"))
        {
            OnCollection();
        }
    }
}
