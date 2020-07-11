using UnityEngine;
using TMPro;

public class Brick : MonoBehaviour {

    public const float BRICK_MASS_MULTIPLIER = 4f;
    private Rigidbody rb;
    [SerializeField] GameObject CreateObjectParticles;
    [SerializeField] GameObject TowerBuiltParticles;
    [SerializeField] GameObject PerfectHitPartcicles;
    [SerializeField] Material PerfectHitMaterial;
    private BrickAudio brickAudio;

    private bool clampYVelocity = false;
    private void Start()
    {
        brickAudio = transform.Find("BrickAudio").GetComponent<BrickAudio>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if(clampYVelocity == true)
        {
            rb.velocity = new Vector3(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -1000f, 0f), rb.velocity.z);
        }
    }

    public void ClampYVelocity(bool _clamp)
    {
        clampYVelocity = _clamp;
    }

    public void OnTowerAdd()
    {
        rb.mass *= BRICK_MASS_MULTIPLIER;
        rb.centerOfMass = Vector3.down * 0.25f;
    }

    public void OnAddHeight()
    {
        brickAudio.PlayHeightAdd();
    }

    public void SetBrickNumber(int _num)
    {
        transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = _num.ToString();
        gameObject.tag = "Brick";
    }

    public void OnCreate()
    {
        GameController.gv.BrickCreateVibration();
        GameObject createPar = Instantiate(CreateObjectParticles, transform.position, Quaternion.identity);
        Destroy(createPar, 4f);
        if(brickAudio == null) { brickAudio = transform.Find("BrickAudio").GetComponent<BrickAudio>(); }
        brickAudio.PlayBrickCreate();
    }

    public void OnTowerBuilt(int _brickPosInTower)
    {
        GameObject towerBuiltPar = Instantiate(TowerBuiltParticles, transform.position, TowerBuiltParticles.transform.rotation);
        Destroy(towerBuiltPar, 4f);
        brickAudio.PlayTowerBuilt(_brickPosInTower);
    }

    public void OnSecondChance()
    {
        rb.centerOfMass = Vector3.down * 0.5f;
    }

    private void OnDie()
    {
        GameController.OnGameOver();
    }

    public void OnPerfectHit(bool _isLastBrickOfTower)
    {
        GameObject perfectHitPar = Instantiate(PerfectHitPartcicles, transform.position, PerfectHitPartcicles.transform.rotation);
        Destroy(perfectHitPar, 4f);
        rb.centerOfMass = Vector3.down * 0.5f;
        if (!_isLastBrickOfTower)
        {
            GetComponent<MeshRenderer>().material = PerfectHitMaterial;
        }
        brickAudio.PlayPerfectHit(GameController.pc.GetPerfectHitsInARow());
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            OnDie();
        }
    }

}
